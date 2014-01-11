/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Configuration;
using NLog;
using NLog.Config;

namespace Matt40k.SIMSBulkImport
{
    internal class ConfigMan
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static bool SetDebugMode
        {
            set
            {
                try
                {
                    var config = new LoggingConfiguration();
                    config = LogManager.Configuration;
                    foreach (LoggingRule rule in config.LoggingRules)
                    {
                        rule.EnableLoggingForLevel(LogLevel.Debug);
                    }
                    config.Reload();
                    LogManager.Configuration = config;
                }
                catch (Exception EnableDebugException)
                {
                    logger.Log(LogLevel.Error, EnableDebugException);
                }
            }
        }

        public static string UpdateUrl
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.UpdateUrl(GET)");
                string result = readConfig("UpdateURL");
                if (string.IsNullOrEmpty(result))
                {
                    return "http://api.matt40k.co.uk/";
                }
                return result;
            }
        }

        public static bool CheckForUpdates
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.CheckForUpdates(GET)");
                bool result = true;
                string configUpdate = readConfig("AutoUpdate");
                try
                {
                    result = Convert.ToBoolean(configUpdate);
                }
                catch (Exception readConfig_DebugException)
                {
                    logger.Log(LogLevel.Error, "Failed to read AutoUpdate from app.config");
                    logger.Log(LogLevel.Error, readConfig_DebugException);
                }
                return result;
            }
        }

        public static bool IsDebugMode
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.IsDebugMode(GET)");
                return logger.IsDebugEnabled;
            }
        }

        private static string readConfig(string field)
        {
            return ConfigurationManager.AppSettings[field];
        }

        public static bool SetConfig(string field, string value)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SetConfig(field: " + field + ", value: " + value + ")");
            return true;
        }
    }
}