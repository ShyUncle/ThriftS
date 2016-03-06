using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThriftS.Common;
using ThriftS.Common.Attributes;
using ThriftS.IDL;
using ThriftS.Service.Http;
using Thrift.Protocol;
using Thrift.Server;
using Thrift.Transport;

namespace ThriftS.Service
{
    /// <summary>
    /// ThriftS服务器
    /// </summary>
    public class ThriftSServer
    {
        /// <summary>
        /// Thrift处理器
        /// </summary>
        private TMultiplexedProcessor multiplexedProcessor = new TMultiplexedProcessor();

        /// <summary>
        /// Thrift Server
        /// </summary>
        private TThreadPoolServer threadPoolServer = null;

        /// <summary>
        /// Http Server
        /// </summary>
        private HttpServer httpServer = null;

        /// <summary>
        /// 构造方法
        /// </summary>
        public ThriftSServer()
        {
        }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 注册ThriftS服务
        /// </summary>
        /// <param name="contractType">契约类</param>
        /// <param name="handlerType">契约实现类</param>
        public void RegisterService(Type contractType, Type handlerType)
        {
            if (contractType == null)
            {
                throw new ArgumentNullException("contractType");
            }

            if (handlerType == null)
            {
                throw new ArgumentNullException("handlerType");
            }

            // 检查服务是否继承了接口
            if (contractType.IsInterface == false)
            {
                throw new ThriftSException("The contractType must be an interface.");
            }

            if (contractType.IsAssignableFrom(handlerType) == false)
            {
                throw new ThriftSException("The serviceType must be implementation contractType.");
            }

            // 检查是否存在契约Attribute
            if (contractType.IsDefined(typeof(ThriftSContractAttribute), false) == false)
            {
                throw new ThriftSException(string.Format("Missing ThriftSContractAttribute in '{0}'.", contractType));
            }

            var serviceName = Utils.GetServiceName(contractType);
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                serviceName = contractType.FullName;
            }

            // 检查是否重复注册
            if (LocalCache.ServiceDictionary.ContainsKey(serviceName))
            {
                return;
            }

            // 扫描并注册远程方法
            LocalCache.ServiceDictionary.TryAdd(serviceName, new Dictionary<string, ServiceMetaInfo>(StringComparer.OrdinalIgnoreCase));
            foreach (var method in contractType.GetMethods())
            {
                if (method.IsDefined(typeof(ThriftSOperationAttribute), false) == false)
                {
                    // throw new ThriftSException(string.Format("Missing ThriftSOperationAttribute: {0}", method.ToString()));
                    continue;
                }

                var methodName = Utils.GetMethodName(method);
                LocalCache.ServiceDictionary[serviceName].Add(
                    methodName, 
                    new ServiceMetaInfo()
                    {
                        ServiceName = serviceName,
                        MethodName = methodName,
                        ServiceHandlerType = handlerType,
                        Method = method,
                        Attribute = (ThriftSOperationAttribute)method.GetCustomAttribute(typeof(ThriftSOperationAttribute), false),
                        Handle = FastInvoker.GetMethodInvoker(method)
                    });
            }
        }

