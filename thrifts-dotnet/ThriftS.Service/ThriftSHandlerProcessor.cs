using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThriftS.Common;
using ThriftS.IDL;
using ThriftS.Serializer;

namespace ThriftS.Service
{
    /// <summary>
    /// ThriftS请求处理器
    /// </summary>
    internal class ThriftSHandlerProcessor : ThriftSHandler.Iface
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ThriftSHandlerProcessor()
        {
        }

        /// <summary>
        /// 请求处理
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>response</returns>
        public ThriftSResponse Process(ThriftSRequest request)
        {
            if (request == null
                || string.IsNullOrWhiteSpace(request.ServiceName)
                || string.IsNullOrWhiteSpace(request.MethodName))
            {
                throw new BadRequestException(request, "The ServiceName or MethodName must be not null.");
            }

            if (LocalCache.ServiceDictionary.ContainsKey(request.ServiceName) == false)
            {
                throw new BadRequestException(request, "Service not found.");
            }

            if (LocalCache.ServiceDictionary[request.ServiceName].ContainsKey(request.MethodName) == false)
            {
                throw new BadRequestException(request, "Method not found.");
            }

            ThriftSEnvirnment.Logger.Debug("Accept request: {0}", request.Uri);

            var serviceMetadata = LocalCache.ServiceDictionary[request.ServiceName][request.MethodName];

            // 创建服务实例
            var service = Activator.CreateInstance(serviceMetadata.ServiceHandlerType);

            // 参数赋值
            var methodParameters = serviceMetadata.Method.GetParameters();
            var invokeParameters = new Dictionary<string, object>();

            for (var i = 0; i < methodParameters.Length; i++)
            {
                var parameter = methodParameters[i];

                // 在跨平台的时候服务端和客户端的参数名可能完全不一样，因此唯一依据是参数顺序，请求上下文的参数名以服务端为基准。
                request.Parameters[i].Name = parameter.Name;
                try
                {
                    ThriftSEnvirnment.Logger.Debug(string.Format(
                        new FileSizeFormatProvider(),
                        "Request parameter:{0}, ContentType:{1}, Compress:{2}, Length:{3:fs}",
                        parameter.Name,
                        request.Parameters[i].ContentType,
                        request.Parameters[i].Compression,
                        request.Parameters[i].HasValue ? request.Parameters[i].Value.Length : 0));

                    // 解压
                    if (request.Parameters[i].Compression == ThriftSCompression.Gzip)
                    {
                        request.Parameters[i].Value = Utils.GzipUnCompress(request.Parameters[i].Value);
                    }

                    if (request.Parameters[i].HasValue == false)
                    {
                        invokeParameters.Add(parameter.Name, null);
                    }
                    else
                    {
                        //if (request.Parameters[i].ContentType == ContentTypes.Protobuf)
                        //{
                        //    invokeParameters.Add(
                        //        parameter.Name,
                        //        Utils.DeserializeFromBytes(parameter.ParameterType, request.Parameters[i].Value));
                        //}
                        //else if (request.Parameters[i].ContentType == ContentTypes.Json)
                        //{
                        //    invokeParameters.Add(
                        //        parameter.Name,
                        //        Utils.DeserializeFromJson(parameter.ParameterType, request.Parameters[i].Value));
                        //}else 
                        if (request.Parameters[i].ContentType == ContentTypes.Thrift)
                        {
                            invokeParameters.Add(
                                parameter.Name,
                                ThriftSerializer.Deserialize(parameter.ParameterType, request.Parameters[i].Value));
                        }
                        else
                        {
                            throw new NotSupportedException(string.Format("Not supported content type: {0}", request.Parameters[i].ContentType));
                        }  
                    }
                }
                catch (Exception ex)
                {
                    throw new BadRequestException(
                        request,
                        string.Format("Parsing error on parameter '{0}'. {1}", parameter.Name, ex.Message));
                }
            }

            var stopwatch = new Stopwatch();
            object result = null;
            var xresponse = new ThriftSResponse()
            {
                Headers = new Dictionary<string, string>()
            };

            xresponse.Version = Utils.Version;

            try
            {
                stopwatch.Start();

                try
                {
                    // 关联调用上下文
                    ThriftSContext.Current = new ThriftSContext()
                    {
                        Request = request
                    };

                    Func<object> action = () => serviceMetadata.Handle(service, invokeParameters.Values.ToArray());

                    if (serviceMetadata.Attribute.IsOneWay)
                    {
                        action.BeginInvoke(null, null);
                    }
                    else
                    {
                        result = action();
                    }

                    /*
                    //异步调用，可以做oneway和timeout
                    //Func<object> action = () =>
                    //{
                    //    return serviceMetadata.Handle(service, invokeParameters.Values.ToArray());
                    //};

                    //IAsyncResult asyncResult = action.BeginInvoke(null, null);
                    //var isGetSignal = asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(60));
                    //if (isGetSignal == false)
                    //{
                    //    throw new TimeoutException("Execution timeout.");
                    //}
                    //result = action.EndInvoke(asyncResult);
                     */
                }
                catch (Exception ex)
                {
                    throw new InvocationException()
                    {
                        Request = request,
                        ErrorMessage = ex.Message,
                        ErrorDescription = ex.ToString()
                    };
                }

                // 检查返回值
                if (serviceMetadata.Method.ReturnType != typeof(void))
                {
                    if (result != null)
                    {
                        //if (Utils.GetSerializerMode(result.GetType()) == SerializerMode.ProtoBuf)
                        //{
                        //    xresponse.Result = new ThriftSResult()
                        //    {
                        //        ContentType = ContentTypes.Protobuf,
                        //        Compression = ThriftSCompression.None,
                        //        Data = Utils.SerializeToBytes(result)
                        //    };
                        //}
                        //else if (Utils.GetSerializerMode(result.GetType()) == SerializerMode.Json)
                        //{
                        //    xresponse.Result = new ThriftSResult()
                        //    {
                        //        ContentType = ContentTypes.Json,
                        //        Compression = ThriftSCompression.None,
                        //        Data = Utils.SerializeToJson(result)
                        //    };
                        //}
                        //else
                        {
                            xresponse.Result = new ThriftSResult()
                            {
                                ContentType = ContentTypes.Thrift,
                                Compression = ThriftSCompression.None,
                                Data = ThriftSerializer.Serialize(result)
                            };
                        }

                        // 大于10K开启压缩
                        if (xresponse.Result.Data.Length > 10 * 1024)
                        {
                            xresponse.Result.Data = Utils.GzipCompress(xresponse.Result.Data);
                            xresponse.Result.Compression = ThriftSCompression.Gzip;
                        }

                        ThriftSEnvirnment.Logger.Debug(string.Format(
                            new FileSizeFormatProvider(),
                            "Render response. ContentType:{0}, Compress:{1}, Length:{2:fs}", 
                            xresponse.Result.ContentType, 
                            xresponse.Result.Compression, 
                            xresponse.Result.Data.Length));
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is InvocationException)
                {
                    throw;
                }

                throw new InternalServerException()
                {
                    Request = request,
                    ErrorMessage = ex.Message,
                    ErrorDescription = ex.ToString()
                };
            }
            finally
            {
                stopwatch.Stop();

                ThriftSEnvirnment.Logger.Debug("Request end. Took: {0}ms", stopwatch.Elapsed.TotalMilliseconds);
            }

            return xresponse;
        }

        /// <summary>
        /// Ping
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>response</returns>
        public ThriftSResponse Ping(ThriftSRequest request)
        {
            if (request == null)
            {
                ThriftSEnvirnment.Logger.Debug("Received ping.");
            }
            else
            {
                ThriftSEnvirnment.Logger.Debug(
                    string.Format("Received ping from {0} [{1}].", request.ClientHostName, request.ClientIP));
            }

            var xresponse = new ThriftSResponse()
            {
                Headers = new Dictionary<string, string>(),
                Result = new ThriftSResult()
                {
                    Compression = ThriftSCompression.None,
                    ContentType = ContentTypes.Text,
                    Data = Encoding.UTF8.GetBytes("pong")
                }
            };

            return xresponse;
        }

        /// <summary>
        /// Hello
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>response</returns>
        public ThriftSResponse Hello(ThriftSRequest request)
        {
            // 考虑是否需要抛出客户端连接事件到外部
            var serverProperties = new Dictionary<string, string>();
            serverProperties.Add("thrifts_server_version", Utils.Version);

            var xresponse = new ThriftSResponse()
            {
                Headers = serverProperties
            };

            return xresponse;
        }
    }
}
