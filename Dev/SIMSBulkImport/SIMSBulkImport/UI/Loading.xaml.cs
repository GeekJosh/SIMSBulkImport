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
    /// Interaction logic for Loading.xaml
    /// </summary>
    public partial class Loading
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private BackgroundWorker bw = new BackgroundWorker();
        private bool IsConnected;

        public Loading()
        {
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
            updateLoadMess("Checking for updates");
            //Update.Check();

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
            if (!string.IsNullOrWhiteSpace(this.textUser.Text) && !string.IsNullOrWhiteSpace(this.passwordBox.Password))
            {
                // Try to connect to SIMS...
                updateLoadMess("Connecting to SIMS...");
                ShowLogon(false);

                Switcher.SimsApiClass.SetSimsUser = this.textUser.Text;
                Switcher.SimsApiClass.SetSimsPass = this.passwordBox.Password;


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
                ShowLogon(true);
            }
            else if (!(e.Error == null))
            {
                ShowLogon(true);
            }
            else
            {
                if (!IsConnected)
                {
                    ShowLogon(true);
                }
                else
                {
                    Switcher.Switch(new MainWindow());
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
    }
}
