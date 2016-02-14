using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NLog;
using System.ComponentModel;
using SIMSBulkImport.UserGen;

namespace SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for UserSet.xaml
    /// </summary>
    public partial class UserSet
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private BackgroundWorker bw = new BackgroundWorker();

        private DataTable _pupilData;
        private DataTable _userData;

        // User a list to store all the usernames, used for checking if the username already exists
        private List<string> _users = new List<string>();

        private DateTime queryEnd;
        private DateTime queryStart;
        private int recordcount;
        private int recordupto;

        public UserSet()
        {
            logger.Log(LogLevel.Debug, "SIMSBulkImport.UserSet.UserSet()");
            InitializeComponent();

            Status.Content = "Querying SIMS database - 0%";
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;

            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }
        }

        public bool SetUsername(string username)
        {
            logger.Log(LogLevel.Debug, "SIMSBulkImport.UserSet.SetUsername(username)");
            if (!_users.Exists(x => x == username))
            {
                _users.Add(username);
                return true;
            }
            return false;
        }


        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.UserSet.bw_RunWorkerCompleted()");
            if (e.Cancelled)
            {
                logger.Log(LogLevel.Info, "User cancelled");
                this.Status.Content = "User cancelled";
            }
            else if (!(e.Error == null))
            {
                logger.Log(LogLevel.Error, "Error - " + e.Error.Message);
                this.Status.Content = "Error - " + e.Error.Message;
            }
            else
            {
                logger.Log(LogLevel.Error, "Completed");
                this.Status.Content = "Completed";

                queryEnd = DateTime.Now;
                logger.Log(LogLevel.Info,
                    "Querying complete: " + queryStart.ToShortTimeString() + " - " +
                    DateTime.Compare(queryEnd, queryStart));
                dataGrid.Items.Refresh();
                this.dataGrid.IsReadOnly = false;
                this.dataGrid.CanUserDeleteRows = true;
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //logger.Log(LogLevel.Debug, "SIMSBulkImport.UserSet.bw_ProgressChanged()");
            Status.Content = "Querying SIMS database - " + e.ProgressPercentage + "%";
            // Refreshing the dataGrid causes a crash in .net 4.5, so don't refresh. Version number for 4.5 
            // is actually 4.0.30319.17626 for some reason
            // http://stackoverflow.com/questions/12971881/how-to-reliably-detect-the-actual-net-4-5-version-installed
            Version dotNetVersion = System.Environment.Version;
            if (dotNetVersion.Major <= 4 && dotNetVersion.Minor <= 0 && dotNetVersion.Build <= 30319 && dotNetVersion.Revision < 17626)
            {
                dataGrid.DataContext = _pupilData;
                dataGrid.Items.Refresh();
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            logger.Log(LogLevel.Debug, "SIMSBulkImport.UserSet.bw_DoWork()");
            queryStart = DateTime.Now;
            var worker = sender as BackgroundWorker;

            // Query SIMS database and get a total count of all the (current) pupils.
            recordcount = Switcher.SimsApiClass.GetPupilUsernameCount;
            recordupto = 0;
            logger.Log(LogLevel.Debug, "Total row count: " + recordcount);

            // Pull the SIMS data in
            _pupilData = Switcher.SimsApiClass.GetPupilUsernames;

            //while (recordupto < recordcount)
            //{

            foreach (DataRow _dr in _pupilData.Rows)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    logger.Log(LogLevel.Debug, "Kill process received");
                    break;
                }

                string newUser;
                bool addNewOk;

                string systemId = _dr["SystemId"].ToString();
                string forename = _dr["Forename"].ToString();
                string surname = _dr["Surname"].ToString();
                string admissionNo = _dr["AdmissionNo"].ToString();
                string admissionYear = _dr["AdmissionYear"].ToString();
                string yearGroup = _dr["YearGroup"].ToString();
                string regGroup = _dr["RegGroup"].ToString();
                int personId = Convert.ToInt32(systemId);
                int increment = 0;

                string simsUser = Switcher.SimsApiClass.GetPupilUsername(personId);
                string status = "New";
                if (string.IsNullOrWhiteSpace(simsUser))
                {
                    status = "Exists";
                    newUser = simsUser;
                }
                else
                {
                    // Generate username
                    newUser = Switcher.UserGenClass.GenerateUsername(forename, surname, admissionNo, admissionYear, yearGroup, regGroup, systemId, increment.ToString());
                    addNewOk = SetUsername(newUser);

                    // Deal with duplicates - but only if we're using the increment field in our username expression
                    while (!addNewOk && Switcher.UserGenClass.ExpressionContainsIncrement)
                    {
                        increment = increment + 1;
                        newUser = Switcher.UserGenClass.GenerateUsername(forename, surname, admissionNo, admissionYear, yearGroup, regGroup, systemId, increment.ToString());
                        addNewOk = SetUsername(newUser);
                    }
                }

                DataRow row = GetUserData.NewRow();
                row["Forename"] = forename;
                row["Surname"] = surname;
                row["YearGroup"] = yearGroup;
                row["RegGroup"] = regGroup;
                row["Username"] = newUser;
                row["ExistsOrNew"] = status;
                GetUserData.Rows.Add(row);

                recordupto = recordupto + 1;
                // Update report progress
                int percent = Convert.ToInt32(recordupto * 100 / recordcount);
                worker.ReportProgress(percent);
                
                //}
            }
        }

        private DataTable GetUserData
        {
            get
            {
                logger.Log(LogLevel.Debug, "GetUsernameData");
                if (_userData == null)
                {
                    _userData = new DataTable();
                    _userData.Columns.Add(new DataColumn("Forename", typeof(string)));
                    _userData.Columns.Add(new DataColumn("Surname", typeof(string)));
                    _userData.Columns.Add(new DataColumn("YearGroup", typeof(string)));
                    _userData.Columns.Add(new DataColumn("RegGroup", typeof(string)));
                    _userData.Columns.Add(new DataColumn("Username", typeof(string)));
                    _userData.Columns.Add(new DataColumn("ExistsOrNew", typeof(string)));
                }
                return _userData;
            }
        }
    }
}