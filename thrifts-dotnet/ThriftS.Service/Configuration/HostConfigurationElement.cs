using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThriftS.Service
{
    /// <summary>
    /// HostConfigurationElement
    /// </summary>
    internal class HostConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// 名称
        /// </summary>
        [ConfigurationProperty("name")]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Thrift端口号
        /// </summary>
        [ConfigurationProperty("thriftPort")]
        public int ThriftPort
        {
            get { return (int)this["thriftPort"]; }
            set { this["thriftPort"] = value; }
        }

        /// <summary>
        /// Http端口号
        /// </summary>
        [ConfigurationProperty("httpPort")]
        public int HttpPort
        {
            get { return (int)this["httpPort"]; }
            set { this["httpPort"] = value; }
        }

        /// <summary>
        /// 线程池最小值
        /// </summary>
        [ConfigurationProperty("minThreadPoolSize")]
        public int MinThreadPoolSize
        {
            get { return (int)this["minThreadPoolSize"]; }
            set { this["minThreadPoolSize"] = value; }
        }

        /// <summary>
        /// 线程池最大值
        /// </summary>
        [ConfigurationProperty("maxThreadPoolSize")]
        public int MaxThreadPoolSize
        {
            get { return (int)this["maxThreadPoolSize"]; }
            set { this["maxThreadPoolSize"] = value; }
        }

        /// <summary>
        /// 客户端超时(秒)
        /// </summary>
        [ConfigurationProperty("clientTimeout")]
        public int ClientTimeout
        {
            get { return (int)this["clientTimeout"]; }
            set { this["clientTimeout"] = value; }
        }

        /// <summary>
        /// 是否缓存Sockets
        /// </summary>
        [ConfigurationProperty("useBufferedSockets")]
        public bool UseBufferedSockets
        {
            get { return (bool)this["useBufferedSockets"]; }
            set { this["useBufferedSockets"] = value; }
        }

        /// <summary>
        /// Services
        /// </summary>
        [ConfigurationProperty("services")]
        public ServiceConfigurationElements Services
        {
            get { return (ServiceConfigurationElements)this["services"]; }
            set { this["services"] = value; }
        }
    }
}
