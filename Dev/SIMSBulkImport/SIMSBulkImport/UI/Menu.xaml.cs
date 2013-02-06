/*
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

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Menu()
        {
            InitializeComponent();
            ConnectedTo();
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
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

        private void MenuItem_Click_New_Contact(object sender, RoutedEventArgs e)
        {
            logger.Log(NLog.LogLevel.Debug, "Menu: Contact selected");
            Switcher.PreImportClass.SetUserType = SIMSAPI.UserType.Contact;
            _open();
        }

        private void MenuItem_Click_New_Pupil(object sender, RoutedEventArgs e)
        {
            logger.Log(NLog.LogLevel.Debug, "Menu: Pupil selected");
            Switcher.PreImportClass.SetUserType = SIMSAPI.UserType.Pupil;
            _open();
        }

        private void MenuItem_Click_New_Staff(object sender, RoutedEventArgs e)
        {
            logger.Log(NLog.LogLevel.Debug, "Menu: Staff selected");
            Switcher.PreImportClass.SetUserType = Switcher.SimsApiClass.user
                //SIMSAPI.UserType.Staff;
            _open();
        }

        private void _open()
        {
            Switcher.ImportFileClass = new ImportFile();
            Switcher.Switch(new Open());
        }

        public void ConnectedTo()
        {
            string currentSchool = Switcher.SimsApiClass.GetCurrentSchool;
            string currentUser = Switcher.SimsApiClass.GetCurrentUser;
            if (!string.IsNullOrWhiteSpace(currentSchool))
                this.LabelSchool.Content = currentSchool;
            if (!string.IsNullOrWhiteSpace(currentUser))
                this.LabelUser.Content = currentUser;
        }
    }
}
