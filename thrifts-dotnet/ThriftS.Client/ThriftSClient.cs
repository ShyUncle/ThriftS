using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Thrift.Protocol;
using Thrift.Transport;
using ThriftS.Client.Pool;
using ThriftS.Common;
using ThriftS.Common.Attributes;
using ThriftS.IDL;

namespace ThriftS.Client
{
    /// <summary>
    /// ThriftS客户端.
    /// </summary>
    public class ThriftSClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThriftSClient"/> class.
        /// </summary>
        /// <param name="host">主机名或者IP地址.</param>
        /// <param name="port">端口号.</param>
        public ThriftSClient(string host, int port) :
            this(host, port, 120)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThriftSClient"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="timeout">The timeout.</param>
        public ThriftSClient(string host, int port, int timeout)
        {
            this.Host = host;
            this.Port = port;
            this.Timeout = timeout;
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
        /// 超时时间(秒)
        /// </summary>
        /// <value>The timeout.</value>
        public int Timeout { get; set; }

        /// <summary>
        /// 创建服务访问代理
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>代理实例</returns>
        public T CreateProxy<T>() where T : class
        {
            if (typeof(T).IsDefined(typeof(ThriftSContractAttribute), false) == false)
            {
                throw new ThriftSException(string.Format("Missing ThriftSContractAttribute in '{0}'.", typeof(T)));
            }

            var serviceName = Utils.GetServiceName(typeof(T));
            var serviceShortName = serviceName;
            if (string.IsNullOrEmpty(serviceName))
            {
                serviceName = typeof(T).FullName;
                serviceShortName = typeof(T).Name;
            }

            return (T)new ThriftSRealProxy<T>(
                    this.Host,
                    this.Port,
                    serviceName,
                    serviceShortName,
                    this.Timeout).GetTransparentProxy();
        }

        /// <summary>
        /// Ping.
        /// </summary>
        public void Ping()
        {
            var pool = ConnectionPoolManager.GetPool(this.Host, this.Port, this.Timeout);
            ThriftSConnection connection = null;
            try
            {
                connection = pool.Borrow();

                var xrequest = new ThriftSRequest();
                xrequest.Headers = new Dictionary<string, string>();
                xrequest.ClientHostName = Environment.MachineName;
                xrequest.ClientIP = connection.LocalAddress.ToString();

                connection.Client.Ping(xrequest);
            }
            catch (TTransportException exception)
            {
                pool.ReportError(connection, exception);
                throw;
            }
            catch (TProtocolException exception)
            {
                pool.ReportError(connection, exception);
                throw;
            }
            catch (SocketException exception)
            {
                pool.ReportError(connection, exception);
                throw;
            }
            catch (IOException exception)
            {
                pool.ReportError(connection, exception);
                throw;
            }
            finally
            {
                pool.Release(connection);
            }
        }
    }
}