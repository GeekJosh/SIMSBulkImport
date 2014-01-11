/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using NLog;
using NLog.Targets;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for Submit.xaml
    /// </summary>
    public partial class Submit
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private string _email;
        private string _log;
        private DataTable _submittionResults;

        public Submit()
        {
            InitializeComponent();
            resetUI();
        }

        private string _logFile
        {
            get
            {
                var t = (FileTarget) LogManager.Configuration.FindTargetByName("system");
                var logEventInfo = new LogEventInfo {TimeStamp = DateTime.Now};
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
                    var log = new StreamReader(logFile);
                    _log = log.ReadToEnd();
                    log.Close();
                }
                return _log;
            }
        }

        private bool submittedOk
        {
            get
            {
                try
                {
                    DataRow dt = _submittionResults.Rows[0];
                    string _status = dt["success"].ToString();
                    logger.Log(LogLevel.Trace, "Submission Status: " + _status);
                    if (_status == "true")
                        return true;
                    return false;
                }
                catch (Exception submittedOk_Exception)
                {
                    logger.Log(LogLevel.Trace, submittedOk_Exception);
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
                    logger.Log(LogLevel.Trace, "Submission ID: " + _submissionId);
                    return _submissionId;
                }
                catch (Exception submissionId_Exception)
                {
                    logger.Log(LogLevel.Trace, submissionId_Exception);
                    return null;
                }
            }
        }

        private void backClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Logs());
        }

        private void submitClick(object sender, RoutedEventArgs e)
        {
            if (successLabel.Visibility == Visibility.Visible)
                Switcher.Switch(new Menu());
            else
            {
                runningUI();

                _email = Email.Text;
                _log = readLogFile;

                var bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.WorkerSupportsCancellation = true;
                bw.DoWork += PostLogs_DoWork;
                bw.RunWorkerCompleted += PostLogs_RunWorkerCompleted;

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
                logger.Log(LogLevel.Error, GetServerVersion_DoWork_Exception);
            }
        }

        private void PostLogs_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            completeUI();
        }

        private void resetUI()
        {
            progressRing.IsActive = false;
            progressRing.Visibility = Visibility.Hidden;
            progressMessage.Visibility = Visibility.Hidden;

            successLabel.Visibility = Visibility.Hidden;
            uniqueIdLabel.Visibility = Visibility.Visible;
            StatID.Visibility = Visibility.Visible;
            logger.Log(LogLevel.Trace, "StatID: " + Stats.ReadID);
            StatID.Text = Stats.ReadID;

            submissionIdLabel.Visibility = Visibility.Hidden;
            emailLabel.Visibility = Visibility.Visible;
            Email.Visibility = Visibility.Visible;
            Email.Text = "";
            Email.ToolTip = "Optional: Enter your email address if you want a reply";
            Email.IsReadOnly = false;

            submitButton.Content = "Submit";
            backButton.Visibility = Visibility.Visible;
            submitButton.Visibility = Visibility.Visible;
        }

        private void completeUI()
        {
            logger.Log(LogLevel.Trace, "One way or another submission is complete");

            progressRing.IsActive = false;
            progressRing.Visibility = Visibility.Hidden;
            progressMessage.Visibility = Visibility.Hidden;

            successLabel.Visibility = Visibility.Visible;
            uniqueIdLabel.Visibility = Visibility.Hidden;
            StatID.Visibility = Visibility.Hidden;

            submissionIdLabel.Visibility = Visibility.Visible;
            emailLabel.Visibility = Visibility.Hidden;
            Email.Visibility = Visibility.Visible;
            Email.Text = submissionId;
            Email.ToolTip = "Unique log file submission ID";
            Email.IsReadOnly = true;

            submitButton.Content = "OK";
            backButton.Visibility = Visibility.Visible;
            submitButton.Visibility = Visibility.Visible;

            if (submittedOk)
                successLabel.Content = "Success";
            else
                successLabel.Content = "Failure";
        }

        private void runningUI()
        {
            progressRing.IsActive = true;
            progressRing.Visibility = Visibility.Visible;
            progressMessage.Visibility = Visibility.Visible;

            successLabel.Visibility = Visibility.Hidden;
            uniqueIdLabel.Visibility = Visibility.Hidden;
            StatID.Visibility = Visibility.Hidden;

            submissionIdLabel.Visibility = Visibility.Hidden;
            emailLabel.Visibility = Visibility.Hidden;
            Email.Visibility = Visibility.Hidden;

            backButton.Visibility = Visibility.Hidden;
            submitButton.Visibility = Visibility.Hidden;
        }
    }
}