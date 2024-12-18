using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Helper
{
    public class LoggerHelper: ILogger
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LoggerHelper));

        static LoggerHelper()
        {
            // Load konfigurasi log4net
            var log4netConfig = new FileInfo("log4net.config");
            if (log4netConfig.Exists)
            {
                XmlConfigurator.Configure(log4netConfig);
            }
            else
            {
                throw new FileNotFoundException("File konfigurasi log4net tidak ditemukan.");
            }
        }

        public static void Info(string message)
        {
            Log.Info(message);
        }

        public static void Error(string message)
        {
            Log.Error(message);
        }

        public static void Warning(string message)
        {
            Log.Warn(message);
        }
    }
}