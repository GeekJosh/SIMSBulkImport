/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    /// Interaction logic for Submit.xaml
    /// </summary>
    public partial class Submit
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private DataTable _submittionResults;
        private string _log;
        private string _email;

        public Submit()
        {
            InitializeComponent();
            resetUI();
        }

        private void backClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Switcher.Switch(new Logs());
        }

        private void submitClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.successLabel.Visibility == Visibility.Visible)
                Switcher.Switch(new Menu());
            else
            {
                runningUI();

                _email = Email.Text;
                _log = readLogFile;

                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.WorkerSupportsCancellation = true;
                bw.DoWork += new DoWorkEventHandler(PostLogs_DoWork);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(PostLogs_RunWorkerCompleted);

                if (bw.IsBusy != true)
                {
                    bw.RunWorkerAsync();
                }
            }
        }

        private void PostLogs_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _submittionResults = Support.Submit.Logs(_email, _log);
            }
            catch (Exception GetServerVersion_DoWork_Exception)
            {
                logger.Log(NLog.LogLevel.Error, GetServerVersion_DoWork_Exception);
            }
        }

        private void PostLogs_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            completeUI();
        }

        private string _logFile
        {
            get
            {
                NLog.Targets.FileTarget t = (NLog.Targets.FileTarget)LogManager.Configuration.FindTargetByName("system");
                var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
                return t.FileName.Render(logEventInfo);
            }
        }

        private string readLogFile
        {
            get
            {
                string _log = null;
                string logFile = _logFile;
                if (File.Exists(logFile))
                {
                    StreamReader log = new StreamReader(logFile);
                    _log = log.ReadToEnd();
                    log.Close();
                }
                return _log;
            }
        }

        private void resetUI()
        {
            this.progressRing.IsActive = false;
            this.progressRing.Visibility = Visibility.Hidden;
            this.progressMessage.Visibility = Visibility.Hidden;

            this.successLabel.Visibility = Visibility.Hidden;
            this.uniqueIdLabel.Visibility = Visibility.Visible;
            this.StatID.Visibility = Visibility.Visible;
            logger.Log(NLog.LogLevel.Trace, "StatID: "+Stats.ReadID);
            this.StatID.Text = Stats.ReadID;

            this.submissionIdLabel.Visibility = Visibility.Hidden;
            this.emailLabel.Visibility = Visibility.Visible;
            this.Email.Visibility = Visibility.Visible;
            this.Email.Text = "";
            this.Email.ToolTip = "Optional: Enter your email address if you want a reply";
            this.Email.IsReadOnly = false;

            this.submitButton.Content = "Submit";
            this.backButton.Visibility = Visibility.Visible;
            this.submitButton.Visibility = Visibility.Visible;
        }

        private void completeUI()
        {
            logger.Log(NLog.LogLevel.Trace, "One way or another submission is complete");

            this.progressRing.IsActive = false;
            this.progressRing.Visibility = Visibility.Hidden;
            this.progressMessage.Visibility = Visibility.Hidden;

            this.successLabel.Visibility = Visibility.Visible;
            this.uniqueIdLabel.Visibility = Visibility.Hidden;
            this.StatID.Visibility = Visibility.Hidden;

            this.submissionIdLabel.Visibility = Visibility.Visible;
            this.emailLabel.Visibility = Visibility.Hidden;
            this.Email.Visibility = Visibility.Visible;
            this.Email.Text = submissionId;
            this.Email.ToolTip = "Unique log file submission ID";
            this.Email.IsReadOnly = true;

            this.submitButton.Content = "OK";
            this.backButton.Visibility = Visibility.Visible;
            this.submitButton.Visibility = Visibility.Visible;

            if (submittedOk)
                this.successLabel.Content = "Success";
            else
                this.successLabel.Content = "Failure";
        }

        private void runningUI()
        {
            progressRing.IsActive = true;
            progressRing.Visibility = Visibility.Visible;
            this.progressMessage.Visibility = Visibility.Visible;

            this.successLabel.Visibility = Visibility.Hidden;
            this.uniqueIdLabel.Visibility = Visibility.Hidden;
            this.StatID.Visibility = Visibility.Hidden;

            this.submissionIdLabel.Visibility = Visibility.Hidden;
            this.emailLabel.Visibility = Visibility.Hidden;
            this.Email.Visibility = Visibility.Hidden;

            this.backButton.Visibility = Visibility.Hidden;
            this.submitButton.Visibility = Visibility.Hidden;
        }

        private bool submittedOk
        {
            get
            {
                try
                {
                    DataRow dt = _submittionResults.Rows[0];
                    string _status = dt["success"].ToString();
                    logger.Log(NLog.LogLevel.Trace, "Submission Status: " + _status);
                    if (_status == "true")
                        return true;
                    else
                        return false;
                }
                catch (Exception submittedOk_Exception)
                {
                    logger.Log(NLog.LogLevel.Trace, submittedOk_Exception);
                    return false;
                }
            }
        }

        private string submissionId
        {
            get
            {
                try
                {
                    DataRow dt = _submittionResults.Rows[0];
                    string _submissionId = dt["logid"].ToString();
                    logger.Log(NLog.LogLevel.Trace, "Submission ID: " + _submissionId);
                    return _submissionId;
                }
                catch (Exception submissionId_Exception)
                {
                    logger.Log(NLog.LogLevel.Trace, submissionId_Exception);
                    return null;
                }
            }
        }
    }
}
