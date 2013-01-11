/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

using NLog;
using NLog.Config;

namespace Matt40k.SIMSBulkImport
{
    class ConfigMan
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static string readConfig(string field)
        {
            return ConfigurationManager.AppSettings[field];
        }

        public static bool SetDebugMode
        {
            set
            {
                try
                {
                    LoggingConfiguration config = new LoggingConfiguration();
                    config = LogManager.Configuration;
                    foreach (NLog.Config.LoggingRule rule in config.LoggingRules)
                    {
                        rule.EnableLoggingForLevel(NLog.LogLevel.Debug);
                    }
                    config.Reload();
                    LogManager.Configuration = config;
                }
                catch (Exception EnableDebugException)
                {
                    logger.Log(NLog.LogLevel.Error, EnableDebugException);    
                }
            }
        }

        public static string UpdateUrl
        {
            get
            {
                string result = readConfig("UpdateURL");
                /*
                if (string.IsNullOrEmpty(result))
                {
                    return "http://matt40k.co.uk/apps/simsbulkimport/";
                }*/
                return result;
            }
        }

        public static bool CheckForUpdates
        {
            get
            {
                bool result = true;
                string configUpdate = readConfig("AutoUpdate");
                try
                {
                    result = Convert.ToBoolean(configUpdate);
                }
                catch (Exception readConfig_DebugException)
                {
                    logger.Log(NLog.LogLevel.Error, "Failed to read AutoUpdate from app.config");
                    logger.Log(NLog.LogLevel.Error, readConfig_DebugException);
                }
                return result;
            }
        }

        public static bool IsDebugMode
        {
            get
            {
                return logger.IsDebugEnabled;
            }
        }

        public static bool SetConfig(string field, string value)
        {
            // Reference: Bin-ze Zhao - http://social.msdn.microsoft.com/Forums/da-DK/csharpgeneral/thread/77b87843-ae0b-463d-b50e-b6b8e9175e50
            // Open App.Config of executable
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // Add an Application Setting.
            config.AppSettings.Settings.Remove(field);
            config.AppSettings.Settings.Add(field, value);
            try
            {
                // Save the configuration file.
                config.Save(ConfigurationSaveMode.Modified);
            }
            catch (Exception SetKeyException)
            {
                string tmpFile = Path.GetTempFileName();
                File.Delete(tmpFile);
                config.SaveAs(tmpFile);
                bool result = CopyMaster(tmpFile);
                if (!result) { return false; }
            }
            // Force a reload of a changed section.
            ConfigurationManager.RefreshSection("appSettings");
            return true;
        }

        private static bool CopyMaster(string fromFile)
        {
            try
            {
                string toFile = Path.Combine(GetExe.FilePath, (GetExe.FileName + ".config"));
                string updateExe = Path.Combine(GetExe.FilePath, "UpdateConfig.exe");
                logger.Log(NLog.LogLevel.Error, fromFile + " " + toFile);
                Process process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = updateExe;
                process.StartInfo.Arguments = "\"" + fromFile + "\" \"" + toFile + "\"";
                process.StartInfo.Verb = "runas";
                process.EnableRaisingEvents = true;
                process.Start();
                process.WaitForExit();
                int result = process.ExitCode;
                if (result == 0)
                {
                    return true;
                }
            }
            catch (Exception CopyMasterException)
            {
                logger.Log(NLog.LogLevel.Error, CopyMasterException);
            }
            return false;
        }
    }
}
