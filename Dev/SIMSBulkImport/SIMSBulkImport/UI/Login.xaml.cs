/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private BackgroundWorker bw = new BackgroundWorker();
        private bool IsConnected;
        private string tmpUsr;

        public Login()
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Login()");
            InitializeComponent();
            this.textUser.Focus();
            BeginLoad();
        }

        private void updateLoadMess(string message)
        {
            logger.Log(NLog.LogLevel.Debug, message);
            this.loadMessage.Content = message;
        }

        private void BeginLoad()
        {
            logger.Log(NLog.LogLevel.Info, "==============================================================================================");
            logger.Log(NLog.LogLevel.Info, "==============================================================================================");
            logger.Log(NLog.LogLevel.Info, "");
            logger.Log(NLog.LogLevel.Info, GetExe.Title + " - " + GetExe.Version);
            logger.Log(NLog.LogLevel.Info, "");

            // Clear previous temp files we created
            updateLoadMess("Clearing up");
            ClearUp.ClearTmp();

            // Check for updates
            if (ConfigMan.CheckForUpdates)
            {
                updateLoadMess("Checking for updates");
                Updater.Update.Check();
            }

            // Read SIMS.ini
            updateLoadMess("Reading SIMS.ini");
            string simsDir = SimsIni.GetSimsDir;

            // Loading SIMS API
            updateLoadMess("Loading SIMS API");
            Switcher.SimsApiClass = new SIMSAPI(simsDir);

            ShowLogon(true);
            
        }

        private void ShowLogon(bool value)
        {
            if (value)
            {
                // Hide the other bits
                this.progressRing.IsActive = false;
                this.loadLabel.Visibility = Visibility.Hidden;
                this.loadMessage.Visibility = Visibility.Hidden;

                // Enable logon
                this.labelTitle.Visibility = Visibility.Visible;
                this.grid.Visibility = Visibility.Visible;
                this.button.Visibility = Visibility.Visible;
            }
            else
            {
                // Hide the other bits
                this.progressRing.IsActive = true;
                this.loadLabel.Visibility = Visibility.Visible;
                this.loadMessage.Visibility = Visibility.Visible;

                // Enable logon
                this.labelTitle.Visibility = Visibility.Hidden;
                this.grid.Visibility = Visibility.Hidden;
                this.button.Visibility = Visibility.Hidden;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            bool userFilled = false;
            hideError();
            if (!string.IsNullOrWhiteSpace(this.textUser.Text) && !string.IsNullOrWhiteSpace(this.passwordBox.Password))
            {
                Switcher.SimsApiClass.SetSimsUser = this.textUser.Text;
                Switcher.SimsApiClass.SetSimsPass = this.passwordBox.Password;
                userFilled = true;
            }

            if ((bool)this.checkWin.IsChecked || userFilled)
            {
                // Try to connect to SIMS...
                updateLoadMess("Connecting to SIMS...");
                ShowLogon(false);
                
                Switcher.SimsApiClass.SetIsTrusted = (bool)this.checkWin.IsChecked;

                bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.WorkerSupportsCancellation = true;
                bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                //bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

                if (bw.IsBusy != true)
                {
                    bw.RunWorkerAsync();
                }
            }


        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                logger.Log(NLog.LogLevel.Info, "Cancelled");
                ShowLogon(true);
            }
            else if (!(e.Error == null))
            {
                logger.Log(NLog.LogLevel.Error, e.Error);
                ShowLogon(true);
            }
            else
            {
                if (!IsConnected)
                {
                    ShowLogon(true);
                    showError(Switcher.SimsApiClass.GetConnectError);
                }
                else
                {
                    Switcher.Switch(new Menu());
                }
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            IsConnected = Switcher.SimsApiClass.Connect;
        }

        private void showError(string errMess)
        {
            errorBorder.BorderThickness = new Thickness(2,2,2,2);
            this.errorMessage.Text = errMess;
        }

        private void hideError()
        {
            errorBorder.BorderThickness = new Thickness(0, 0, 0, 0);
            this.errorMessage.Text = "";
        }

        private void connectType_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)this.checkSql.IsChecked)
            {
                if (this.textUser.Text == null)
                {
                    this.textUser.Text = tmpUsr;
                }
                this.textUser.IsEnabled = true;
                this.passwordBox.IsEnabled = true;
            }
            else
            {
                tmpUsr = this.textUser.Text;
                this.textUser.Text = null;
                this.passwordBox.Password = null;
                this.textUser.IsEnabled = false;
                this.passwordBox.IsEnabled = false;
            }
        }
    }
}
