using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThriftS.Service
{
    /// <summary>
    /// ServiceConfigurationElement
    /// </summary>
    internal class ServiceConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// 契约名称
        /// </summary>
        [ConfigurationProperty("contract")]
        public string Contract
        {
            get { return (string)this["contract"]; }
            set { this["contract"] = value; }
        }

        /// <summary>
        /// 契约Assembly
        /// </summary>
        [ConfigurationProperty("contractAssembly")]
        public string ContractAssembly
        {
            get { return (string)this["contractAssembly"]; }
            set { this["contractAssembly"] = value; }
        }

        /// <summary>
        /// 处理器
        /// </summary>
        [ConfigurationProperty("handler")]
        public string Handler
        {
            get { return (string)this["handler"]; }
            set { this["handler"] = value; }
        }

        /// <summary>
        /// 处理器Assembly
        /// </summary>
        [ConfigurationProperty("handlerAssembly")]
        public string HandlerAssembly
        {
            get { return (string)this["handlerAssembly"]; }
            set { this["handlerAssembly"] = value; }
        }
    }
}
