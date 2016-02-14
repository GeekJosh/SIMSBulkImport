using System;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using NLog;

namespace SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for ImportWindow.xaml
    /// </summary>
    public partial class ImportWindow
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private BackgroundWorker bw = new BackgroundWorker();
        private DataTable dataGridTable;
        private DateTime queryEnd;
        private DateTime queryStart;
        private int recordcount;
        private int recordupto;

        public ImportWindow()
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.ImportWindow()");
            InitializeComponent();
            load();
            dataGrid.Items.Refresh();
        }



        private void load()
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.ImportWindow.load()");
            queryStart = DateTime.Now;
            dataGridTable = Switcher.PreImportClass.CreateDataTable;

            foreach (DataColumn column in dataGridTable.Columns)
            {
                column.ReadOnly = true;
            }

            dataGrid.DataContext = dataGridTable;

            dataGrid.Items.Refresh();
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

        /// <summary>
        ///     Run once the SIMS .net database has finished being queried
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.ImportWindow.bw_RunWorkerCompleted()");
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
                button.IsEnabled = true;
                //this.MenuPrint.IsEnabled = true;
                queryEnd = DateTime.Now;
                logger.Log(LogLevel.Info,
                    "Querying complete: " + queryStart.ToShortTimeString() + " - " +
                    DateTime.Compare(queryEnd, queryStart));
                dataGrid.Items.Refresh();
                //this.dataGrid.IsReadOnly = false;
                //this.dataGrid.CanUserDeleteRows = true;
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.ImportWindow.bw_ProgressChanged()");
            Status.Content = "Querying SIMS database - " + e.ProgressPercentage + "%";
            // Refreshing the dataGrid causes a crash in .net 4.5, so don't refresh. Version number for 4.5 
            // is actually 4.0.30319.17626 for some reason
            // http://stackoverflow.com/questions/12971881/how-to-reliably-detect-the-actual-net-4-5-version-installed
            Version dotNetVersion = System.Environment.Version;
            if (dotNetVersion.Major <= 4 && dotNetVersion.Minor <= 0 && dotNetVersion.Build <= 30319 && dotNetVersion.Revision < 17626)
            {
                dataGrid.DataContext = dataGridTable;
                dataGrid.Items.Refresh();
            }   
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.ImportWindow.bw_DoWork()");
            var worker = sender as BackgroundWorker;
            recordcount = Switcher.PreImportClass.GetImportFileRecordCount;
            recordupto = 0;
            while (recordupto < recordcount)
            {
                logger.Log(LogLevel.Debug, " -- " + recordupto);
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    logger.Log(LogLevel.Debug, "Kill process received");
                    break;
                }
                logger.Log(LogLevel.Debug, "ImportAddToDataTable: " + recordupto);
                dataGridTable = Switcher.PreImportClass.AddToDataTable(recordupto);
                recordupto++;

                long lonCount = recordupto;
                long lonTotal = recordcount;
                int percent = Convert.ToInt32(lonCount*100/lonTotal);

                worker.ReportProgress(percent);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.ImportWindow.button_Click()");
            Switcher.ImportListClass = new ImportList();
            foreach (DataRow row in dataGridTable.Rows)
            {
                Switcher.ImportListClass.AddToList(row);
            }
            Switcher.Switch(new Importing());
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.ImportWindow.Grid_KeyDown()");
            if (e.Key == Key.Delete)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
