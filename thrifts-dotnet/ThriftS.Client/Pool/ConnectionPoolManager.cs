using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThriftS.Client.Pool
{
    /// <summary>
    /// Class ConnectionPoolManager.
    /// </summary>
    internal static class ConnectionPoolManager
    {
        /// <summary>
        /// The pools
        /// </summary>
        private static readonly Dictionary<ConnectionSettingInfo, ConnectionPool> Pools = new Dictionary<ConnectionSettingInfo, ConnectionPool>();

        /// <summary>
        /// Gets the pool.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>ConnectionPool.</returns>
        public static ConnectionPool GetPool(string host, int port, int timeout)
        {
            var connection = new ConnectionSettingInfo(host, port, timeout);

            if (Pools.ContainsKey(connection) == false)
            {
                lock (Pools)
                {
                    if (Pools.ContainsKey(connection) == false)
                    {
                        Pools.Add(connection, new ConnectionPool(connection));
                    }
                }
            }

            return Pools[connection];
        }
    }
}
