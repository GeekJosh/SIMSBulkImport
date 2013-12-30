/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
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
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Menu()");
            InitializeComponent();
            ConnectedTo();
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Menu - MenuItem_Click_Exit");
            Application.Current.Shutdown();
        }

        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Menu - MenuItem_Click_About");
            Switcher.Switch(new About());
        }

        private void MenuItem_Click_Options(object sender, RoutedEventArgs e)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Menu - MenuItem_Click_Options");
            Switcher.Switch(new Options());
        }

        private void MenuItem_Click_Logs(object sender, RoutedEventArgs e)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Menu - MenuItem_Click_Logs");
            Switcher.Switch(new Logs());
        }

        private void MenuItem_Click_Manual(object sender, RoutedEventArgs e)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Menu - MenuItem_Click_Manual");
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
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Menu - MenuItem_Click_New_Contact");
            MenuClick(Interfaces.UserType.Contact);
        }

        private void MenuItem_Click_New_Pupil(object sender, RoutedEventArgs e)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Menu - MenuItem_Click_New_Pupil");
            MenuClick(Interfaces.UserType.Pupil);
        }

        private void MenuItem_Click_New_Staff(object sender, RoutedEventArgs e)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Menu - MenuItem_Click_New_Staff");
            MenuClick(Interfaces.UserType.Staff);
        }

        private void MenuClick(Interfaces.UserType userType)
        {
            logger.Log(NLog.LogLevel.Debug, "Menu: " + userType);
            Switcher.PreImportClass = null;
            Switcher.PreImportClass = new PreImport();
            Switcher.PreImportClass.SetUserType = userType;
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
            BitmapImage currentSchoolLogo = Switcher.SimsApiClass.GetCurrentSchoolLogo;
            if (!string.IsNullOrWhiteSpace(currentSchool))
                this.LabelSchool.Content = currentSchool;
            if (!string.IsNullOrWhiteSpace(currentUser))
                this.LabelUser.Content = currentUser;
            this.ImageSchoolLogo.Source = currentSchoolLogo;
        }
    }
}
