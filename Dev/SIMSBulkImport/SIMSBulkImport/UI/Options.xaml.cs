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

        private void readConfig()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Options.readConfig()");
            checkBoxDebug.IsChecked = Switcher.ConfigManClass.IsDebugMode;
            checkBoxUpdates.IsChecked = Switcher.ConfigManClass.CheckForUpdates;
        }

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

            Switcher.ConfigManClass.SetDebug = (bool)debugCheckBox;
            Switcher.ConfigManClass.SetCheckUpdates = (bool)updateCheckBox;

            Switcher.ConfigManClass.SaveConfig();

            Switcher.Switch(new Menu());
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Options.buttonCancel_Click()");
            Switcher.Switch(new Menu());
        }
    }
}