using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading;

namespace ThriftS.Client.Pool
{
    /// <summary>
    /// ConnectionPool
    /// </summary>
    internal class ConnectionPool
    {
        /// <summary>
        /// The _lock
        /// </summary>
        private readonly object lockObj = new object();

        /// <summary>
        /// The _free connections
        /// </summary>
        private readonly Queue<ThriftSConnection> freeConnectionQueue = new Queue<ThriftSConnection>();

        /// <summary>
        /// The _used connections
        /// </summary>
        private readonly HashSet<ThriftSConnection> usedConnectionSet = new HashSet<ThriftSConnection>();

        /// <summary>
        /// The _maintenance timer
        /// </summary>
        private readonly Timer maintenanceTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionPool"/> class.
        /// </summary>
        /// <param name="connectionSetting">The server.</param>
        public ConnectionPool(ConnectionSettingInfo connectionSetting)
        {
            this.ConnectionSetting = connectionSetting;

            // 有的服务可能只是初始化用一次。
            // this.MinPoolSize = 0;

            // 每个连接字符串最多128个连接,注意Timeout值不同认为是不同的连接
            this.MaxPoolSize = 128;

            // 连接保持3分钟
            this.KeepaliveTime = TimeSpan.FromSeconds(180);

            // 每30秒清理一次连接
            this.maintenanceTimer = new Timer(o => this.Cleanup(), null, 30000L, 30000L);
        }

        /// <summary>
        /// Gets the server.
        /// </summary>
        /// <value>The server.</value>
        public ConnectionSettingInfo ConnectionSetting { get; private set; }

        /*
        /// <summary>
        /// Gets the minimum size of the pool.
        /// </summary>
        /// <value>The minimum size of the pool.</value>
        public int MinPoolSize { get; private set; }
        */

        /// <summary>
        /// Gets the maximum size of the pool.
        /// </summary>
        /// <value>The maximum size of the pool.</value>
        public int MaxPoolSize { get; private set; }

        /// <summary>
        /// Gets the connection keepalive time.
        /// </summary>
        /// <value>The connection keepalive time.</value>
        public TimeSpan KeepaliveTime { get; private set; }

        /// <summary>
        /// Borrows this instance.
        /// </summary>
        /// <returns>ThriftSConnection.</returns>
        public ThriftSConnection Borrow()
        {
            ThriftSConnection conn = null;

            lock (this.lockObj)
            {
                var stopwatch = new Stopwatch();
                var fcount = this.freeConnectionQueue.Count;
                var ucount = this.usedConnectionSet.Count;
                var isnew = false;

                stopwatch.Start();
                if (this.freeConnectionQueue.Count > 0)
                {
                    conn = this.freeConnectionQueue.Dequeue();
                    conn.Open();
                    this.usedConnectionSet.Add(conn);
                }
                else if (this.freeConnectionQueue.Count + this.usedConnectionSet.Count >= this.MaxPoolSize)
                {
                    if (!Monitor.Wait(this.lockObj, TimeSpan.FromSeconds(30)))
                    {
                        throw new TimeoutException(
                            "No connection could be made, timed out trying to acquire a connection from the connection pool.");
                    }

                    return this.Borrow();
                }
                else
                {
                    conn = new ThriftSConnection(this.ConnectionSetting);
                    conn.Open();
                    this.usedConnectionSet.Add(conn);

                    isnew = true;
                }

                stopwatch.Stop();

                Trace.WriteLine(string.Format(
                    "borrow connection. server: {0}, free: {1} -> {2}, used: {3} -> {4}, isnew: {5}, took: {6}ms.",
                    this.ConnectionSetting,
                    fcount,
                    this.freeConnectionQueue.Count,
                    ucount,
                    this.usedConnectionSet.Count,
                    isnew,
                    stopwatch.ElapsedMilliseconds));
            }

            return conn;
        }

        /// <summary>
        /// Releases the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Release(ThriftSConnection connection)
        {
            // 异常连接不需要再放回池里
            if (connection == null || connection.IsErrorOccurred)
            {
                return false;
            }

            lock (this.lockObj)
            {
                var fcount = this.freeConnectionQueue.Count;
                var ucount = this.usedConnectionSet.Count;

                this.usedConnectionSet.Remove(connection);

                if (this.IsAlive(connection))
                {
                    this.freeConnectionQueue.Enqueue(connection);
                }

                Trace.WriteLine(string.Format(
                    "release connection, server: {0}, free: {1} -> {2}, used: {3} -> {4}",
                    this.ConnectionSetting,
                    fcount,
                    this.freeConnectionQueue.Count,
                    ucount,
                    this.usedConnectionSet.Count));
            }

            return true;
        }

        /// <summary>
        /// 当连接使用时发生socket异常,则标记该连接不可用，在连接池中移除
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="exception">The exception.</param>
        public void ReportError(ThriftSConnection connection, Exception exception)
        {
            if (connection == null)
            {
                return;
            }

            connection.IsErrorOccurred = true;

            lock (this.lockObj)
            {
                this.usedConnectionSet.RemoveWhere(x => x.ConnectionSetting == connection.ConnectionSetting);

                var currentFreeConnections = this.freeConnectionQueue.ToArray();
                this.freeConnectionQueue.Clear();

                foreach (var conn in currentFreeConnections)
                {
                    if (conn.ConnectionSetting != connection.ConnectionSetting)
                    {
                        this.freeConnectionQueue.Enqueue(conn);
                    }
                }
            }
        }

        /// <summary>
        /// Cleans up this instance.
        /// </summary>
        public void Cleanup()
        {
            if (this.freeConnectionQueue.Count == 0)
            {
                return;
            }

            var fcount = this.freeConnectionQueue.Count;
            var ucount = this.usedConnectionSet.Count;
            this.CheckFreeConnectionsAlive();

            Trace.WriteLine(string.Format(
                "clean connection pool, server: {0}, free: {1} -> {2}, used: {3} -> {4}",
                this.ConnectionSetting,
                fcount,
                this.freeConnectionQueue.Count,
                ucount,
                this.usedConnectionSet.Count));
        }

        /// <summary>
        /// Determines whether the connection is alive.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>True if alive; otherwise false.</returns>
        private bool IsAlive(ThriftSConnection connection)
        {
            if (this.KeepaliveTime > TimeSpan.Zero && connection.LastActiveTime.Add(this.KeepaliveTime) < DateTime.UtcNow)
            {
                return false;
            }

            return connection.IsOpen;
        }

        /// <summary>
        /// The check free connections alive.
        /// </summary>
        private void CheckFreeConnectionsAlive()
        {
            lock (this.lockObj)
            {
                var freeConnections = this.freeConnectionQueue.ToArray();
                this.freeConnectionQueue.Clear();

                foreach (var free in freeConnections)
                {
                    if (this.IsAlive(free))
                    {
                        this.freeConnectionQueue.Enqueue(free);
                    }
                    else if (free.IsOpen)
                    {
                        free.Close();

                        Trace.WriteLine(string.Format(
                            "close connection for clean pool. server: {0}.", 
                            this.ConnectionSetting));
                    }
                }
            }
        }
    }
}
