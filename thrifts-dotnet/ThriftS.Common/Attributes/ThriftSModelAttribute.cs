using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThriftS.Common.Attributes
{
    /// <summary>
    /// ThriftS Model
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ThriftSModelAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ThriftSModelAttribute()
        {
            this.SerializerMode = SerializerMode.Thrift;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mode">序列化方式</param>
        public ThriftSModelAttribute(SerializerMode mode) : this()
        {
            SerializerMode = mode;
        }

        /// <summary>
        /// 序列化方式
        /// </summary>
        public SerializerMode SerializerMode { get; set; }
    }
}
