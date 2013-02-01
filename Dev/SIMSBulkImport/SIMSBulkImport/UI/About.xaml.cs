/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using NLog;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public About()
        {
            InitializeComponent();
            this.labelTitle.Content = GetExe.Title;
            this.labelDescription.Content = GetExe.Description;
            this.labelCopyright.Content = GetExe.Copyright;
            this.labelVersion.Content = "Version: " + GetExe.Version;
            this.buttonAppUrl.Content = GetExe.AppUrl;
  
        }

        private void buttonAppUrl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = GetExe.AppUrl;
                process.Start();
            }
            catch (Exception buttonAppUrl_Exception)
            {
                logger.Log(NLog.LogLevel.Error, buttonAppUrl_Exception);
            }
        }

        private void buttonClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainWindow());
        }
    }
}
