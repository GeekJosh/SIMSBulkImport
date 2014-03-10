/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NLog;
using NLog.Config;

namespace Matt40k.SIMSBulkImport
{
    public class ConfigMan
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private string appConfigDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SIMSBulkImport");

        private bool debugMode;
        private bool updateMode;
        private string appGuid;
        private string updateUrl;

        public ConfigMan()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan()");
            ReadConfig();
        }

        private string GetAppConfig
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.GetAppConfig(GET)");
                return Path.Combine(appConfigDir, "SIMSBulkImport.Config.json");
            }
        }

        private void ReadConfig()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.ReadConfig()");
            if (File.Exists(GetAppConfig))
            {
                using (StreamReader r = new StreamReader(GetAppConfig))
                {
                    dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(r.ReadToEnd());
                    foreach (var item in result)
                    {
                        debugMode = item.Debug;
                        updateMode = item.CheckUpdate;
                        appGuid = item.AppGUID;
                    }
                }
            }
            else
            {
                CreateConfigFile();
            }
        }

        public class ConfigItem
        {
            public bool Debug;
            public bool CheckUpdate;
            public string AppGUID;
            public string UpdateURL;
        }

        private void CreateConfigFile()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.CreateConfigFile()");
            string newGuid = Guid.NewGuid().ToString();

            debugMode = true;
            updateMode = true;
            appGuid = newGuid;
            updateUrl = "http://api.matt40k.co.uk/";

            List<ConfigItem> _data = new List<ConfigItem>();
            _data.Add(new ConfigItem()
            {
                Debug = debugMode,
                CheckUpdate = updateMode,
                AppGUID = appGuid
            });
            string json = JsonConvert.SerializeObject(_data.ToArray());

            File.WriteAllText(GetAppConfig, json);           
        }

        public bool CheckForUpdates
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.CheckForUpdates(GET)");
                return updateMode;
            }
        }

        public string GetAppGUID
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.GetAppGUID(GET)");
                return appGuid;
            }
        }
        

        public string GetUpdateURL
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.GetUpdateURL(GET)");
                return updateUrl;
            }
        }

        /*
        public static bool SetDebugMode
        {
            set
            {
                try
                {
                    ReadConfig();
                    Configuration config; 
                    //= ConfigurationManager.
                    //    (GetAppConfig);
                    config = ConfigurationManager.OpenExeConfiguration(GetAppConfig);
                    config.AppSettings.Settings.Add("YourKey", "YourValue");
                    config.Save(ConfigurationSaveMode.Minimal);
                }
                catch (Exception EnableDebugException)
                {
                    logger.Log(LogLevel.Error, EnableDebugException);
                }
                
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
        */

        public string UpdateUrl
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.UpdateUrl(GET)");
                string result = GetUpdateURL;
                if (string.IsNullOrEmpty(result))
                {
                    return "http://api.matt40k.co.uk/";
                }
                return result;
            }
        }

        /*
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
*/
    }
}