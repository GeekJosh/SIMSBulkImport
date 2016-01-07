using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UserGen;
using NLog;
using System.ComponentModel;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for UserSet.xaml
    /// </summary>
    public partial class UserSet
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private BackgroundWorker bw = new BackgroundWorker();

        public UserSet()
        {
            InitializeComponent();

            dataGrid.DataContext = _completeData;

            dataGrid.Items.Refresh();
            //Status.Content = "Querying SIMS database - 0%";
            bw = new BackgroundWorker();
            //bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;

            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }

        }

        private DataTable _pupilData;
        private DataTable _completeData;
        private List<string> _users = new List<string>();

        private DataTable GetPupilData
        {
            get
            {
                if (_pupilData == null)
                    _pupilData = Switcher.SimsApiClass.GetPupilHierarchyData;
                return _pupilData;
            }
        }

        public DataTable GetCompleteData
        {
            get
            {
                if (_completeData == null)
                {
                    _completeData = new DataTable();
                    _completeData.Columns.Add(new DataColumn("PersonID", typeof(string)));
                    _completeData.Columns.Add(new DataColumn("Year", typeof(string)));
                    _completeData.Columns.Add(new DataColumn("RegGroup", typeof(string)));
                    _completeData.Columns.Add(new DataColumn("Forename", typeof(string)));
                    _completeData.Columns.Add(new DataColumn("Surname", typeof(string)));
                    _completeData.Columns.Add(new DataColumn("AdmissionNo", typeof(string)));
                    _completeData.Columns.Add(new DataColumn("Username", typeof(string)));
                }
                return _completeData; 
            }
        }

        public bool SetUsername(string username)
        {
            if (!_users.Exists(x => x == username))
            {
                _users.Add(username);
                return true;
            }
            return false;
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.ImportWindow.bw_RunWorkerCompleted()");
            if (e.Cancelled)
            {
                //this.Status.Content = "Disconnected";
                logger.Log(LogLevel.Info, "User cancelled");
            }
            else if (!(e.Error == null))
            {
                logger.Log(LogLevel.Error, "Error - " + e.Error.Message);
            }
            else
            {
                //this.Status.Content = "";

                //queryEnd = DateTime.Now;
                //logger.Log(LogLevel.Info,
                //    "Querying complete: " + queryStart.ToShortTimeString() + " - " +
                //    DateTime.Compare(queryEnd, queryStart));
                dataGrid.Items.Refresh();
                //this.dataGrid.IsReadOnly = false;
                //this.dataGrid.CanUserDeleteRows = true;
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Status.Content = e.ProgressPercentage + "%";
            dataGrid.DataContext = _completeData;
            dataGrid.Items.Refresh();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            //recordcount = Switcher.PreImportClass.GetImportFileRecordCount;
            //recordupto = 0;
            //while (recordupto < recordcount)
            //{

            /*
            if (worker.CancellationPending)
            {
                e.Cancel = true;
                logger.Log(LogLevel.Debug, "Kill process received");
                break;
            }
            */
            _completeData = GetPupilData;


            DataRow[] existingUsers = GetPupilData.Select("IsSet = true");
            foreach (DataRow _dr in existingUsers)
            {
                bool resultAddExisting = SetUsername(_dr["Username"].ToString());
            }

            DataRow[] newUsers = GetPupilData.Select("IsSet = true");
            foreach (DataRow _dr in _pupilData.Rows)
            {
                string newUser;
                bool addNewOk;
                // Pull the SIMS data in
                string personId = _dr["PersonID"].ToString();
                string forename = _dr["Forename"].ToString();
                string surname = _dr["Surname"].ToString();
                string admissionNo = _dr["AdmissionNo"].ToString();
                string admissionYear = _dr["AdmissionYear"].ToString();
                string yearGroup = _dr["YearGroup"].ToString();
                string regGroup = _dr["RegGroup"].ToString();
                int increment = 0;
                newUser = Switcher.UserGenClass.GenerateUsername(forename, surname, admissionNo, admissionYear, yearGroup, regGroup, personId, increment.ToString());
                addNewOk = SetUsername(newUser);

                // Deal with duplicates - but only if we're using the increment field in our username expression
                while (!addNewOk && Switcher.UserGenClass.ExpressionContainsIncrement)
                {
                    increment = increment + 1;
                    newUser = Switcher.UserGenClass.GenerateUsername(forename, surname, admissionNo, admissionYear, yearGroup, regGroup, personId, increment.ToString());
                }

                DataRow _compRow = _completeData.NewRow();
                _compRow["PersonID"] = personId;
                _compRow["Year"] = yearGroup;
                _compRow["RegGroup"] = regGroup;
                _compRow["Forename"] = forename;
                _compRow["Surname"] = surname;
                _compRow["AdmissionNo"] = admissionNo;
                _compRow["Username"] = newUser;
                _completeData.Rows.Add(_compRow);
            }



            //long lonCount = recordupto;
            //long lonTotal = recordcount;
            //int percent = Convert.ToInt32(lonCount * 100 / lonTotal);

            //worker.ReportProgress(percent);
        }
    }
}