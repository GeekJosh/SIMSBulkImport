/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 
        /// </summary>
        public Options()
        {
            InitializeComponent();
            readConfig();
        }

        private void readConfig()
        {
            this.checkBoxDebug.IsChecked = ConfigMan.IsDebugMode;
            this.checkBoxUpdates.IsChecked = ConfigMan.CheckForUpdates;
            //this.checkBoxDebug.IsEnabled = false;
            this.checkBoxUpdates.IsEnabled = false;
        }

        private bool writeConfig_Updates
        {
            set
            {
                // Reference: Bin-ze Zhao - http://social.msdn.microsoft.com/Forums/da-DK/csharpgeneral/thread/77b87843-ae0b-463d-b50e-b6b8e9175e50
                // Open App.Config of executable
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                // Add an Application Setting.
                config.AppSettings.Settings.Remove("AutoUpdate");
                config.AppSettings.Settings.Add("AutoUpdate", value.ToString());
                try
                {
                    // Save the configuration file.
                    config.Save(ConfigurationSaveMode.Modified);
                }
                catch (Exception writeConfig_UpdatesException)
                {
                    logger.Log(NLog.LogLevel.Error, writeConfig_UpdatesException);
                }
                // Force a reload of a changed section.
                ConfigurationManager.RefreshSection("appSettings");
            }
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            //writeConfig_Updates = this.checkBoxUpdates.IsChecked.Value;
            ConfigMan.SetDebugMode = this.checkBoxDebug.IsChecked.Value;
            Switcher.Switch(new Menu());
        }
    }
}
