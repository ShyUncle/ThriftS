using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThriftS.Service
{
    /// <summary>
    /// ThriftS Service集合
    /// </summary>
    internal class ServiceConfigurationElements : ConfigurationElementCollection
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
                return "service";
            }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>ServiceConfigurationElement</returns>
        public new ServiceConfigurationElement this[string name]
        {
            get
            {
                return BaseGet(name) as ServiceConfigurationElement;
            }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>ServiceConfigurationElement</returns>
        public ServiceConfigurationElement this[int index]
        {
            get
            {
                return BaseGet(index) as ServiceConfigurationElement;
            }
        }

        /// <summary>
        /// CreateNewElement
        /// </summary>
        /// <returns>ConfigurationElement</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceConfigurationElement();
        }

        /// <summary>
        /// GetElementKey
        /// </summary>
        /// <param name="element">element</param>
        /// <returns>Name</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ServiceConfigurationElement).Contract;
        }
    }
}
