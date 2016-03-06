using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThriftS.Service
{
    /// <summary>
    /// ThriftS Host集合
    /// </summary>
    internal class HostConfigurationElements : ConfigurationElementCollection
    {
        /// <summary>
        /// CollectionType
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        /// <summary>
        /// ElementName
        /// </summary>
        protected override string ElementName
        {
            get
            {
                return "host";
            }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>HostConfigurationElement</returns>
        public new HostConfigurationElement this[string name]
        {
            get
            {
                return BaseGet(name) as HostConfigurationElement;
            }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>HostConfigurationElement</returns>
        public HostConfigurationElement this[int index]
        {
            get
            {
                return BaseGet(index) as HostConfigurationElement;
            }
        }

        /// <summary>
        /// CreateNewElement
        /// </summary>
        /// <returns>ConfigurationElement</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new HostConfigurationElement();
        }

        /// <summary>
        /// GetElementKey
        /// </summary>
        /// <param name="element">element</param>
        /// <returns>Name</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as HostConfigurationElement).Name;
        }
    }
}
