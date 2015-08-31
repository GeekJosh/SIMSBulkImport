using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Windows;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;

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

        private string emailMain;
        private string emailPrimary;
        private string emailLocation;
        private string emailNotes;

        private string telephoneMain;
        private string telephonePrimary;
        private string telephoneLocation;
        private string telephoneNotes;
        private string telephoneDevice;

        private string theme;
        private string accent;

        public ConfigMan()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan()");
            ReadConfig();
            SetDebug = IsDebugMode;
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
                    dynamic result = JsonConvert.DeserializeObject(r.ReadToEnd());
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
                        try
                        {
                            theme = item.Theme;
                            accent = item.Accent;
                        }
                        catch (Exception)
                        {
                        }
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
            public string EmailPrimary;
            public string EmailMain;
            public string EmailLocation;
            public string EmailNotes;
            public string TelephonePrimary;
            public string TelephoneMain;
            public string TelephoneLocation;
            public string TelephoneNotes;
            public string TelephoneDevice;
            public string Theme;
            public string Accent;
        }

        /// <summary>
        /// Create a new config file (.config.json) - new installations
        /// </summary>
        private void CreateConfigFile()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.CreateConfigFile()");

            // Generate a new GUID
            string newGuid = Guid.NewGuid().ToString();

            debugMode = false;
            updateMode = true;
            appGuid = newGuid;
            updateUrl = "http://simsbulkimport.uk/";

            emailMain = "Yes";
            emailPrimary = "Yes";
            telephoneMain = "Yes";
            telephonePrimary = "Yes";

            /*
            // These shouldn't be hardcoded
            emailLocation = null;
            emailNotes = null;
            telephoneLocation = null;
            telephoneNotes = null;
            telephoneDevice = null;
            */

            // Save the new config to the file system
            SaveConfig();         
        }

        /// <summary>
        /// Returns TRUE if it is set to check for updates using the online services
        /// </summary>
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
        
        /// <summary>
        /// Returns the URL to be used for checking for updates
        /// </summary>
        public string GetUpdateURL
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.GetUpdateURL(GET)");
                return updateUrl;
            }
        }

        /// <summary>
        /// Returns true if it is set to Debug mode
        /// </summary>
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
                ,EmailLocation = emailLocation
                ,EmailNotes = emailNotes
                ,TelephoneMain = telephoneMain
                ,TelephonePrimary = telephonePrimary
                ,TelephoneLocation = telephoneLocation
                ,TelephoneNotes = telephoneNotes
                ,TelephoneDevice = telephoneDevice
                ,Theme = theme
                ,Accent = accent
            });
            string json = JsonConvert.SerializeObject(_data.ToArray());

            File.WriteAllText(GetAppConfig, json); 
        }

        /// <summary>
        /// Set if the application should run in debug mode, changes the log level to trace if TRUE
        /// </summary>
        public bool SetDebug
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SetDebug(" + value  +")");
                debugMode = value;
                if (value)
                    SetLogToDebug();
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

        public string SetTheme
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SetTheme(" + value + ")");
                theme = value;
            }
        }

        public string SetAccent
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SetAccent(" + value + ")");
                accent = value;
            }
        }

        public string GetTheme
        {
            get
            {
                if (string.IsNullOrEmpty((theme)))
                    return "BaseLight";
                return theme;
            }
        }

        public string GetAccent
        {
            get
            {
                if (string.IsNullOrEmpty(accent))
                    return "Blue";
                return accent;
            }
        }

#region Email
        #region Email Primary
        /// <summary>
        /// Sets the Email - Primary
        /// 
        /// Use SaveConfig() to write to .config.json
        /// </summary>
        public string SetDefaultEmailPrimary
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SetEmailPrimary(" + value + ")");
                emailPrimary = value;
            }
        }

        /// <summary>
        /// Reads the .config.json file for the default settings for Email - Primary
        /// </summary>
        public string GetDefaultEmailPrimary
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.GetEmailPrimary(GET)");
                return emailPrimary;
            }
        }
        #endregion

        #region Email Main
        /// <summary>
        /// Sets the Email - Main
        /// 
        /// Use SaveConfig() to write to .config.json
        /// </summary>
        public string SetDefaultEmailMain
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SetEmailMain(" + value + ")");
                emailMain = value;
            }
        }

        /// <summary>
        /// Reads the .config.json file for the default settings for Email - Main
        /// </summary>
        public string GetDefaultEmailMain
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.GetEmailMain(GET)");
                return emailMain;
            }
        }
        #endregion

        #region Email Location
        /// <summary>
        /// Sets the Email - Location
        /// 
        /// Use SaveConfig() to write to .config.json
        /// </summary>
        public string SetDefaultEmailLocation
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SetDefaultEmailLocation(" + value + ")");
                emailLocation = value;
            }
        }

        /// <summary>
        /// Reads the .config.json file for the default settings for Email - Location
        /// </summary>
        public string GetDefaultEmailLocation
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.GetDefaultEmailLocation(GET)");
                if (!string.IsNullOrWhiteSpace(emailLocation))
                    return emailLocation;
                else
                {
                    try
                    {
                        return Switcher.SimsApiClass.GetDefaultEmailLocation;
                    }
                    catch (Exception GetDefaultEmailLocation_GetDefault_Exception)
                    {
                        logger.Log(LogLevel.Error, "Matt40k.SIMSBulkImport.ConfigMan.GetDefaultEmailLocation_GetDefault_Exception");
                        logger.Log(LogLevel.Error, GetDefaultEmailLocation_GetDefault_Exception);
                    }
                    return null;
                }
            }
        }
        #endregion

        #region Email Notes
        /// <summary>
        /// Sets the Email - Notes
        /// 
        /// Use SaveConfig() to write to .config.json
        /// </summary>
        public string SetDefaultEmailNotes
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SetDefaultEmailNotes(" + value + ")");
                emailNotes = value;
            }
        }

        /// <summary>
        /// Reads the .config.json file for the default settings for Email - Notes
        /// </summary>
        public string GetDefaultEmailNotes
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.GetDefaultEmailNotes(GET)");
                return emailNotes;
            }
        }
        #endregion
