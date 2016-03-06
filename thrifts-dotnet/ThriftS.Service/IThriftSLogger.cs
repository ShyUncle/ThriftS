using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThriftS.Service
{
    /// <summary>
    /// ThriftS日志接口
    /// </summary>
    public interface IThriftSLogger
    {
        /// <summary>
        /// 写Debug日志
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        void Debug(string format, params object[] args);

        /// <summary>
        /// 写Info日志
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        void Info(string format, params object[] args);

        /// <summary>
        /// 写Error日志
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        void Error(string format, params object[] args);

        /// <summary>
        /// 写Error日志
        /// </summary>
        /// <param name="exception">The exception.</param>
        void Error(Exception exception);
    }

    /// <summary>
    /// logger
    /// </summary>
    internal class ThriftSLogger : IThriftSLogger
    {
        /// <summary>
        /// Debugs the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public virtual void Debug(string format, params object[] args)
        {
            Trace.WriteLine(string.Format(format, args));
        }

        /// <summary>
        /// Informations the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public virtual void Info(string format, params object[] args)
        {
            Trace.WriteLine(string.Format(format, args));
        }

        /// <summary>
        /// Errors the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public virtual void Error(string format, params object[] args)
        {
            Trace.WriteLine(string.Format(format, args));
        }

        /// <summary>
        /// Errors the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public virtual void Error(Exception exception)
        {
            Trace.WriteLine(exception);
        }
    }
}
