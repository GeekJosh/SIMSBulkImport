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
    /// <summary>
    /// Class for dealing with the .config.json file - reads and writes.
    /// </summary>
    public class ConfigMan
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private string appConfigDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SIMSBulkImport");

        private bool debugMode;
        private bool updateMode;
        private string appGuid;
        private string updateUrl;

        private int telephonePrimary;
        private int telephoneMain;
        private int emailPrimary;
        private int emailMain;

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

        /// <summary>
        /// Reads the config file (config.json) from the filesystem and loads it into in-memory
        /// </summary>
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
                        updateUrl = item.UpdateURL;
                        emailMain = item.EmailMain;
                        emailPrimary = item.EmailPrimary;
                        telephoneMain = item.TelephoneMain;
                        telephonePrimary = item.TelephonePrimary;
                    }
                }
            }
            else
            {
                CreateConfigFile();
            }
        }

        /// <summary>
        /// Defination of the Configuration file items
        /// </summary>
        public class ConfigItem
        {
            public bool Debug;
            public bool CheckUpdate;
            public string AppGUID;
            public string UpdateURL;
            public int EmailPrimary;
            public int EmailMain;
            public int TelephonePrimary;
            public int TelephoneMain;
        }

        /// <summary>
        /// Create a new config file (.config.json) - new installations
        /// </summary>
        private void CreateConfigFile()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.CreateConfigFile()");
            string newGuid = Guid.NewGuid().ToString();

            debugMode = true;
            updateMode = true;
            appGuid = newGuid;
            updateUrl = "http://api.matt40k.co.uk/";
            emailMain = 0;
            emailPrimary = 0;
            telephoneMain = 0;
            telephonePrimary = 0;

            SaveConfig();         
        }

        public bool CheckForUpdates
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.CheckForUpdates(GET)");
                return updateMode;
            }
        }

        /// <summary>
        /// Reads the unique installation ID
        /// </summary>
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

        public bool IsDebugMode
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.IsDebugMode(GET)");
                return debugMode;
            }
        }

        /// <summary>
        /// Writes the in-memory config to the file system (.config.json)
        /// </summary>
        public void SaveConfig()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SaveConfig()");
            if (File.Exists(GetAppConfig))
                File.Delete(GetAppConfig);

            List<ConfigItem> _data = new List<ConfigItem>();
            _data.Add(new ConfigItem()
            {
                Debug = debugMode
                ,CheckUpdate = updateMode
                ,AppGUID = appGuid
                ,EmailMain = emailMain
                ,EmailPrimary = emailPrimary
                ,TelephoneMain = telephoneMain
                ,TelephonePrimary = telephonePrimary
            });
            string json = JsonConvert.SerializeObject(_data.ToArray());

            File.WriteAllText(GetAppConfig, json); 
        }

        public bool SetDebug
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SetDebug(" + value  +")");
                debugMode = value;
            }
        }

        /// <summary>
        /// Sets the Check for Updates
        /// 
        /// Use SaveConfig() to write to .config.json
        /// </summary>
        public bool SetCheckUpdates
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SetCheckUpdates(" + value + ")");
                updateMode = value;
            }
        }

        /// <summary>
        /// Reads the .config.json file for the default settings for Telephone - Primary
        ///   0-Yes
        ///   1-Yes (Overwrite)
        ///   2-No
        /// </summary>
        public int GetTelephonePrimary
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.GetTelephonePrimary(GET)");
                return telephonePrimary;
            }
        }

        /// <summary>
        /// Sets the Telephone - Primary
        /// 
        /// Use SaveConfig() to write to .config.json
        /// </summary>
        public int SetTelephonePrimary
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SetTelephonePrimary(" + value + ")");
                telephonePrimary = value;
            }
        }

        /// <summary>
        /// Reads the .config.json file for the default settings for Telephone - Main
        ///   0-Yes
        ///   1-Yes (Overwrite)
        ///   2-No
        /// </summary>
        public int GetTelephoneMain
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.GetTelephoneMain(GET)");
                return telephoneMain;
            }
        }

        /// <summary>
        /// Sets the Telephone - Main
        /// 
        /// Use SaveConfig() to write to .config.json
        /// </summary>
        public int SetTelephoneMain
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SetTelephoneMain(" + value + ")");
                telephoneMain = value;
            }
        }

        /// <summary>
        /// Reads the .config.json file for the default settings for Email - Primary
        ///   0-Yes
        ///   1-Yes (Overwrite)
        ///   2-No
        /// </summary>
        public int GetEmailPrimary
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.GetEmailPrimary(GET)");
                return emailPrimary;
            }
        }

        /// <summary>
        /// Sets the Email - Primary
        /// 
        /// Use SaveConfig() to write to .config.json
        /// </summary>
        public int SetEmailPrimary
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SetEmailPrimary(" + value + ")");
                emailPrimary = value;
            }
        }

        /// <summary>
        /// Reads the .config.json file for the default settings for Email - Main
        ///   0-Yes
        ///   1-Yes (Overwrite)
        ///   2-No
        /// </summary>
        public int GetEmailMain
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.GetEmailMain(GET)");
                return emailMain;
            }
        }

        /// <summary>
        /// Sets the Email - Main
        /// 
        /// Use SaveConfig() to write to .config.json
        /// </summary>
        public int SetEmailMain
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SetEmailMain(" + value + ")");
                emailMain = value;
            }
        }
    }
}