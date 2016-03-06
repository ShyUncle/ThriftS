using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThriftS.Common
{
    /// <summary>
    /// ThriftS异常
    /// </summary>
    public class ThriftSException : ApplicationException
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="message">异常信息</param>
        public ThriftSException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="innerException">内部异常</param>
        public ThriftSException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="description">异常描述</param>
        public ThriftSException(string message, string description)
            : base(message)
        {
            this.Description = description;
        }

        /// <summary>
        /// 异常描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 字符串描述
        /// </summary>
        /// <returns>描述信息</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Description) == false)
            {
                return this.Description;
            }

            return base.ToString();
        }
    }
}
