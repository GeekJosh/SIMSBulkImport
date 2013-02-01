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
    /// Interaction logic for ImportWindow.xaml
    /// </summary>
    public partial class ImportWindow
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private BackgroundWorker bw = new BackgroundWorker();
        private BackgroundWorker bwImport = new BackgroundWorker();
        private DataTable dataGridTable;
        private DateTime importStart;
        private DateTime importEnd;
        private DateTime queryStart;
        private DateTime queryEnd;
        private int ignoreCount;
        private int recordcount;
        private int recordupto = 0;

        public ImportWindow()
        {
            InitializeComponent();
            loadContact();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            logger.Log(NLog.LogLevel.Info, "Import Start");
            
            importStart = DateTime.Now;

            switch (Switcher.SimsApiClass.GetUserType)
            {
                case SIMSAPI.UserType.Staff:
                    //StaffImport();
                    break;
                case SIMSAPI.UserType.Pupil:
                    //PupilImport();
                    break;
                case SIMSAPI.UserType.Contact:
                    //ContactImport();
                    break;
            }
        }

        private void loadContact()
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
                //this.Status.Content = "Disconnected";
                logger.Log(NLog.LogLevel.Info, "User cancelled");
            }
            else if (!(e.Error == null))
            {
                logger.Log(NLog.LogLevel.Error, "Error - " + e.Error.Message);
            }
            else
            {
                //this.Status.Content = "";
                this.button.Visibility = Visibility.Visible;
                this.button.IsEnabled = true;
                //this.MenuPrint.IsEnabled = true;
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

        private void bwContact_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            MessageBox.Show("Test");
            Switcher.SimsApiClass.CreateContactResultTable();
            dataGridTable = Switcher.SimsApiClass.CreateContactDataTable;

            while (recordupto < recordcount)
            {
                MessageBox.Show("Test");
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
    }
}
