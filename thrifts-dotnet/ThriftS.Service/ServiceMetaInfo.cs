using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ThriftS.Common.Attributes;

namespace ThriftS.Service
{
    /// <summary>
    /// 服务方法元数据信息
    /// </summary>
    internal class ServiceMetaInfo
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 服务实现类型
        /// </summary>
        public Type ServiceHandlerType { get; set; }

        /// <summary>
        /// 方法信息
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// 特性标签
        /// </summary>
        public ThriftSOperationAttribute Attribute { get; set; }

        /// <summary>
        /// 方法调用委托
        /// </summary>
        public FastInvokeHandler Handle { get; set; }
    }
}