        /// <summary>
        /// 启动ThriftS服务器
        /// </summary>
        /// <param name="httpPort">http端口</param>
        /// <param name="thriftPort">thrift端口</param>
        /// <param name="minThreads">最小线程数</param>
        /// <param name="maxThreads">最大线程数</param>
        /// <param name="clientTimeout">客户端超时设置</param>
        /// <param name="useBufferedSockets">启动缓存Socket</param>
        public void Start(int httpPort, int thriftPort, int minThreads, int maxThreads, int clientTimeout, bool useBufferedSockets)
        {
            this.Port = thriftPort;
            var clientTimeoutMS = (int)TimeSpan.FromSeconds(clientTimeout).TotalMilliseconds;
            var serverTransport = new TServerSocket(thriftPort, clientTimeoutMS, useBufferedSockets);

            var logDelegate = new TServer.LogDelegate((message) => ThriftSEnvirnment.Logger.Error(message));

            this.threadPoolServer = new TThreadPoolServer(
                this.multiplexedProcessor, 
                serverTransport, 
                new TFramedTransport.Factory(), 
                new TFramedTransport.Factory(),
                new TBinaryProtocol.Factory(), 
                new TBinaryProtocol.Factory(),
                minThreads, 
                maxThreads, 
                logDelegate);
            this.multiplexedProcessor.RegisterProcessor("ThriftSHandler", new ThriftSHandler.Processor(new ThriftSHandlerProcessor()));

            // this.threadPoolServer.setEventHandler(new ServerEventHandler());
            ThriftSEnvirnment.Logger.Info(
                "Starting thriftS server. ThriftPort:{0}, HttpPort:{1}, Version:{2}",
                thriftPort, 
                httpPort, 
                Utils.Version);
            
            var task = new Task(this.threadPoolServer.Serve);
            task.Start();

            if (httpPort > 0)
            {
                this.httpServer = new SimpleHttpServer(httpPort);
                var httpTask = new Task(this.httpServer.Start);
                httpTask.Start();
            }

            ThriftSEnvirnment.MinThreadPoolSize = minThreads;
            ThriftSEnvirnment.MaxThreadPoolSize = maxThreads;
            ThriftSEnvirnment.ClientTimeout = clientTimeout;
        }

        /// <summary>
        /// 启动ThriftS服务器
        /// </summary>
        /// <param name="name">host名称</param>
        /// <param name="httpPort">http端口</param>
        /// <param name="thriftPort">thrift端口</param>
        public void Start(string name, int httpPort, int thriftPort)
        {
            var hostConfiguration = ThriftSEnvirnment.Configuration;
            var host = hostConfiguration.Hosts[name];

            try
            {
                for (int i = 0; i < host.Services.Count; i++)
                {
                    var serviceEl = host.Services[i];
                    if (string.IsNullOrWhiteSpace(serviceEl.Contract) == false)
                    {
                        var contractType = Assembly.Load(serviceEl.ContractAssembly).GetType(serviceEl.Contract);
                        if (contractType == null)
                        {
                            throw new TypeLoadException(string.Concat("Not found contract type: ", serviceEl.Contract));
                        }

                        var handlerType = Assembly.Load(serviceEl.HandlerAssembly).GetType(serviceEl.Handler);
                        if (handlerType == null)
                        {
                            throw new TypeLoadException(string.Concat("Not found handler type: ", serviceEl.Handler));
                        }

                        this.RegisterService(contractType, handlerType);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new ThriftSException("Load service assembly failed.", exception);
            }

            if (thriftPort > 0)
            {
                host.ThriftPort = thriftPort;
            }

            if (httpPort > 0)
            {
                host.HttpPort = httpPort;
            }

            this.Start(
                host.HttpPort,
                host.ThriftPort, 
                host.MinThreadPoolSize, 
                host.MaxThreadPoolSize, 
                host.ClientTimeout,
                host.UseBufferedSockets);
        }

        /// <summary>
        /// 启动ThriftS服务器
        /// </summary>
        public void Start()
        {
            // 优先使用命令行参数，否则使用配置参数
            int httpPort = 0;
            int thriftPort = 0;
            foreach (var arg in Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("httpport=", StringComparison.OrdinalIgnoreCase))
                {
                    int.TryParse(arg.Split(new[] { '=' }, 2)[1], out httpPort);
                }

                if (arg.StartsWith("thriftport=", StringComparison.OrdinalIgnoreCase))
                {
                    int.TryParse(arg.Split(new[] { '=' }, 2)[1], out thriftPort);
                }
            }

            var hostConfiguration = ThriftSEnvirnment.Configuration;
            this.Start(hostConfiguration.DefaultHost, httpPort, thriftPort);
        }

        /// <summary>
        /// 停止ThriftS服务器
        /// </summary>
        public void Stop()
        {
            if (this.threadPoolServer != null)
            {
                ThriftSEnvirnment.Logger.Info("Stop thriftS server. Port:{0}, Version:{1}", this.Port, Utils.Version);
                this.threadPoolServer.Stop();
            }

            if (this.httpServer != null)
            {
                 this.httpServer.Stop();
            }
        }
    }
}
