using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using Thrift;
using Thrift.Protocol;
using Thrift.Transport;
using ThriftS.Client.Pool;
using ThriftS.Common;
using ThriftS.Common.Attributes;
using ThriftS.IDL;
using ThriftS.Serializer;

namespace ThriftS.Client
{
    /// <summary>
    /// Class ThriftSRealProxy.
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    internal class ThriftSRealProxy<T> : RealProxy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThriftSRealProxy{T}"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="serviceShortName">Short name of the service.</param>
        /// <param name="timeout">The timeout.</param>
        public ThriftSRealProxy(string host, int port, string serviceName, string serviceShortName, int timeout)
            : base(typeof(T))
        {
            this.Host = host;
            this.Port = port;
            this.ServiceName = serviceName;
            this.ServiceShortName = serviceShortName;
            this.Timeout = timeout;
            this.ClientPid = Process.GetCurrentProcess().Id;
            this.ClientHostName = Environment.MachineName;
        }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        /// <value>The name of the service.</value>
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        /// <value>The name of the service.</value>
        public string ServiceShortName { get; set; }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        public int Timeout { get; set; }

        /// <summary>
        /// Client Pid
        /// </summary>
        public int ClientPid { get; set; }

        /// <summary>
        /// Client IP
        /// </summary>
        public string ClientIP { get; set; }

        /// <summary>
        /// Client HostName
        /// </summary>
        public string ClientHostName { get; set; }

