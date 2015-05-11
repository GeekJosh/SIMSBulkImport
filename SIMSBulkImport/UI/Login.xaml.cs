/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System.ComponentModel;
using System.Windows;
using Matt40k.SIMSBulkImport.Updater;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for Login.xaml
    /// </summary>
    public partial class Login
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private bool IsConnected;
        private BackgroundWorker bw = new BackgroundWorker();
        private string tmpUsr;

        public Login()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Login()");
            InitializeComponent();
            textUser.Focus();
            BeginLoad();
        }

        private void updateLoadMess(string message)
        {
            logger.Log(LogLevel.Debug, message);
            loadMessage.Content = message;
        }

        private void BeginLoad()
        {
            logger.Log(LogLevel.Info,
                "==============================================================================================");
            logger.Log(LogLevel.Info,
                "==============================================================================================");
            logger.Log(LogLevel.Info, "");
            logger.Log(LogLevel.Info, GetExe.Title + " - " + GetExe.Version);
            logger.Log(LogLevel.Info, "");

            // Clear previous temp files we created
            updateLoadMess("Clearing up");
            ClearUp.ClearTmp();

            // Check for updates
            Switcher.ConfigManClass = new ConfigMan();
            if (Switcher.ConfigManClass.CheckForUpdates)
            {
                updateLoadMess("Checking for updates");
                Update.Check();
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
                progressRing.IsActive = false;
                loadLabel.Visibility = Visibility.Hidden;
                loadMessage.Visibility = Visibility.Hidden;

                // Enable logon
                labelTitle.Visibility = Visibility.Visible;
                grid.Visibility = Visibility.Visible;
                button.Visibility = Visibility.Visible;
            }
            else
            {
                // Hide the other bits
                progressRing.IsActive = true;
                loadLabel.Visibility = Visibility.Visible;
                loadMessage.Visibility = Visibility.Visible;

                // Enable logon
                labelTitle.Visibility = Visibility.Hidden;
                grid.Visibility = Visibility.Hidden;
                button.Visibility = Visibility.Hidden;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            bool userFilled = false;
            hideError();
            if (!string.IsNullOrWhiteSpace(textUser.Text) && !string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                Switcher.SimsApiClass.SetSimsUser = textUser.Text;
                Switcher.SimsApiClass.SetSimsPass = passwordBox.Password;
                userFilled = true;
            }

            if ((bool) checkWin.IsChecked || userFilled)
            {
                // Try to connect to SIMS...
                updateLoadMess("Connecting to SIMS...");
                ShowLogon(false);

                Switcher.SimsApiClass.SetIsTrusted = (bool) checkWin.IsChecked;

                bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.WorkerSupportsCancellation = true;
                bw.DoWork += bw_DoWork;
                //bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
                bw.RunWorkerCompleted += bw_RunWorkerCompleted;

                if (bw.IsBusy != true)
                {
                    bw.RunWorkerAsync();
                }
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                logger.Log(LogLevel.Info, "Cancelled");
                ShowLogon(true);
            }
            else if (!(e.Error == null))
            {
                logger.Log(LogLevel.Error, e.Error);
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
            var worker = sender as BackgroundWorker;

            IsConnected = Switcher.SimsApiClass.Connect;
        }

        private void showError(string errMess)
        {
            errorBorder.BorderThickness = new Thickness(2, 2, 2, 2);
            errorMessage.Text = errMess;
        }

        private void hideError()
        {
            errorBorder.BorderThickness = new Thickness(0, 0, 0, 0);
            errorMessage.Text = "";
        }

        private void connectType_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool) checkSql.IsChecked)
            {
                if (textUser.Text == null)
                {
                    textUser.Text = tmpUsr;
                }
                textUser.IsEnabled = true;
                passwordBox.IsEnabled = true;
            }
            else
            {
                tmpUsr = textUser.Text;
                textUser.Text = null;
                passwordBox.Password = null;
                textUser.IsEnabled = false;
                passwordBox.IsEnabled = false;
            }
        }
    }
}
