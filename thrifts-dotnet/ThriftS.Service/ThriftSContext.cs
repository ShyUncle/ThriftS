using System.Runtime.Remoting.Messaging;
using ThriftS.IDL;

namespace ThriftS.Service
{
    /// <summary>
    /// ThriftS调用上下文
    /// </summary>
    public class ThriftSContext
    {
        /// <summary>
        /// 当前ThriftS调用
        /// </summary>
        public static ThriftSContext Current
        {
            get
            {
                return (ThriftSContext)CallContext.LogicalGetData("thriftS_request_context");
            }

            internal set
            {
                CallContext.LogicalSetData("thriftS_request_context", value);
            }
        }

        /// <summary>
        /// ThriftS请求
        /// </summary>
        public ThriftSRequest Request { get; set; }
    }
}