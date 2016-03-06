using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThriftS.Common;

namespace ThriftS.Service
{
    /// <summary>
    /// ThriftS运行环境
    /// </summary>
    public static class ThriftSEnvirnment
    {
        /// <summary>
        /// hostConfiguration
        /// </summary>
        private static HostConfigurationSetion hostConfiguration = null;

        /// <summary>
        /// Initializes static members of the <see cref="ThriftSEnvirnment" /> class.
        /// </summary>
        static ThriftSEnvirnment()
        {
            Logger = new ThriftSLogger();
        }

        /// <summary>
        /// Logger
        /// </summary>
        /// <value>The logger.</value>
        public static IThriftSLogger Logger { get; set; }

        /// <summary>
        /// 获取配置文件
        /// </summary>
        internal static HostConfigurationSetion Configuration
        {
            get
            {
                if (hostConfiguration != null)
                {
                    return hostConfiguration;
                }

                // load configuration file
                var map = new ExeConfigurationFileMap();
                map.ExeConfigFilename = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\ThriftS.config";
                if (File.Exists(map.ExeConfigFilename) == false)
                {
                    throw new ThriftSException("Missing thrifts configuration file.");
                }

                var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                hostConfiguration = (HostConfigurationSetion)config.GetSection("hostConfiguration");
                return hostConfiguration;
            }
        }

        /// <summary>
        /// MinThreadPoolSize
        /// </summary>
        internal static int MinThreadPoolSize { get; set; }

        /// <summary>
        /// MaxThreadPoolSize
        /// </summary>
        internal static int MaxThreadPoolSize { get; set; }

        /// <summary>
        /// ClientTimeout
        /// </summary>
        internal static int ClientTimeout { get; set; }
    }
}
