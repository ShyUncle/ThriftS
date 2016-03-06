using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using ServiceStack.Common.Extensions;

namespace ThriftS.IDL
{
    /// <summary>
    /// ThriftS请求
    /// </summary>
    public partial class ThriftSRequest
    {
        /// <summary>
        /// KeyRequestId
        /// </summary>
        private static string keyRequestId = "request_id";

        /// <summary>
        /// KeyRequestUri
        /// </summary>
        private static string keyRequestUri = "request_uri";

        /// <summary>
        /// KeyTrackerId
        /// </summary>
        private static string keyTrackerId = "tracker_id";

        /// <summary>
        /// KeyVersion
        /// </summary>
        private static string keyVersion = "client_version";

        /// <summary>
        /// ClientIP
        /// </summary>
        private static string keyClientIP = "client_ip";

        /// <summary>
        /// ClientHostName
        /// </summary>
        private static string keyClientHostName = "client_hostname";

        /// <summary>
        /// ClientPID
        /// </summary>
        private static string keyClientPid = "client_pid";

        /// <summary>
        /// Client Runtime(jre or .net)
        /// </summary>
        private static string keyClientRuntime = "client_runtime";

        /// <summary>
        /// Accept
        /// </summary>
        private static string keyAccept = "accept";

        /// <summary>
        /// 请求Id
        /// </summary>
        public string RequestId
        {
            get
            {
                if (this.Headers.ContainsKey(keyRequestId) == false)
                {
                    return string.Empty;
                }

                return this.Headers[keyRequestId];
            }

            set
            {
                if (this.Headers.ContainsKey(keyRequestId) == false)
                {
                    this.Headers.Add(keyRequestId, string.Empty);
                }

                this.Headers[keyRequestId] = value;
            }
        }

        /// <summary>
        /// 请求资源路径
        /// </summary>
        public string Uri
        {
            get
            {
                if (this.Headers.ContainsKey(keyRequestUri) == false)
                {
                    return string.Empty;
                }

                return this.Headers[keyRequestUri];
            }

            set
            {
                if (this.Headers.ContainsKey(keyRequestUri) == false)
                {
                    this.Headers.Add(keyRequestUri, string.Empty);
                }

                this.Headers[keyRequestUri] = value;
            }
        }

        /// <summary>
        /// 跟踪Id, 用于串联服务调用栈
        /// </summary>
        public string TrackerId
        {
            get
            {
                if (this.Headers.ContainsKey(keyTrackerId) == false)
                {
                    return string.Empty;
                }

                return this.Headers[keyTrackerId];
            }

            set
            {
                if (this.Headers.ContainsKey(keyTrackerId) == false)
                {
                    this.Headers.Add(keyTrackerId, string.Empty);
                }

                this.Headers[keyTrackerId] = value;
            }
        }

        /// <summary>
        /// 组件版本
        /// </summary>
        public string Version
        {
            get
            {
                if (this.Headers.ContainsKey(keyVersion) == false)
                {
                    return string.Empty;
                }

                return this.Headers[keyVersion];
            }

            set
            {
                if (this.Headers.ContainsKey(keyVersion) == false)
                {
                    this.Headers.Add(keyVersion, string.Empty);
                }

                this.Headers[keyVersion] = value;
            }
        }

        /// <summary>
        /// 客户端IP地址
        /// </summary>
        public string ClientIP
        {
            get
            {
                if (this.Headers.ContainsKey(keyClientIP) == false)
                {
                    return string.Empty;
                }

                return this.Headers[keyClientIP];
            }

            set
            {
                if (this.Headers.ContainsKey(keyClientIP) == false)
                {
                    this.Headers.Add(keyClientIP, string.Empty);
                }

                this.Headers[keyClientIP] = value;
            }
        }

        /// <summary>
        /// 客户端主机名
        /// </summary>
        public string ClientHostName
        {
            get
            {
                if (this.Headers.ContainsKey(keyClientHostName) == false)
                {
                    return string.Empty;
                }

                return this.Headers[keyClientHostName];
            }

            set
            {
                if (this.Headers.ContainsKey(keyClientHostName) == false)
                {
                    this.Headers.Add(keyClientHostName, string.Empty);
                }

                this.Headers[keyClientHostName] = value;
            }
        }

        /// <summary>
        /// 客户端进程号
        /// </summary>
        public int ClientPid
        {
            get
            {
                if (this.Headers.ContainsKey(keyClientPid) == false)
                {
                    return 0;
                }

                return Convert.ToInt32(this.Headers[keyClientPid]);
            }

            set
            {
                if (this.Headers.ContainsKey(keyClientPid) == false)
                {
                    this.Headers.Add(keyClientPid, string.Empty);
                }

                this.Headers[keyClientPid] = value.ToString();
            }
        }

        /// <summary>
        /// 客户端运行时(Java JRE或者.NET CLR版本)
        /// </summary>
        public string ClientRuntime
        {
            get
            {
                if (this.Headers.ContainsKey(keyClientRuntime) == false)
                {
                    return string.Empty;
                }

                return this.Headers[keyClientRuntime];
            }

            set
            {
                if (this.Headers.ContainsKey(keyClientRuntime) == false)
                {
                    this.Headers.Add(keyClientRuntime, string.Empty);
                }

                this.Headers[keyClientRuntime] = value;
            }
        }

        /// <summary>
        /// 接受的ContentType
        /// </summary>
        public string Accept
        {
            get
            {
                if (this.Headers.ContainsKey(keyAccept) == false)
                {
                    return string.Empty;
                }

                return this.Headers[keyAccept];
            }

            set
            {
                if (this.Headers.ContainsKey(keyAccept) == false)
                {
                    this.Headers.Add(keyAccept, string.Empty);
                }

                this.Headers[keyAccept] = value;
            }
        }
    }
}
