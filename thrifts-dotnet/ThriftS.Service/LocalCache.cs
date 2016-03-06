using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThriftS.Service
{
    /// <summary>
    /// 本地进程内缓存
    /// </summary>
    internal static class LocalCache
    {
        /// <summary>
        /// 服务方法元数据
        /// </summary>
        public static readonly ConcurrentDictionary<string, Dictionary<string, ServiceMetaInfo>> ServiceDictionary =
            new ConcurrentDictionary<string, Dictionary<string, ServiceMetaInfo>>(StringComparer.OrdinalIgnoreCase);
    }
}
