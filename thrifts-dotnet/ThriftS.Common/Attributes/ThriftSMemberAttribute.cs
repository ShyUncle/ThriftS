using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThriftS.Common.Attributes
{
    /// <summary>
    /// ThriftS Member
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ThriftSMemberAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tag">unique identify</param>
        public ThriftSMemberAttribute(short tag)
        {
            Tag = tag;
        }

        /// <summary>
        /// Gets the unique tag used to identify this member within the type.
        /// </summary>
        public short Tag { get; private set; }
    }
}
