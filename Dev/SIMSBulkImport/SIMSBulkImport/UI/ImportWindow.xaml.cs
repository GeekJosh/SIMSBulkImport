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
        private DataTable dataGridTable;
        private DateTime importStart;
        private DateTime importEnd;
        private DateTime queryStart;
        private DateTime queryEnd;
        private int ignoreCount;
        private int recordcount;
        private int recordupto;

        public ImportWindow()
        {
            InitializeComponent();
           
            //TEST:: load import data straight into datagrid
            //this.dataGrid.DataContext = Switcher.PreImportClass.GetImportDataTable;
            
            //TEST:: load test data
            //test();

            load();
            
            this.dataGrid.Items.Refresh();
        }

        private void test()
        {
            DataTable staffTable = new DataTable();
            staffTable.Columns.Add(new DataColumn("Status", typeof(string)));
            staffTable.Columns.Add(new DataColumn("Surname", typeof(string)));
            staffTable.Columns.Add(new DataColumn("Forename", typeof(string)));
            staffTable.Columns.Add(new DataColumn("Title", typeof(string)));
            staffTable.Columns.Add(new DataColumn("Gender", typeof(string)));
            staffTable.Columns.Add(new DataColumn("Staff Code", typeof(string)));
            staffTable.Columns.Add(new DataColumn("Date of Birth", typeof(string)));
            staffTable.Columns.Add(new DataColumn("Import email", typeof(string)));
            staffTable.Columns.Add(new DataColumn("Import telephone", typeof(string)));
            staffTable.Columns.Add(new DataColumn("Import UDF", typeof(string)));
            staffTable.Columns.Add(new DataColumn("SIMS email addresses", typeof(string)));
            staffTable.Columns.Add(new DataColumn("SIMS telephone", typeof(string)));
            staffTable.Columns.Add(new DataColumn("SIMS UDF", typeof(string)));
            staffTable.Columns.Add(new DataColumn("PersonID", typeof(string)));

            DataRow newrow = staffTable.NewRow();
            newrow["Surname"] = "smith";
            newrow["Forename"] = "matthew";
            newrow["Title"] = "mr";
            newrow["Gender"] = "male";
            newrow["Staff Code"] = null;
            newrow["Date of Birth"] = null;
            newrow["Import email"] = null;
            newrow["Import telephone"] = null;
            newrow["Import UDF"] = null;
            newrow["Status"] = null;
            newrow["SIMS email addresses"] = null;
            newrow["SIMS telephone"] = null;
            newrow["SIMS UDF"] = null;
            newrow["PersonID"] = null;
            staffTable.Rows.Add(newrow);

            this.dataGrid.DataContext = staffTable;

            // http://blogs.msdn.com/b/jaimer/archive/2009/01/20/styling-microsoft-s-wpf-datagrid.aspx
        }

        private void load()
        {
            dataGridTable = Switcher.PreImportClass.CreateDataTable;
            this.dataGrid.DataContext = dataGridTable;
            this.dataGrid.Items.Refresh();
            this.Status.Content = "Querying SIMS database - 0%";
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
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
                this.button.IsEnabled = true;
                //this.MenuPrint.IsEnabled = true;
                queryEnd = DateTime.Now;
                logger.Log(NLog.LogLevel.Info, "Querying complete: " + queryStart.ToShortTimeString() + " - " + DateTime.Compare(queryEnd, queryStart));
                this.dataGrid.Items.Refresh();
                //this.dataGrid.IsReadOnly = false;
                //this.dataGrid.CanUserDeleteRows = true;
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Status.Content = "Querying SIMS database - " + e.ProgressPercentage.ToString() + "%";
            this.dataGrid.DataContext = dataGridTable;
            this.dataGrid.Items.Refresh();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            recordcount = Switcher.PreImportClass.GetImportFileRecordCount;
            recordupto = 0;
            while (recordupto < recordcount)
            {
                logger.Log(NLog.LogLevel.Debug, " -- " + recordupto);
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    logger.Log(NLog.LogLevel.Debug, "Kill process received");
                    break;
                }
                else
                {
                    logger.Log(NLog.LogLevel.Debug, "ImportAddToDataTable: " + recordupto);
                    dataGridTable = Switcher.PreImportClass.AddToDataTable(recordupto);
                    recordupto++;

                    long lonCount = recordupto;
                    long lonTotal = recordcount;
                    int percent = Convert.ToInt32(lonCount * 100 / lonTotal);

                    worker.ReportProgress(percent);
                }
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            logger.Log(NLog.LogLevel.Info, "Import Start");
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                deleteRow();
                e.Handled = true;
            }
        }

        private void button_ClickDelete(object sender, RoutedEventArgs e)
        {
            deleteRow();
        }

        private void deleteRow()
        {
            MessageBox.Show("delete pressed");
        }
    }
}