#endregion Email

#region Telephone
        #region Telephone Primary
        /// <summary>
        /// Sets the Telephone - Primary
        /// 
        /// Use SaveConfig() to write to .config.json
        /// </summary>
        public string SetDefaultTelephonePrimary
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SetTelephonePrimary(" + value + ")");
                telephonePrimary = value;
            }
        }

        /// <summary>
        /// Reads the .config.json file for the default settings for Telephone - Primary
        /// </summary>
        public string GetDefaultTelephonePrimary
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.GetTelephonePrimary(GET)");
                return telephonePrimary;
            }
        }
        #endregion

        #region Telephone Main
        /// <summary>
        /// Sets the default Telephone - Main
        /// 
        /// Use SaveConfig() to write to .config.json
        /// </summary>
        public string SetDefaultTelephoneMain
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SetDefaultTelephoneMain(" + value + ")");
                telephoneMain = value;
            }
        }

        /// <summary>
        /// Reads the .config.json file for the default settings for Telephone - Main
        /// </summary>
        public string GetDefaultTelephoneMain
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.GetTelephoneMain(GET)");
                return telephoneMain;
            }
        }
        #endregion

        #region Telephone Location
        /// <summary>
        /// Sets the Telephone - Location
        /// 
        /// Use SaveConfig() to write to .config.json
        /// </summary>
        public string SetDefaultTelephoneLocation
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SetTelephoneLocation(" + value + ")");
                telephoneLocation = value;
            }
        }

        /// <summary>
        /// Reads the .config.json file for the default settings for Telephone - Location
        /// </summary>
        public string GetDefaultTelephoneLocation
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.GetTelephoneLocation(GET)");
                if (!string.IsNullOrWhiteSpace(telephoneLocation))
                return telephoneLocation;
                else
                {
                    try
                    {
                        return Switcher.SimsApiClass.GetDefaultTelephoneLocation;
                    }
                    catch (Exception GetDefaultTelephoneLocation_GetDefault_Exception)
                    {
                        logger.Log(LogLevel.Error, "Matt40k.SIMSBulkImport.ConfigMan.GetDefaultTelephoneLocation_GetDefault_Exception");
                        logger.Log(LogLevel.Error, GetDefaultTelephoneLocation_GetDefault_Exception);
                    }
                    return null;
                }
            }
        }
        #endregion

        #region Telephone Notes
        /// <summary>
        /// Sets the Telephone - Notes
        /// 
        /// Use SaveConfig() to write to .config.json
        /// </summary>
        public string SetDefaultTelephoneNotes
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SetTelephoneNotes(" + value + ")");
                telephoneNotes = value;
            }
        }

        /// <summary>
        /// Reads the .config.json file for the default settings for Telephone - Notes
        /// </summary>
        public string GetDefaultTelephoneNotes
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.GetTelephoneNotes(GET)");
                return telephoneNotes;
            }
        }
        #endregion

        #region Telephone Device
        /// <summary>
        /// Sets the Telephone - Notes
        /// 
        /// Use SaveConfig() to write to .config.json
        /// </summary>
        public string SetDefaultTelephoneDevice
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.SetTelephoneDevice(" + value + ")");
                telephoneDevice = value;
            }
        }

        /// <summary>
        /// Reads the .config.json file for the default settings for Telephone - Device
        /// </summary>
        public string GetDefaultTelephoneDevice
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ConfigMan.GetTelephoneDevice(GET)");
                if (!string.IsNullOrWhiteSpace(telephoneDevice))
                return telephoneDevice;
                else
                {
                    try
                    {
                        return Switcher.SimsApiClass.GetDefaultTelephoneDevice;
                    }
                    catch (Exception GetDefaultTelephoneDevice_GetDefault_Exception)
                    {
                        logger.Log(LogLevel.Error, "Matt40k.SIMSBulkImport.ConfigMan.GetDefaultTelephoneDevice_GetDefault_Exception");
                        logger.Log(LogLevel.Error, GetDefaultTelephoneDevice_GetDefault_Exception);
                    }
                    return null;
                }
            }
        }
        #endregion
#endregion


        /// <summary>
        /// 
        /// </summary>
        public void SetLogToDebug()
        {
            foreach (var rule in LogManager.Configuration.LoggingRules)
            {
                rule.EnableLoggingForLevel(LogLevel.Trace);
            }

            LogManager.ReconfigExistingLoggers();
        }
    }
}