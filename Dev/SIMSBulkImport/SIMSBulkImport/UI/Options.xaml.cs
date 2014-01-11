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
            InitializeComponent();
            readConfig();
        }

        private void readConfig()
        {
            checkBoxDebug.IsChecked = ConfigMan.IsDebugMode;
            checkBoxUpdates.IsChecked = ConfigMan.CheckForUpdates;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Menu());
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Menu());
        }
    }
}