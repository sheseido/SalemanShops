using log4net;
using System;
using System.IO;

namespace filter.framework.utility
{
    /// <summary>
    /// 日志管理
    /// </summary>
    public sealed class LogHelper
    {
        private ILog mLog;

        public void SetLog(string loggerName = "", bool forceSet = false)
        {
            if (forceSet || mLog == null)
            {
                if (string.IsNullOrWhiteSpace(loggerName))
                {
                    mLog = LogManager.GetLogger("Default");
                }
                else
                {
                    mLog = LogManager.GetLogger(loggerName);
                }
            }
        }

        public void Warn(string message)
        {
            SetLog();
            mLog.Warn(message);
        }

        public void Debug(string message)
        {
            SetLog();
            mLog.Debug(message);
        }

        public void Info(string message)
        {
            SetLog();
            mLog.Info(message);
        }

        public void Fatal(string message, Exception ex = null)
        {
            SetLog();
            mLog.Fatal(message, ex);
        }

        public void Error(string message, Exception ex = null)
        {
            SetLog();
            mLog.Error(message, ex);
        }

        public void InitLog4Net()
        {
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(logCfg);
        }
    }
}
