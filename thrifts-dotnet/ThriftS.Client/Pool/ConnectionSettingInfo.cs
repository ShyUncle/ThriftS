using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThriftS.Client.Pool
{
    /// <summary>
    /// Class ConnectionSettingInfo.
    /// </summary>
    internal class ConnectionSettingInfo : IEquatable<ConnectionSettingInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionSettingInfo"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="timeout">The timeout.</param>
        public ConnectionSettingInfo(string host, int port, int timeout)
        {
            this.Host = host;
            this.Port = port;
            this.Timeout = timeout;
        }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port { get; private set; }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>The host.</value>
        public string Host { get; private set; }

        /// <summary>
        /// Gets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        public int Timeout { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("host={0};port={1};timeout={2}", this.Host, this.Port, this.Timeout);
        }

        #region IEquatable<Server> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(ConnectionSettingInfo other)
        {
            return other != null && this.Host == other.Host && this.Port == other.Port && this.Timeout == other.Timeout;
        }

        #endregion

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return obj is ConnectionSettingInfo && this.Equals((ConnectionSettingInfo)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return this.Host.GetHashCode() ^ this.Port.GetHashCode() ^ this.Timeout.GetHashCode();
        }
    }
}
