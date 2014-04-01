/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System.Windows;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    /// Interaction logic for Default.xaml
    /// </summary>
    public partial class Default
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 
        /// </summary>
        public Default()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Default()");
            InitializeComponent();
            Load();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Load()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Load()");
            SetTitle();
            GetEmailLocations();
            GetTelephoneLocations();
            GetTelephoneDevices();
            ReadConfig();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReadConfig()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Default.ReadConfig()");

            comboEmailMain.SelectedValue = Switcher.ConfigManClass.GetEmailMain;
            comboEmailPrimary.SelectedValue = Switcher.ConfigManClass.GetEmailPrimary;
            comboTelephoneMain.SelectedValue = Switcher.ConfigManClass.GetTelephoneMain;
            comboTelephonePrimary.SelectedValue = Switcher.ConfigManClass.GetTelephonePrimary;
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetEmailLocations()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Options.GetEmailLocations()");
            // Get Email Locations
            string[] emailLocations = Switcher.SimsApiClass.GetEmailLocations;
            if (emailLocations.Length != 0)
            {
                foreach (string emailLocation in emailLocations)
                {
                    comboEmailLocation.Items.Add(emailLocation);
                }
                comboEmailLocation.Items.Add("");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetTelephoneLocations()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Options.GetTelephoneLocations(GET)");
            // Get Telephone Locations
            string[] telephoneLocations = Switcher.SimsApiClass.GetTelephoneLocations;
            if (telephoneLocations.Length != 0)
            {
                foreach (string telephoneLocation in telephoneLocations)
                {
                    comboTelephoneLocation.Items.Add(telephoneLocation);
                }
                comboTelephoneLocation.Items.Add("");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetTelephoneDevices()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Options.GetTelephoneDevices(GET)");
            // Get Telephone Devices
            string[] telephoneDevices = Switcher.SimsApiClass.GetTelephoneDevices;
            if (telephoneDevices.Length != 0)
            {
                foreach (string device in telephoneDevices)
                {
                    comboTelephoneDevice.Items.Add(device);
                }
                comboTelephoneDevice.Items.Add("");
            }
        }

        /// <summary>
        /// Saves the config to the file system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            Switcher.ConfigManClass.SetEmailMain = comboEmailMain.SelectedIndex;
            Switcher.ConfigManClass.SetEmailPrimary = comboEmailPrimary.SelectedIndex;
            Switcher.ConfigManClass.SetTelephoneMain = comboTelephoneMain.SelectedIndex;
            Switcher.ConfigManClass.SetTelephonePrimary = comboTelephonePrimary.SelectedIndex;

            // Write the config file (.config.json) to the file-system
            Switcher.ConfigManClass.SaveConfig();

            // Return to Match UI
            Switcher.Switch(new Match());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Options.buttonCancel_Click()");

            // Return to Match UI
            Switcher.Switch(new Match());
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetTitle()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Options.SetTitle()");
            switch (Switcher.PreImportClass.GetUserType)
            {
                case Interfaces.UserType.Staff:
                    labelTitle.Content = "Default - Staff";
                    break;
                case Interfaces.UserType.Pupil:
                    labelTitle.Content = "Default - Pupil";
                    break;
                case Interfaces.UserType.Contact:
                    labelTitle.Content = "Default - Contact";
                    break;
            }
        }
    }
}