        /// <summary>
        /// Invokes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>IMessage.</returns>
        public override IMessage Invoke(IMessage message)
        {
            var mcm = (IMethodCallMessage)message;
            IMethodReturnMessage returnMessage = null;

            if (mcm.MethodBase.IsDefined(typeof(ThriftSOperationAttribute), false) == false)
            {
                throw new ThriftSException(string.Format("Missing ThriftSOperationAttribute in '{0}.{1}'.", this.ServiceName, mcm.MethodName));
            }

            object returnValue = null;

            var stopwatch = new Stopwatch();

            string invokeDesc = string.Format("调用{0}契约的{1}方法", this.ServiceName, mcm.MethodName);

            // 构造请求
            var xrequest = new ThriftSRequest()
            {
                ServiceName = this.ServiceName,
                MethodName = Utils.GetMethodName((MethodInfo)mcm.MethodBase),
                Headers = new Dictionary<string, string>(),
                Parameters = new List<ThriftSParameter>()
            };

            xrequest.Uri = string.Format("thrift://{0}:{1}/{2}/{3}", this.Host, this.Port, this.ServiceShortName, xrequest.MethodName);
            xrequest.Version = Utils.Version;
            xrequest.ClientPid = this.ClientPid;
            xrequest.ClientHostName = this.ClientHostName;
            xrequest.ClientRuntime = ".NET " + Environment.Version.ToString();

            try
            {
                // 计时开始
                stopwatch.Start();
                if (mcm.Args != null)
                {
                    for (var i = 0; i < mcm.Args.Length; i++)
                    {
                        var argument = mcm.Args[i];

                        ThriftSParameter parameter = null;
                        if (argument == null)
                        {
                            parameter = new ThriftSParameter()
                            {
                                Index = (sbyte)i,
                                Name = string.Empty,
                                Type = string.Empty,
                                Compression = ThriftSCompression.None,
                                ContentType = ContentTypes.Binary,
                                HasValue = false
                            };
                        }
                        else
                        {
                            parameter = new ThriftSParameter()
                            {
                                Index = (sbyte)i,
                                Name = string.Empty,
                                Type = string.Empty,
                                Compression = ThriftSCompression.None,
                                ContentType = ContentTypes.Thrift,
                                HasValue = true,
                                Value = ThriftSerializer.Serialize(argument)
                            };
                        }

                        // 大于10K开启压缩
                        if (parameter.Value != null && parameter.Value.Length > 10 * 1024)
                        {
                            parameter.Value = Utils.GzipCompress(parameter.Value);
                            parameter.Compression = ThriftSCompression.Gzip;
                        }

                        xrequest.Parameters.Add(parameter);
                    }
                }

                ThriftSResponse xresponse = null;
                try
                {
                    var xpool = ConnectionPoolManager.GetPool(this.Host, this.Port, this.Timeout);
                    ThriftSConnection xconnection = null;
                    TimeSpan connectTookTime = TimeSpan.MinValue;
                    try
                    {
                        DateTime beginConnectTime = DateTime.Now;
                        xconnection = xpool.Borrow();
                        connectTookTime = beginConnectTime - DateTime.Now;

                        if (string.IsNullOrEmpty(this.ClientIP))
                        {
                            this.ClientIP = xconnection.LocalAddress.ToString();
                        }

                        xrequest.ClientIP = this.ClientIP;
                        xresponse = xconnection.Client.Process(xrequest);
                    }
                    catch (SocketException exception)
                    {
                        xpool.ReportError(xconnection, exception);
                        this.HandleException("SocketException", invokeDesc, xrequest, exception);
                    }
                    catch (IOException exception)
                    {
                        xpool.ReportError(xconnection, exception);
                        var socketException = exception.InnerException as SocketException;
                        if (socketException != null)
                        {
                            this.HandleException("SocketException", invokeDesc, xrequest, socketException);
                        }

                        this.HandleException("IOException", invokeDesc, xrequest, exception);
                    }
                    catch (TTransportException exception)
                    {
                        xpool.ReportError(xconnection, exception);

                        // 5秒以内的timeout认为是服务器积极拒绝.
                        if (exception.Message.StartsWith("Connect timed out")
                            && connectTookTime.TotalSeconds < 5)
                        {
                            // 处理异常
                            this.HandleException(
                                "TTransportException", 
                                invokeDesc, 
                                xrequest,
                                new TTransportException(exception.Type, "Service unavailable."));
                        }
                        else
                        {
                            this.HandleException("TTransportException", invokeDesc, xrequest, exception);
                        }
                    }
                    catch (TProtocolException exception)
                    {
                        xpool.ReportError(xconnection, exception);
                        this.HandleException("TProtocolException", invokeDesc, xrequest, exception);
                    }
                    finally
                    {
                        // 内部Try可以更快的释放连接。
                        xpool.Release(xconnection);
                    }

                    // 非void且result非空
                    if (xresponse != null && xresponse.Result != null)
                    {
                        // 解压
                        if (xresponse.Result.Compression == ThriftSCompression.Gzip)
                        {
                            xresponse.Result.Data = Utils.GzipUnCompress(xresponse.Result.Data);
                        }

                        if (xresponse.Result.ContentType == ContentTypes.Thrift)
                        {
                            returnValue = ThriftSerializer.Deserialize(
                                ((MethodInfo)mcm.MethodBase).ReturnType,
                                xresponse.Result.Data);
                        }
                        else
                        {
                            throw new NotSupportedException(string.Format("Not supported content type: {0}", xresponse.Result.ContentType)); 
                        }
                    }

                    returnMessage = new ReturnMessage(returnValue, null, 0, mcm.LogicalCallContext, mcm);

                }
                catch (TApplicationException tapplicationException)
                {
                    var info = string.Format("tapplication exception on calling {0}. ", xrequest.Uri);
                    var exception = new ThriftSException(info + tapplicationException.Message, tapplicationException);

                    returnMessage = new ReturnMessage(exception, mcm);
                }
                catch (BadRequestException badRequestException)
                {
                    var info = string.Format("Bad request exception on calling {0}. ", xrequest.Uri);
                    var exception = new ThriftSException(
                        info + badRequestException.ErrorMessage,
                        info + Environment.NewLine + badRequestException.ErrorMessage);

                    // 远端异常使用ReturnMessage包装
                    returnMessage = new ReturnMessage(exception, mcm);
                }
                catch (InternalServerException internalServerException)
                {
                    var info = string.Format("Server internal exception on calling {0}. ", xrequest.Uri);
                    var exception = new ThriftSException(
                        info + internalServerException.ErrorMessage,
                        info + Environment.NewLine + internalServerException.ErrorDescription);

                    returnMessage = new ReturnMessage(exception, mcm);
                }
                catch (InvocationException invocationException)
                {
                    var info = string.Format("Server invocation exception on calling {0}. ", xrequest.Uri);
                    var exception = new ThriftSException(
                        info + invocationException.ErrorMessage,
                        info + Environment.NewLine + invocationException.ErrorDescription);

                    returnMessage = new ReturnMessage(exception, mcm);
                }
                catch (Exception exception)
                {
                    this.HandleException("Exception", invokeDesc, xrequest, exception);
                }
            }
            finally
            {
                stopwatch.Stop();
            }

            return returnMessage;
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="exceptionName">Name of the exception.</param>
        /// <param name="invokeDesc">The invoke description.</param>
        /// <param name="request">The request.</param>
        /// <param name="exception">The exception.</param>
        private void HandleException(string exceptionName, string invokeDesc, ThriftSRequest request, Exception exception)
        {
            throw exception;
        }
    }
}
