using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThriftS.Common
{
    /// <summary>
    /// 序列化方式
    /// </summary>
    public enum SerializerMode
    {
        /// <summary>
        /// Thrift序列化
        /// </summary>
        Thrift = 1,

        /// <summary>
        /// ProtoBuf序列化
        /// </summary>
        ProtoBuf = 2,

        /// <summary>
        /// Json序列化
        /// </summary>
        Json = 3
    }
}
