using System;
using NLog;

namespace CachingDemo
{
    public static class Log
    {
        private static readonly Logger nLogger = NLog.LogManager.GetCurrentClassLogger();

        public static void Fatal(Exception e)
        {
            nLogger.Fatal(e);
        }

        public static void Fatal(string logMessage)
        {
            nLogger.Fatal(logMessage);
        }

        public static void Error(Exception e)
        {
            nLogger.Error(e);
        }

        public static void Error(string logMessage)
        {
            nLogger.Error(logMessage);
        }

        public static void Warn(Exception e)
        {
            nLogger.Warn(e);
        }

        public static void Warn(string logMessage)
        {
            nLogger.Warn(logMessage);
        }

        public static void Debug(Exception e)
        {
            nLogger.Debug(e);
        }

        public static void Debug(string logMessage)
        {
            nLogger.Debug(logMessage);
        }

        public static void Info(Exception e)
        {
            nLogger.Info(e);
        }

        public static void Info(string logMessage)
        {
            nLogger.Info(logMessage);
        }
    }
}
