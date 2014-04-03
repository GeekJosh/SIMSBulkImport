/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System.Windows;
using System.Windows.Controls;
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
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Default.Load()");
            SetSubTitle();
            GetEmailLocations();
            GetTelephoneLocations();
            GetTelephoneDevices();
            ReadConfig();
        }

        /// <summary>
        /// Reads the Default settings and sets them as the selected in the UI
        /// </summary>
        private void ReadConfig()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Default.ReadConfig()");

            comboEmailMain.SelectedValue = Switcher.ConfigManClass.GetDefaultEmailMain;
            comboEmailPrimary.SelectedValue = Switcher.ConfigManClass.GetDefaultEmailPrimary;
            comboEmailLocation.SelectedValue = Switcher.ConfigManClass.GetDefaultEmailLocation;
            comboEmailNotes.Text = Switcher.ConfigManClass.GetDefaultEmailNotes;

            comboTelephoneMain.SelectedValue = Switcher.ConfigManClass.GetDefaultTelephoneMain;
            comboTelephonePrimary.SelectedValue = Switcher.ConfigManClass.GetDefaultTelephonePrimary;
            comboTelephoneLocation.SelectedValue = Switcher.ConfigManClass.GetDefaultTelephoneLocation;
            comboTelephoneNotes.Text = Switcher.ConfigManClass.GetDefaultTelephoneNotes;
            comboTelephoneDevice.SelectedValue = Switcher.ConfigManClass.GetDefaultTelephoneDevice;
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetEmailLocations()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Default.GetEmailLocations()");
            // Get Email Locations
            string[] emailLocations = Switcher.SimsApiClass.GetEmailLocations;
            if (emailLocations.Length != 0)
            {
                foreach (string emailLocation in emailLocations)
                {
                    comboEmailLocation.Items.Add(emailLocation);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetTelephoneLocations()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Default.GetTelephoneLocations(GET)");
            // Get Telephone Locations
            string[] telephoneLocations = Switcher.SimsApiClass.GetTelephoneLocations;
            if (telephoneLocations.Length != 0)
            {
                foreach (string telephoneLocation in telephoneLocations)
                {
                    comboTelephoneLocation.Items.Add(telephoneLocation);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetTelephoneDevices()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Default.GetTelephoneDevices(GET)");
            // Get Telephone Devices
            string[] telephoneDevices = Switcher.SimsApiClass.GetTelephoneDevices;
            if (telephoneDevices.Length != 0)
            {
                foreach (string device in telephoneDevices)
                {
                    comboTelephoneDevice.Items.Add(device);
                }
            }
        }

        /// <summary>
        /// Saves the config to the file system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Default.buttonSave_Click()");

            ComboBoxItem emailMainItem = (ComboBoxItem)comboEmailMain.SelectedValue;
            ComboBoxItem emailPrimaryItem = (ComboBoxItem)comboEmailPrimary.SelectedValue;
            ComboBoxItem telephoneMainItem = (ComboBoxItem)comboTelephoneMain.SelectedValue;
            ComboBoxItem telephonePrimaryItem = (ComboBoxItem)comboTelephonePrimary.SelectedValue;

            Switcher.ConfigManClass.SetDefaultEmailMain = (string)emailMainItem.Content;
            Switcher.ConfigManClass.SetDefaultEmailPrimary = (string)emailPrimaryItem.Content;
            Switcher.ConfigManClass.SetDefaultEmailLocation = comboEmailLocation.SelectedValue.ToString();
            Switcher.ConfigManClass.SetDefaultEmailNotes = comboEmailNotes.Text;

            Switcher.ConfigManClass.SetDefaultTelephoneMain = (string)telephoneMainItem.Content;
            Switcher.ConfigManClass.SetDefaultTelephonePrimary = (string)telephonePrimaryItem.Content;
            Switcher.ConfigManClass.SetDefaultTelephoneLocation = comboTelephoneLocation.SelectedValue.ToString();
            Switcher.ConfigManClass.SetDefaultTelephoneNotes = comboTelephoneNotes.Text;
            Switcher.ConfigManClass.SetDefaultTelephoneDevice = comboTelephoneDevice.SelectedValue.ToString();

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
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Default.buttonCancel_Click()");

            // Return to Match UI
            Switcher.Switch(new Match());
        }

        /// <summary>
        /// Set the UI subtitle
        /// </summary>
        private void SetSubTitle()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Default.SetTitle()");
            switch (Switcher.PreImportClass.GetUserType)
            {
                case Interfaces.UserType.Staff:
                    labelSubTitle.Content = "Default - Staff";
                    break;
                case Interfaces.UserType.Pupil:
                    labelSubTitle.Content = "Default - Pupil";
                    break;
                case Interfaces.UserType.Contact:
                    labelSubTitle.Content = "Default - Contact";
                    break;
            }
        }
    }
}
