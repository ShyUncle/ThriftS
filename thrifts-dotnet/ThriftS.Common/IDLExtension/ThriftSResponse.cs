using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThriftS.IDL
{
    /// <summary>
    /// ThriftS响应
    /// </summary>
    public partial class ThriftSResponse
    {
        /// <summary>
        /// KeyVersion
        /// </summary>
        private static string keyVersion = "server_version";

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
    }
}
