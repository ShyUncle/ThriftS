using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThriftS.Common.Attributes
{
    /// <summary>
    /// ThriftS契约
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class ThriftSContractAttribute : Attribute
    {
        /// <summary>
        /// 契约名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 契约版本
        /// </summary>
        public int Version { get; set; }
    }
}
