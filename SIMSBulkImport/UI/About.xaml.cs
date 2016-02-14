using System;
using System.Diagnostics;
using System.Windows;
using NLog;

namespace SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for About.xaml
    /// </summary>
    public partial class About
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public About()
        {
            InitializeComponent();
            labelTitle.Content = GetExe.Title;
            labelDescription.Content = GetExe.Description;
            labelCopyright.Content = GetExe.Copyright;
            labelVersion.Content = "Version: " + GetExe.Version;
            buttonAppUrl.Content = GetExe.AppUrl;
        }

        private void buttonAppUrl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var process = new Process();
                process.StartInfo.FileName = GetExe.AppUrl;
                process.Start();
            }
            catch (Exception buttonAppUrl_Exception)
            {
                logger.Log(LogLevel.Error, buttonAppUrl_Exception);
            }
        }

        private void buttonClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Menu());
        }
    }
}