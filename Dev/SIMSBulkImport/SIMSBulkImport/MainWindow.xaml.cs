/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private SIMSAPI simsApi;

        private BackgroundWorker bw = new BackgroundWorker();
        private BackgroundWorker bwImport = new BackgroundWorker();

        private DataTable dataGridTable;

        private int emailCount;
        private int udfCount;
        private int ignoreCount;
        private int recordcount;
        private int recordupto = 0;

        private DateTime importStart;
        private DateTime importEnd;

        private DateTime queryStart;
        private DateTime queryEnd;

        private ImportFromFile importFromFile = new ImportFromFile();

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            logger.Log(NLog.LogLevel.Info, "==============================================================================================");
            logger.Log(NLog.LogLevel.Info, "==============================================================================================");
            logger.Log(NLog.LogLevel.Info, "");
            logger.Log(NLog.LogLevel.Info, GetExe.Title + " - " + GetExe.Version);
            logger.Log(NLog.LogLevel.Info, "");

            // Clear previous temp files we created
            Clear.ClearTmp();

            // Check for updates
            Update.Check();
            
            InitializeComponent();
            this.Title = GetExe.Title;
            this.labelTitle.Content = GetExe.Title;

            logger.Log(NLog.LogLevel.Debug, "Loading Branding...");
            GetBranding();

            logger.Log(NLog.LogLevel.Debug, "Loading SIMS API...");

            try
            {
                SimsIni simsIni = new SimsIni();
                simsApi = new SIMSAPI(simsIni.GetSimsDir);
            }
            catch (Exception SIMSAPI_Exception)
            {
                MessageBox.Show(SIMSAPI_Exception.ToString());
            }


            /* 
             * TO REMOVE (LICENSING)
             *             
            logger.Log(NLog.LogLevel.Info, simsApi.GetLicense);
            
            if (simsApi.IsDemo)
            {
                MessageBox.Show("You don't appear to have entered a license key\n\nYou can only import email addresses into the\nCapita Green Abbey Training database", "Demo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
             */
        }

        public void Reset()
        {
            //this.Status.Content = "Not Connected";

            if (bw.WorkerSupportsCancellation == true)
            {
                bw.CancelAsync();
                bw.Dispose();
                bw = null;
            }
            emailCount = 0;
            udfCount = 0;
            ignoreCount = 0;
            recordupto = 0;
            this.dataGrid.DataContext = null;
            dataGridTable = null;

            importFromFile.Reset();
            simsApi.Reset();

            this.dataGrid.Visibility = Visibility.Hidden;
            this.labelTitle.Visibility = Visibility.Visible;
            //this.imageLogo.Visibility = Visibility.Visible;
            this.button.Visibility = Visibility.Hidden;
        }

        private bool IsConnected = false;

        private bool GetConnection
        {
            get
            {
                if (IsConnected)
                {
                    return true;
                }
                try
                {
                    // Blank it ready for use
                    Reset();

                    Logon logon = new Logon(simsApi);
                    logon.ShowDialog();
                }
                catch (Exception GetConnectionException)
                {
                    logger.Log(NLog.LogLevel.Error, GetConnectionException);
                    MessageBox.Show(GetConnectionException.ToString());
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(simsApi.GetCurrentSchool))
                {
                    this.Status.Content = "Connected to: " + simsApi.GetCurrentSchool;
                    logger.Log(NLog.LogLevel.Info, "Connected to: " + simsApi.GetCurrentSchool);
                    logger.Log(NLog.LogLevel.Info, "Connected as: " + simsApi.GetCurrentUser);

                    IsConnected = true;
                    return true;
                    /*
                    Open open = new Open(simsApi);
                    open.ShowDialog();
                }

                if (!string.IsNullOrWhiteSpace(simsApi.GetImportFile))
                {
                    return true;
                }
                Reset();
                return false;*/
                }
                return false;
            }
        }

        /// <summary>
        /// Run once the SIMS .net database has finished being queried 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                this.Status.Content = "Disconnected";
                logger.Log(NLog.LogLevel.Info, "User cancelled");
            }
            else if (!(e.Error == null))
            {
                logger.Log(NLog.LogLevel.Error, "Error - " + e.Error.Message);
            }
            else
            {
                this.Status.Content = "";
                this.button.Visibility = Visibility.Visible;
                this.button.IsEnabled = true;
                this.MenuPrint.IsEnabled = true;
                queryEnd = DateTime.Now;
                logger.Log(NLog.LogLevel.Info, "Querying complete: " + queryStart.ToShortTimeString() + " - " + DateTime.Compare(queryEnd, queryStart));
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Status.Content = "Querying SIMS database - " + e.ProgressPercentage.ToString() + "%";
            this.dataGrid.DataContext = dataGridTable;
            this.dataGrid.Items.Refresh();
        }

        private void MenuItem_Click_New_Contact(object sender, RoutedEventArgs e)
        {
            string nameType = "Contact";

            logger.Log(NLog.LogLevel.Info, nameType + " selected");
            if (GetConnection)
            {
                Open open = new Open(importFromFile);
                open.ShowDialog();

                if (!string.IsNullOrWhiteSpace(importFromFile.GetImportFile))
                {
                    simsApi.SetImportType = 3;
                    Match match = new Match(simsApi, importFromFile);
                    match.ShowDialog();

                    if (simsApi.GetMatched)
                    {
                        this.dataGrid.Visibility = Visibility.Visible;
                        this.labelTitle.Visibility = Visibility.Hidden;
                        //this.imageLogo.Visibility = Visibility.Hidden;

                        this.dataGrid.Items.Refresh();

                        recordcount = simsApi.GetImportFileRecordCount;
                        logger.Log(NLog.LogLevel.Info, "Record count: " + recordcount);

                        queryStart = DateTime.Now;
                        logger.Log(NLog.LogLevel.Info, "Querying started " + queryStart.ToShortTimeString());

                        bw = new BackgroundWorker();
                        bw.WorkerReportsProgress = true;
                        bw.WorkerSupportsCancellation = true;
                        bw.DoWork += new DoWorkEventHandler(bwContact_DoWork);
                        bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
                        bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

                        if (bw.IsBusy != true)
                        {
                            bw.RunWorkerAsync();
                        }
                        this.labelTitle.Visibility = Visibility.Hidden;
                        //this.imageLogo.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        private void bwContact_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            simsApi.CreateContactResultTable();
            dataGridTable = simsApi.CreateContactDataTable;

            while (recordupto < recordcount)
            {
                logger.Log(NLog.LogLevel.Debug, " -- " + recordupto);
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    logger.Log(NLog.LogLevel.Debug, "Kill process received - contact");
                    break;
                }
                else
                {
                    dataGridTable = simsApi.AddContactToDataTable(dataGridTable, recordupto);
                    recordupto++;
                    //logger.Log(NLog.LogLevel.Info, recordupto + recordcount);

                    long lonCount = recordupto;
                    long lonTotal = recordcount;
                    int percent = Convert.ToInt32(lonCount * 100 / lonTotal);

                    worker.ReportProgress(percent);
                }
            }
        }

        private void MenuItem_Click_New_Pupil(object sender, RoutedEventArgs e)
        {
            string nameType = "Pupil";

            logger.Log(NLog.LogLevel.Info, nameType + " selected");
            if (GetConnection)
            {
                Open open = new Open(importFromFile);
                open.ShowDialog();

                if (!string.IsNullOrWhiteSpace(importFromFile.GetImportFile))
                {
                    simsApi.SetImportType = 2;
                    Match match = new Match(simsApi, importFromFile);
                    match.ShowDialog();

                    if (simsApi.GetMatched)
                    {
                        if (!string.IsNullOrWhiteSpace(importFromFile.GetImportFile))
                        {
                            this.dataGrid.Visibility = Visibility.Visible;
                            this.labelTitle.Visibility = Visibility.Hidden;
                            //this.imageLogo.Visibility = Visibility.Hidden;

                            this.dataGrid.Items.Refresh();

                            recordcount = simsApi.GetImportFileRecordCount;

                            queryStart = DateTime.Now;
                            logger.Log(NLog.LogLevel.Info, "Querying started " + queryStart.ToShortTimeString());

                            bw = new BackgroundWorker();
                            bw.WorkerReportsProgress = true;
                            bw.WorkerSupportsCancellation = true;
                            bw.DoWork += new DoWorkEventHandler(bwPupil_DoWork);
                            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
                            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

                            if (bw.IsBusy != true)
                            {
                                bw.RunWorkerAsync();
                            }
                            this.labelTitle.Visibility = Visibility.Hidden;
                            //this.imageLogo.Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
        }
      
        private void bwPupil_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            simsApi.CreatePupilResultTable();
            dataGridTable = simsApi.CreatePupilDataTable;

            while (recordupto < recordcount)
            {
                logger.Log(NLog.LogLevel.Debug, " -- " + recordupto);
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    logger.Log(NLog.LogLevel.Debug, "Kill process received - pupil");
                    break;
                }
                else
                {
                    dataGridTable = simsApi.AddPupilToDataTable(dataGridTable, recordupto);
                    recordupto++;
                    //logger.Log(NLog.LogLevel.Info, recordupto + recordcount);

                    long lonCount = recordupto;
                    long lonTotal = recordcount;
                    int percent = Convert.ToInt32(lonCount * 100 / lonTotal);

                    worker.ReportProgress(percent);
                }
            }
        }

        private void MenuItem_Click_New_Staff(object sender, RoutedEventArgs e)
        {
            string nameType = "Staff";

            logger.Log(NLog.LogLevel.Info, nameType + " selected");
            if (GetConnection)
            {
                Open open = new Open(importFromFile);
                open.ShowDialog();

                if (!string.IsNullOrWhiteSpace(importFromFile.GetImportFile))
                {
                    simsApi.SetImportType = 1;
                    Match match = new Match(simsApi, importFromFile);
                    match.ShowDialog();

                    if (simsApi.GetMatched)
                    {
                        this.dataGrid.Visibility = Visibility.Visible;
                        this.labelTitle.Visibility = Visibility.Hidden;
                        //this.imageLogo.Visibility = Visibility.Hidden;

                        this.dataGrid.Items.Refresh();

                        recordcount = simsApi.GetImportFileRecordCount;

                        queryStart = DateTime.Now;
                        logger.Log(NLog.LogLevel.Info, "Querying started " + queryStart.ToShortTimeString());

                        bw = new BackgroundWorker();
                        bw.WorkerReportsProgress = true;
                        bw.WorkerSupportsCancellation = true;
                        bw.DoWork += new DoWorkEventHandler(bwStaff_DoWork);
                        bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
                        bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

                        if (bw.IsBusy != true)
                        {
                            bw.RunWorkerAsync();
                        }
                        this.labelTitle.Visibility = Visibility.Hidden;
                        //this.imageLogo.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        private void bwStaff_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            simsApi.CreateStaffResultTable();
            dataGridTable = simsApi.CreateStaffDataTable;

            while (recordupto < recordcount)
            {
                logger.Log(NLog.LogLevel.Debug, " -- " + recordupto);
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    logger.Log(NLog.LogLevel.Debug, "Kill process received - staff");
                    break;
                }
                else
                {
                    dataGridTable = simsApi.AddStaffToDataTable(dataGridTable, recordupto);
                    recordupto++;

                    long lonCount = recordupto;
                    long lonTotal = recordcount;
                    int percent = Convert.ToInt32(lonCount * 100 / lonTotal);

                    worker.ReportProgress(percent);
                }
            }
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.Show();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            /* 
             * TO REMOVE (LICENSING)
            if (simsApi.GetIsLicensed)
            {
             */
            logger.Log(NLog.LogLevel.Info, "Import Start");
            importStart = DateTime.Now;

            switch (simsApi.GetImportType)
            {
                case 1:
                    StaffImport();
                    break;
                case 2:
                    PupilImport();
                    break;
                case 3:
                    ContactImport();
                    break;
            }
            /* 
             * TO REMOVE (LICENSING)
            }
                else
                {
                    logger.Log(NLog.LogLevel.Error, "Unlicensed");
                    MessageBox.Show("You are not licensed to use " + GetExe.Title, "Unlicensed", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
             */
        }
        

        private int GetAverage
        {
            get
            {
                try
                {
                    return (emailCount + udfCount) / DateTime.Compare(importEnd, importStart);
                }
                catch (System.Exception GetAverageException)
                {
                    logger.Log(NLog.LogLevel.Error, GetAverageException);
                }
                return 0;
            }
        }

        private void bwImport_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Status.Content = "Importing into SIMS - " + e.ProgressPercentage.ToString() + "%";
        }

        private void bwImport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                this.Status.Content = "Disconnected";
                logger.Log(NLog.LogLevel.Info, "User cancelled");
            }
            else if (!(e.Error == null))
            {
                logger.Log(NLog.LogLevel.Error, "Error - " + e.Error.Message);
            }
            else
            {
                importEnd = DateTime.Now;
                this.Reset();
                this.Status.Content = "Import Complete";
                this.button.IsEnabled = true;
                queryEnd = DateTime.Now;
                logger.Log(NLog.LogLevel.Info, "Import Complete: " + queryStart.ToShortTimeString() + " - " + DateTime.Compare(queryEnd, queryStart));
                logger.Log(NLog.LogLevel.Debug, "Imported - Emails: " + emailCount + "\n " +
                    "Imported - UDFs: " + udfCount + "\n " +
                    "Ignored: " + ignoreCount + "\n " +
                    "Start time: " + importStart.ToShortTimeString() + "\n " +
                    "End time: " + importEnd.ToShortTimeString() + "\n " +
                    "Time: " + DateTime.Compare(importEnd, importStart) + " seconds\n " +
                    "Import per second: " + GetAverage);
                this.Hide();
                Results results = new Results(simsApi.GetResultTable, simsApi.GetImportType);
                this.Close();
            }
        }

        private void bwImportContact_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            logger.Log(NLog.LogLevel.Debug, "ImportContact");
            int recordcount = dataGridTable.Rows.Count;
            int recordupto = 0;

            while (recordupto < recordcount)
            {
                logger.Log(NLog.LogLevel.Debug, " -- " + recordupto);
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    logger.Log(NLog.LogLevel.Debug, "Kill process received - pupilimport");
                    break;
                }
                else
                {
                    DataRow row = dataGridTable.Rows[recordupto];

                    string status = (row["Status"].ToString());
                    if (status.StartsWith("Import"))
                    {
                        string personId = (row["PersonID"].ToString());
                        string surname = (row["Surname"].ToString());
                        string forename = (row["Forename"].ToString());
                        string postcode = (row["Postcode"].ToString());
                        string town = (row["Town"].ToString());

                        int pid = Convert.ToInt32(personId);
                        if (status.Contains("email"))
                        {
                            emailCount = emailCount + 1;
                            string personEmail = (row["Import email"].ToString());
                            bool importResult = importContactEmail(pid, personEmail);
                            simsApi.AddContactResultToTable(surname, forename, postcode, town, personId, "Email", personEmail, importResult, "");
                        }
                        if (status.Contains("UDF"))
                        {
                            udfCount = udfCount + 1;
                            string personUdf = (row["Import UDF"].ToString());
                            bool importResult = importContactUDF(pid, personUdf);
                            simsApi.AddContactResultToTable(surname, forename, postcode, town, personId, "UDF", personUdf, importResult, "");
                        }
                    }
                    else
                    {
                        ignoreCount = ignoreCount + 1;
                    }
                    recordupto++;

                    long lonCount = recordupto;
                    long lonTotal = recordcount;
                    int percent = Convert.ToInt32(lonCount * 100 / lonTotal);

                    worker.ReportProgress(percent);
                }
            }
        }

        private void bwImportPupil_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            logger.Log(NLog.LogLevel.Debug, "ImportPupil");
            int recordcount = dataGridTable.Rows.Count;
            int recordupto = 0;

            while (recordupto < recordcount)
            {
                logger.Log(NLog.LogLevel.Debug, " -- " + recordupto);
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    logger.Log(NLog.LogLevel.Debug, "Kill process received - pupilimport");
                    break;
                }
                else
                {
                    DataRow row = dataGridTable.Rows[recordupto];
                    
                    string status = (row["Status"].ToString());
                    if (status.StartsWith("Import"))
                    {
                        
                        string personId = (row["PersonID"].ToString());
                        string surname = (row["Surname"].ToString());
                        string forename = (row["Forename"].ToString());
                        string gender = (row["Gender"].ToString());
                        string admis = (row["Gender"].ToString());
                        string dob= (row["Gender"].ToString());
                        string year= (row["Gender"].ToString());
                        string reg = (row["Gender"].ToString());
                        string house = (row["Gender"].ToString());
                        //string personName = (forename + " " + surname);

                        int pid = Convert.ToInt32(personId);
                        if (status.Contains("email"))
                        {
                            emailCount = emailCount + 1;
                            string personEmail = (row["Import email"].ToString());
                            bool importResult = importPupilEmail(pid, personEmail);
                            simsApi.AddPupilResultToTable(surname, forename, gender, admis, dob, year, reg, house, personId, "Email", personEmail, importResult, "");
                        }
                        if (status.Contains("UDF"))
                        {
                            udfCount = udfCount + 1;
                            string personUdf = (row["Import UDF"].ToString());
                            bool importResult = importPupilUDF(pid, personUdf);
                            simsApi.AddPupilResultToTable(surname, forename, gender, admis, dob, year, reg, house, personId, "UDF", personUdf, importResult, "");
                        }
                    }
                    else
                    {
                        ignoreCount = ignoreCount + 1;
                    }
                    recordupto++;

                    long lonCount = recordupto;
                    long lonTotal = recordcount;
                    int percent = Convert.ToInt32(lonCount * 100 / lonTotal);

                    worker.ReportProgress(percent);
                }
            }
        }

        private void bwImportStaff_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            logger.Log(NLog.LogLevel.Debug, "ImportStaff");
            int recordcount = dataGridTable.Rows.Count;
            int recordupto = 0;

            while (recordupto < recordcount)
            {
                logger.Log(NLog.LogLevel.Debug, " -- " + recordupto);
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    logger.Log(NLog.LogLevel.Debug, "Kill process received - staffimport");
                    break;
                }
                else
                {
                    DataRow row = dataGridTable.Rows[recordupto];
                    string surname = (row["Surname"].ToString());
                    string forename = (row["Forename"].ToString());
                    string gender = (row["Gender"].ToString());
                    string staffcode = (row["Staff Code"].ToString());
                    string dob = (row["Date of Birth"].ToString());
                    //string personName = (row["Forename"].ToString()) + " " + (row["Surname"].ToString());
                    string personId = (row["PersonID"].ToString());
                    string status = (row["Status"].ToString());

                    if (status.StartsWith("Import"))
                    {
                        int pid = Convert.ToInt32(personId);
                        if (status.Contains("email"))
                        {
                            emailCount = emailCount + 1;
                            string personEmail = (row["Import email"].ToString());
                            bool importResult = importStaffEmail(pid, personEmail);
                            simsApi.AddStaffResultToTable(surname, forename, gender, staffcode, dob, personId, "Email", personEmail, importResult, "");
                        }
                        if (status.Contains("UDF"))
                        {
                            udfCount = udfCount + 1;
                            string personUdf = (row["Import UDF"].ToString());
                            bool importResult = importStaffUDF(pid, personUdf);
                            simsApi.AddStaffResultToTable(surname, forename, gender, staffcode, dob, personId, "UDF", personUdf, importResult, "");
                        }
                    }
                    else
                    {
                        ignoreCount = ignoreCount + 1;
                    }
                    recordupto++;

                    long lonCount = recordupto;
                    long lonTotal = recordcount;
                    int percent = Convert.ToInt32(lonCount * 100 / lonTotal);

                    worker.ReportProgress(percent);
                }
            }
        }

        private void ContactImport()
        {
            logger.Log(NLog.LogLevel.Debug, "ContactImport");

            bwImport = new BackgroundWorker();
            bwImport.WorkerReportsProgress = true;
            bwImport.WorkerSupportsCancellation = true;
            bwImport.DoWork += new DoWorkEventHandler(bwImportContact_DoWork);
            bwImport.ProgressChanged += new ProgressChangedEventHandler(bwImport_ProgressChanged);
            bwImport.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwImport_RunWorkerCompleted);
            if (bwImport.IsBusy != true)
            {
                bwImport.RunWorkerAsync();
            }
        }

        private void PupilImport()
        {
            logger.Log(NLog.LogLevel.Debug, "PupilImport");

            bwImport = new BackgroundWorker();
            bwImport.WorkerReportsProgress = true;
            bwImport.WorkerSupportsCancellation = true;
            bwImport.DoWork += new DoWorkEventHandler(bwImportPupil_DoWork);
            bwImport.ProgressChanged += new ProgressChangedEventHandler(bwImport_ProgressChanged);
            bwImport.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwImport_RunWorkerCompleted);

            if (bwImport.IsBusy != true)
            {
                bwImport.RunWorkerAsync();
            }
        }

        private void StaffImport()
        {
            logger.Log(NLog.LogLevel.Debug, "StaffImport");

            bwImport = new BackgroundWorker();
            bwImport.WorkerReportsProgress = true;
            bwImport.WorkerSupportsCancellation = true;
            bwImport.DoWork += new DoWorkEventHandler(bwImportStaff_DoWork);
            bwImport.ProgressChanged += new ProgressChangedEventHandler(bwImport_ProgressChanged);
            bwImport.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwImport_RunWorkerCompleted);

            if (bwImport.IsBusy != true)
            {
                bwImport.RunWorkerAsync();
            }
        }

        private bool importContactEmail(int personid, string address)
        {
            if (personid == 0) { return true; }
            if (string.IsNullOrWhiteSpace(address)) { return true; }
            return simsApi.SetContactEmail(personid, address);
        }

        private bool importStaffEmail(int personid, string address)
        {
            if (personid == 0) { return true; }
            if (string.IsNullOrWhiteSpace(address)) { return true; }
            return simsApi.SetStaffEmail(personid, address);
        }

        private bool importPupilEmail(int personid, string address)
        {
            if (personid == 0) { return false; }
            if (string.IsNullOrWhiteSpace(address)) { return false; }
            return simsApi.SetStudentEmail(personid, address);
        }

        private bool importStaffUDF(int personid, string UDF)
        {
            if (personid == 0) { return true; }
            if (string.IsNullOrWhiteSpace(UDF)) { return true; }
            return simsApi.SetStaffUDF(personid, UDF);
        }

        private bool importPupilUDF(int personid, string UDF)
        {
            if (personid == 0) { return false; }
            if (string.IsNullOrWhiteSpace(UDF)) { return false; }
            return simsApi.SetStudentUDF(personid, UDF);
        }

        private bool importContactUDF(int personid, string UDF)
        {
            return false;
        }

        /*
         * TO REMOVE (LICENSING)
        private void MenuItem_Click_License(object sender, RoutedEventArgs e)
        {
            LicenseView licenceView1 = new LicenseView();
            licenceView1.Show();
            //MessageBox.Show(simsApi.GetLicense);
        }
         */

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string tmpFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Connect.ini");
            if (System.IO.File.Exists(tmpFile)) {
                try
                {
                    System.IO.File.Delete(tmpFile);
                }
                catch (System.Exception WindowClosingException)
                {
                    logger.Log(NLog.LogLevel.Error, WindowClosingException);
                }
            }
        }

        private void MenuItem_Click_Logs(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\SIMSBulkImport\\Logs";
                prc.Start();
            }
            catch (System.Exception MenuItem_Click_LogsException)
            {
                logger.Log(NLog.LogLevel.Error, MenuItem_Click_LogsException);
            }
        }

        private void MenuItem_Click_Options(object sender, RoutedEventArgs e)
        {
            Options options = new Options();
            options.ShowDialog();
        }

        private void GetBranding()
        {
            bool showText = Branding.Brand.ShowText;
            bool showLogo = Branding.Brand.ShowLogo;

            if (showLogo)
            {
                this.imageLogo.Visibility = Visibility.Visible;
                this.labelTitle.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            }

            if (showText)
            {
                this.labelTitle.Visibility = Visibility.Visible;
            }
        }

        private void MenuItem_Click_Print(object sender, RoutedEventArgs e)
        {
            DataTable currentDt = (DataTable)dataGrid.DataContext;
            Results results = new Results(dataGridTable, simsApi.GetImportType);
        }
    }
}