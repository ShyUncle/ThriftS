using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThriftS.Common
{
    /// <summary>
    /// 数据类型
    /// </summary>
    public static class ContentTypes
    {
        /// <summary>
        /// unknown
        /// </summary>
        public static string Unknown
        {
            get { return "unknown"; }
        }

        /// <summary>
        /// thrift
        /// </summary>
        public static string Thrift
        {
            get { return "thrift"; }
        }

        /// <summary>
        /// protobuf
        /// </summary>
        public static string Protobuf
        {
            get { return "protobuf"; }
        }

        /// <summary>
        /// json
        /// </summary>
        public static string Json
        {
            get { return "json"; }
        }

        /// <summary>
        /// avro
        /// </summary>
        public static string Avro
        {
            get { return "avro"; }
        }

        /// <summary>
        /// binary
        /// </summary>
        public static string Binary
        {
            get { return "binary"; }
        }

        /// <summary>
        /// text
        /// </summary>
        public static string Text
        {
            get { return "text"; }
        }
    }
}
