using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;
using UserGen;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public Menu()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Menu()");

            InitializeComponent();
            
            ConnectedTo();
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Menu - MenuItem_Click_Exit");
            Application.Current.Shutdown();
        }

        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Menu - MenuItem_Click_About");
            Switcher.Switch(new About());
        }

        private void MenuItem_Click_Options(object sender, RoutedEventArgs e)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Menu - MenuItem_Click_Options");
            Switcher.Switch(new Options());
        }

        private void MenuItem_Click_Logs(object sender, RoutedEventArgs e)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Menu - MenuItem_Click_Logs");
            Switcher.Switch(new Logs());
        }

        private void MenuItem_Click_Manual(object sender, RoutedEventArgs e)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Menu - MenuItem_Click_Manual");
            try
            {
                var prc = new Process();
                prc.StartInfo.FileName = "Guide.pdf";
                prc.Start();
            }
            catch (Exception MenuItem_Click_Manual_Exception)
            {
                logger.Log(LogLevel.Error, MenuItem_Click_Manual_Exception);
            }
        }

        private void MenuItem_Click_New_Contact(object sender, RoutedEventArgs e)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Menu - MenuItem_Click_New_Contact");
            MenuClick(Interfaces.UserType.Contact);
        }

        private void MenuItem_Click_New_Pupil(object sender, RoutedEventArgs e)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Menu - MenuItem_Click_New_Pupil");
            MenuClick(Interfaces.UserType.Pupil);
        }

        private void MenuItem_Click_New_Staff(object sender, RoutedEventArgs e)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Menu - MenuItem_Click_New_Staff");
            MenuClick(Interfaces.UserType.Staff);
        }

        private void MenuItem_Click_User_Pupil(object sender, RoutedEventArgs e)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Menu - MenuItem_Click_User_Pupil");
            Switcher.SimsApiClass.SetUserType = Interfaces.UserType.Pupil;
            Switcher.UserGenClass = new Builder();
            Switcher.Switch(new UserUdf());
        }

        private void MenuClick(Interfaces.UserType userType)
        {
            logger.Log(LogLevel.Debug, "Menu: " + userType);
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
                LabelSchool.Content = currentSchool;
            if (!string.IsNullOrWhiteSpace(currentUser))
                LabelUser.Content = currentUser;
            ImageSchoolLogo.Source = currentSchoolLogo;
        }
    }
}