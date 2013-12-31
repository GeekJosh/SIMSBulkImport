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
