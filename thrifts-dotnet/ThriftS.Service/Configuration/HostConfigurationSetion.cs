using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThriftS.Service
{
    /// <summary>
    /// ThriftS配置节
    /// </summary>
    internal class HostConfigurationSetion : ConfigurationSection
    {
        /// <summary>
        /// 默认Host
        /// </summary>
        [ConfigurationProperty("defaultHost")]
        public string DefaultHost
        {
            get { return (string)this["defaultHost"]; }
            set { this["defaultHost"] = value; }
        }

        /// <summary>
        /// 日志级别
        /// </summary>
        [ConfigurationProperty("logLevel")]
        public string LogLevel
        {
            get { return (string)this["logLevel"]; }
            set { this["logLevel"] = value; }
        }

        /// <summary>
        /// Hosts
        /// </summary>
        [ConfigurationProperty("hosts")]
        public HostConfigurationElements Hosts
        {
            get { return (HostConfigurationElements)this["hosts"]; }
            set { this["hosts"] = value; }
        }
    }
}
