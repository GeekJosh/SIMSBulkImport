﻿/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Matt40k.SIMSBulkImport.UI
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Menu()
        {
            InitializeComponent();
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            //this.Close();
            Application.Current.Shutdown();
        }

        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new About());
        }

        private void MenuItem_Click_Options(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Options());
        }

        private void MenuItem_Click_Logs(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\SIMSBulkImport\\Logs";
                prc.Start();
            }
            catch (System.Exception MenuItem_Click_LogsException)
            {
                logger.Log(NLog.LogLevel.Error, MenuItem_Click_LogsException);
            }
        }

        private void MenuItem_Click_Manual(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = "Guide.pdf";
                prc.Start();
            }
            catch (System.Exception MenuItem_Click_Manual_Exception)
            {
                logger.Log(NLog.LogLevel.Error, MenuItem_Click_Manual_Exception);
            }
        }
    }
}
