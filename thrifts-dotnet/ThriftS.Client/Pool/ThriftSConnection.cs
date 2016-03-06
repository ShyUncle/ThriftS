using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thrift.Protocol;
using Thrift.Transport;
using ThriftS.IDL;

namespace ThriftS.Client.Pool
{
    /// <summary>
    /// Class ThriftSConnection.
    /// </summary>
    internal class ThriftSConnection : IDisposable
    {
        /// <summary>
        /// The _lock
        /// </summary>
        private readonly object lockObj = new object();

        /// <summary>
        /// The socket
        /// </summary>
        private TSocket socket = null;

        /// <summary>
        /// The _transport
        /// </summary>
        private TTransport transport = null;

        /// <summary>
        /// The _client
        /// </summary>
        private ThriftSHandler.Client client = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThriftSConnection"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public ThriftSConnection(ConnectionSettingInfo connection)
        {
            this.IsErrorOccurred = false;
            this.LastActiveTime = DateTime.UtcNow;
            this.ConnectionSetting = connection;

            this.Initialize();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ThriftSConnection" /> class.
        /// </summary>
        ~ThriftSConnection()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the LastActiveTime.
        /// </summary>
        /// <value>The LastActiveTime.</value>
        public DateTime LastActiveTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the connection setting.
        /// </summary>
        /// <value>The connection setting.</value>
        public ConnectionSettingInfo ConnectionSetting
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>The client.</value>
        public ThriftSHandler.Client Client
        {
            get
            {
                lock (this.lockObj)
                {
                    return this.client;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public bool IsOpen
        {
            get
            {
                if (this.transport == null)
                {
                    return false;
                }

                lock (this.lockObj)
                {
                    return this.transport.IsOpen;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether [was disposed].
        /// </summary>
        /// <value><c>true</c> if [was disposed]; otherwise, <c>false</c>.</value>
        public bool Disposed
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the local address.
        /// </summary>
        /// <value>The local address.</value>
        public System.Net.IPAddress LocalAddress
        {
            get; 
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is error occurred.
        /// </summary>
        /// <value><c>true</c> if this instance is error occurred; otherwise, <c>false</c>.</value>
        public bool IsErrorOccurred 
        { 
            get; 
            internal set; 
        }

        /// <summary>
        /// Opens this instance.
        /// </summary>
        public void Open()
        {
            this.CheckWasDisposed();
            
            if (this.IsOpen)
            {
                this.LastActiveTime = DateTime.UtcNow;
                return;
            }

            if (this.transport == null)
            {
                this.Initialize();
            }

            lock (this.lockObj)
            {
                this.transport.Open();
                this.LastActiveTime = DateTime.UtcNow;
                this.LocalAddress = ((System.Net.IPEndPoint)this.socket.TcpClient.Client.LocalEndPoint).Address;
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            this.CheckWasDisposed();

            if (!this.IsOpen)
            {
                return;
            }

            lock (this.lockObj)
            {
                this.transport.Close();
                this.transport = null;
                this.client = null;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("{0}/{1}", this.ConnectionSetting, this.LastActiveTime);
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.Disposed && disposing && this.transport != null)
            {
                this.Close();
            }

            this.Disposed = true;
        }

        /// <summary>
        /// Checks the was disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">connection has been disposed.</exception>
        private void CheckWasDisposed()
        {
            if (this.Disposed)
            {
                throw new ObjectDisposedException("connection has been disposed.");
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            this.socket = new TSocket(this.ConnectionSetting.Host, this.ConnectionSetting.Port, this.ConnectionSetting.Timeout * 1000);
            this.socket.TcpClient.NoDelay = true;
            this.transport = new TFramedTransport(this.socket);
            var protocol = new TMultiplexedProtocol(new TBinaryProtocol(this.transport), "ThriftSHandler");
            this.client = new ThriftSHandler.Client(protocol);
        }

        #endregion
    }
}
