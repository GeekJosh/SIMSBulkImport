/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System.Windows;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for Options.xaml
    /// </summary>
    public partial class Options
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// </summary>
        public Options()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Options()");
            InitializeComponent();
            readConfig();
        }

        /// <summary>
        /// Reads the config file (.config.json) then sets the UI
        /// </summary>
        private void readConfig()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Options.readConfig()");
            checkBoxDebug.IsChecked = Switcher.ConfigManClass.IsDebugMode;
            checkBoxUpdates.IsChecked = Switcher.ConfigManClass.CheckForUpdates;
            comboEmailMain.SelectedIndex = Switcher.ConfigManClass.GetEmailMain;
            comboEmailPrimary.SelectedIndex = Switcher.ConfigManClass.GetEmailPrimary;
            comboTelephoneMain.SelectedIndex = Switcher.ConfigManClass.GetTelephoneMain;
            comboTelephonePrimary.SelectedIndex = Switcher.ConfigManClass.GetTelephonePrimary;
        }

        /// <summary>
        /// Saves the config to the file system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Options.buttonSave_Click()");
            bool? debugCheckBox = checkBoxDebug.IsChecked;
            if (!debugCheckBox.HasValue)
            {
                debugCheckBox = true;
            }

            bool? updateCheckBox = checkBoxUpdates.IsChecked;
            if (!updateCheckBox.HasValue)
            {
                updateCheckBox = true;
            }

            // Set the in-memory config
            Switcher.ConfigManClass.SetDebug = (bool)debugCheckBox;
            Switcher.ConfigManClass.SetCheckUpdates = (bool)updateCheckBox;
            Switcher.ConfigManClass.SetEmailMain = comboEmailMain.SelectedIndex;
            Switcher.ConfigManClass.SetEmailPrimary = comboEmailPrimary.SelectedIndex;
            Switcher.ConfigManClass.SetTelephoneMain = comboTelephoneMain.SelectedIndex;
            Switcher.ConfigManClass.SetTelephonePrimary = comboTelephonePrimary.SelectedIndex;

            // Write the config file (.config.json) to the file-system
            Switcher.ConfigManClass.SaveConfig();

            Switcher.Switch(new Menu());
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Options.buttonCancel_Click()");
            Switcher.Switch(new Menu());
        }

        /// <summary>
        /// Translates from the Main\Primary ID to the friendly name
        ///   0-Yes
        ///   1-Yes (Overwrite)
        ///   2-No
        /// </summary>
        /// <param name="AsInt"></param>
        /// <returns></returns>
        private string PrimaryMainFromIntToString(int AsInt)
        {
            switch(AsInt)
            {
                case 0:
                    return "Yes";
                case 1:
                    return "Yes (Overwrite)";
                case 2:
                    return "No";
                default:
                    return "Yes";
            }
        }
    }
}