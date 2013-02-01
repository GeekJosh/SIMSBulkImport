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
    public partial class MainWindow
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

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

        private ImportFile _importFile = new ImportFile();

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            ConnectedTo();
        }

        public void ConnectedTo()
        {
            string currentSchool = Switcher.SimsApiClass.GetCurrentSchool;
            if (!string.IsNullOrWhiteSpace(currentSchool))
                this.Status.Content = "Connected to: " + currentSchool;
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

            _importFile.Reset();
            Switcher.SimsApiClass.Reset();

            this.dataGrid.Visibility = Visibility.Hidden;
            this.labelTitle.Visibility = Visibility.Visible;
            //this.imageLogo.Visibility = Visibility.Visible;
            this.button.Visibility = Visibility.Hidden;
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
            logger.Log(NLog.LogLevel.Info, SIMSAPI.UserType.Contact + " selected");
            Switcher.SimsApiClass.SetUserType = SIMSAPI.UserType.Contact;
            bool loadOk = importLogic;

            if (loadOk)
            {
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

        private void bwContact_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            Switcher.SimsApiClass.CreateContactResultTable();
            dataGridTable = Switcher.SimsApiClass.CreateContactDataTable;

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
                    dataGridTable = Switcher.SimsApiClass.AddContactToDataTable(dataGridTable, recordupto);
                    recordupto++;
                    //logger.Log(NLog.LogLevel.Info, recordupto + recordcount);

                    long lonCount = recordupto;
                    long lonTotal = recordcount;
                    int percent = Convert.ToInt32(lonCount * 100 / lonTotal);

                    worker.ReportProgress(percent);
                }
            }
        }

        private bool importLogic
        {
            get
            {
                try
                {
                    Switcher.ImportFileClass = new ImportFile();
                    Switcher.Switch(new Open());

                    if (_importFile.IsImportCompleted)
                    {
                        Switcher.Switch(new Match());

                        if (Switcher.SimsApiClass.GetMatched)
                        {
                            if (_importFile.IsImportFileSet)
                            {
                                this.dataGrid.Visibility = Visibility.Visible;
                                this.labelTitle.Visibility = Visibility.Hidden;

                                this.dataGrid.Items.Refresh();

                                recordcount = Switcher.SimsApiClass.GetImportFileRecordCount;

                                queryStart = DateTime.Now;
                                logger.Log(NLog.LogLevel.Info, "Querying started " + queryStart.ToShortTimeString());
                            }
                        }
                    }
                }
                catch (Exception importLogic_Exception)
                {
                    logger.Log(NLog.LogLevel.Error, importLogic_Exception);
                    MessageBox.Show(importLogic_Exception.ToString());
                    return false;
                }
                return true;
            }
        }

        private void MenuItem_Click_New_Pupil(object sender, RoutedEventArgs e)
        {
            logger.Log(NLog.LogLevel.Info, SIMSAPI.UserType.Pupil + " selected");
            Switcher.SimsApiClass.SetUserType = SIMSAPI.UserType.Pupil;
            bool loadOk = importLogic;

            if (loadOk)
            {
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
      
        private void bwPupil_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            Switcher.SimsApiClass.CreatePupilResultTable();
            dataGridTable = Switcher.SimsApiClass.CreatePupilDataTable;

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
                    dataGridTable = Switcher.SimsApiClass.AddPupilToDataTable(dataGridTable, recordupto);
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
            logger.Log(NLog.LogLevel.Info, SIMSAPI.UserType.Staff + " selected");
            Switcher.SimsApiClass.SetUserType = SIMSAPI.UserType.Staff;
            bool loadOk = importLogic;

            if (loadOk)
            {
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

        private void bwStaff_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            Switcher.SimsApiClass.CreateStaffResultTable();
            dataGridTable = Switcher.SimsApiClass.CreateStaffDataTable;

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
                    dataGridTable = Switcher.SimsApiClass.AddStaffToDataTable(dataGridTable, recordupto);
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
            //this.Close();
            Application.Current.Shutdown();
        }

        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new About());
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            logger.Log(NLog.LogLevel.Info, "Import Start");
            importStart = DateTime.Now;

            switch (Switcher.SimsApiClass.GetUserType)
            {
                case SIMSAPI.UserType.Staff:
                    StaffImport();
                    break;
                case SIMSAPI.UserType.Pupil:
                    PupilImport();
                    break;
                case SIMSAPI.UserType.Contact:
                    ContactImport();
                    break;
            }
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
                string _importSummary = "Import summary: " + Environment.NewLine +
                    "Imported - Emails: " + emailCount + Environment.NewLine +
                    "Imported - UDFs: " + udfCount + Environment.NewLine +
                    "Ignored: " + ignoreCount + Environment.NewLine +
                    "Start time: " + importStart.ToShortTimeString() + Environment.NewLine +
                    "End time: " + importEnd.ToShortTimeString() + Environment.NewLine +
                    "Time: " + DateTime.Compare(importEnd, importStart) + " seconds" + Environment.NewLine +
                    "Import per second: " + GetAverage;
                logger.Log(NLog.LogLevel.Debug, _importSummary);
                //TODO this.Hide();
                Results results = new Results(Switcher.SimsApiClass.GetResultTable, Switcher.SimsApiClass.GetUserType);
                //TODO this.Close();
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
                            Switcher.SimsApiClass.AddContactResultToTable(surname, forename, postcode, town, personId, "Email", personEmail, importResult, "");
                        }
                        if (status.Contains("UDF"))
                        {
                            udfCount = udfCount + 1;
                            string personUdf = (row["Import UDF"].ToString());
                            bool importResult = importContactUDF(pid, personUdf);
                            Switcher.SimsApiClass.AddContactResultToTable(surname, forename, postcode, town, personId, "UDF", personUdf, importResult, "");
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
                            Switcher.SimsApiClass.AddPupilResultToTable(surname, forename, gender, admis, dob, year, reg, house, personId, "Email", personEmail, importResult, "");
                        }
                        if (status.Contains("UDF"))
                        {
                            udfCount = udfCount + 1;
                            string personUdf = (row["Import UDF"].ToString());
                            bool importResult = importPupilUDF(pid, personUdf);
                            Switcher.SimsApiClass.AddPupilResultToTable(surname, forename, gender, admis, dob, year, reg, house, personId, "UDF", personUdf, importResult, "");
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
                            Switcher.SimsApiClass.AddStaffResultToTable(surname, forename, gender, staffcode, dob, personId, "Email", personEmail, importResult, "");
                        }
                        if (status.Contains("UDF"))
                        {
                            udfCount = udfCount + 1;
                            string personUdf = (row["Import UDF"].ToString());
                            bool importResult = importStaffUDF(pid, personUdf);
                            Switcher.SimsApiClass.AddStaffResultToTable(surname, forename, gender, staffcode, dob, personId, "UDF", personUdf, importResult, "");
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
            return Switcher.SimsApiClass.SetContactEmail(personid, address);
        }

        private bool importStaffEmail(int personid, string address)
        {
            if (personid == 0) { return true; }
            if (string.IsNullOrWhiteSpace(address)) { return true; }
            return Switcher.SimsApiClass.SetStaffEmail(personid, address);
        }

        private bool importPupilEmail(int personid, string address)
        {
            if (personid == 0) { return false; }
            if (string.IsNullOrWhiteSpace(address)) { return false; }
            return Switcher.SimsApiClass.SetPupilEmail(personid, address);
        }

        private bool importStaffUDF(int personid, string UDF)
        {
            if (personid == 0) { return true; }
            if (string.IsNullOrWhiteSpace(UDF)) { return true; }
            return Switcher.SimsApiClass.SetStaffUDF(personid, UDF);
        }

        private bool importPupilUDF(int personid, string UDF)
        {
            if (personid == 0) { return false; }
            if (string.IsNullOrWhiteSpace(UDF)) { return false; }
            return Switcher.SimsApiClass.SetPupilUDF(personid, UDF);
        }

        private bool importContactUDF(int personid, string UDF)
        {
            return false;
        }

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

        private void MenuItem_Click_Manual(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = "Guide.pdf";
                prc.Start();
            }
            catch (System.Exception MenuItem_Click_Manual_Exception)
            {
                logger.Log(NLog.LogLevel.Error, MenuItem_Click_Manual_Exception);
            }
        }


        private void MenuItem_Click_Options(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Options());
        }

        private void MenuItem_Click_Print(object sender, RoutedEventArgs e)
        {
            DataTable currentDt = (DataTable)dataGrid.DataContext;
            Results results = new Results(dataGridTable, Switcher.SimsApiClass.GetUserType);
        }
    }
}