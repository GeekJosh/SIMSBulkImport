using System;
using System.Collections.Generic;
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

namespace GingerBeard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SIMSAPI simsApi;
        private DataTable dataGridTable;
        private int importType;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
            this.Title = GetName.Title;

            SimsIni simsIni = new SimsIni();
            simsApi = new SIMSAPI(simsIni.GetSimsDir);
        }

        private bool GetConnection
        {
            get
            {
                try
                {
                    // Blank it ready for use
                    simsApi.Reset();

                    this.Status.Content = "Not Connected";
                    this.dataGrid.DataContext = null;
                    this.dataGrid.Items.Refresh();

                    Logon logon = new Logon(simsApi);
                    logon.ShowDialog();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(simsApi.GetCurrentSchool))
                {
                    this.Status.Content = "Connected to: " + simsApi.GetCurrentSchool;
                    logger.Log(NLog.LogLevel.Info, "Connected to: " + simsApi.GetCurrentSchool);

                    Open open = new Open(simsApi);
                    open.ShowDialog();
                }

                if (!string.IsNullOrWhiteSpace(simsApi.GetImportFile))
                {
                    return true;
                }
                return false;
            }
        }

        private void MenuItem_Click_New_Contact(object sender, RoutedEventArgs e)
        {
            if (GetConnection)
            {
                importType = 3;

                Match match = new Match(simsApi, importType);
                match.ShowDialog();

                if (simsApi.GetMatched)
                {
                    dataGridTable = simsApi.FillContactDataTable;
                    this.dataGrid.DataContext = dataGridTable;
                    this.dataGrid.Items.Refresh();

                    MessageBox.Show("Loaded");

                    this.button.IsEnabled = true;
                }
            }
        }

        private void MenuItem_Click_New_Pupil(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            if (GetConnection)
            {
                importType = 2;

                Match match = new Match(simsApi, importType);
                match.ShowDialog();

                if (simsApi.GetMatched)
                {
                    dataGridTable = simsApi.FillPupilDataTable;

                    this.dataGrid.DataContext = dataGridTable;
                    this.dataGrid.Items.Refresh();

                    this.button.IsEnabled = true;
                }
            }
        }

        private void MenuItem_Click_New_Staff(object sender, RoutedEventArgs e)
        {
            if (GetConnection)
            {
                //ImportFromFile importFromFile = new ImportFromFile();
                //simsApi.SetImportDataset = importFromFile.GetDataSetFromFile(simsApi.GetImportFile);

                importType = 1;

                simsApi.WriteDmPayment();

                Match match = new Match(simsApi, importType);
                match.ShowDialog();

                if (simsApi.GetMatched)
                {
                    dataGridTable = simsApi.FillStaffDataTable;

                    this.dataGrid.DataContext = dataGridTable;
                    this.dataGrid.Items.Refresh();

                    this.button.IsEnabled = true;
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
            if (simsApi.GetIsLicensed)
            {
                switch (importType)
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
                simsApi.Reset();
                this.Status.Content = "Disconnected";
                this.button.IsEnabled = false;
                this.dataGrid.DataContext = null;
                logger.Log(NLog.LogLevel.Info, "Import complete");
                MessageBox.Show("Import complete");
            }
            else
            {
                logger.Log(NLog.LogLevel.Error, "Unlicensed");
                MessageBox.Show("You are not licensed to use GingerBeard", "Unlicensed", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void ContactImport()
        {
            foreach (DataRow row in dataGridTable.Rows)
            {
                string personName = (row["Forename"].ToString()) + " " + (row["Surname"].ToString());
                string personId = (row["PersonID"].ToString());
                string status = (row["Status"].ToString());
                if (status.StartsWith("Import"))
                {
                    int pid = Convert.ToInt32(personId);
                    if (status.Contains("email"))
                    {
                        string personEmail = (row["Import email"].ToString());
                        logger.Log(NLog.LogLevel.Info, "Importing: " + personEmail);
                        importContactEmail(pid, personEmail);
                    }
                }
            }
        }

        private void PupilImport()
        {
            foreach (DataRow row in dataGridTable.Rows)
            {
                
            }
        }

        private void StaffImport()
        {
                foreach (DataRow row in dataGridTable.Rows)
                {
                    string personName = (row["Forename"].ToString()) + " " + (row["Surname"].ToString());
                    string personId = (row["PersonID"].ToString());
                    string status = (row["Status"].ToString());
                    if (status.StartsWith("Import"))
                    {
                        int pid = Convert.ToInt32(personId);
                        if (status.Contains("email"))
                        {
                            string personEmail = (row["Import email"].ToString());
                            
                            //MessageBox.Show("Importing: " + personEmail);
                            //this.Status.Content = "Importing: " + personEmail;
                            logger.Log(NLog.LogLevel.Info, "Importing: " + personEmail);
                            importStaffEmail(pid, personEmail);
                        }
                        if (status.Contains("UDF"))
                        {
                            string personUdf = (row["Import UDF"].ToString());
                            this.Status.Content = "Importing: " + personUdf;
                            logger.Log(NLog.LogLevel.Info, "Importing: " + personUdf);
                            importStaffUDF(pid, personUdf);
                        }
                    }
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

        private bool importStaffUDF(int personid, string UDF)
        {
            if (personid == 0) { return true; }
            if (string.IsNullOrWhiteSpace(UDF)) { return true; }
            return simsApi.SetPersonUDF(personid, UDF);
        }

        private void MenuItem_Click_License(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(simsApi.GetLicense);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string tmpFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Connect.ini");
            if (System.IO.File.Exists(tmpFile)) {
                try
                {
                    System.IO.File.Delete(tmpFile);
                }
                catch (System.Exception)
                {
                   
                }
            }
        }
    }
}