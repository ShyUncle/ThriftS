using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThriftS.Common.Attributes
{
    /// <summary>
    /// ThriftS方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ThriftSOperationAttribute : Attribute
    {
        /// <summary>
        /// 别名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否异步方法
        /// </summary>
        public bool IsOneWay { get; set; }

        /// <summary>
        /// 描述信息，为监控日志提供接口说明
        /// </summary>
        public string Description { get; set; }

        /*
        //方法执行超时设定
        //public int Timeout { get; set; }
         * */
    }
}
