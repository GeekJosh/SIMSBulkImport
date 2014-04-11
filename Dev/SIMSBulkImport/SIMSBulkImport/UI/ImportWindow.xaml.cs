/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for ImportWindow.xaml
    /// </summary>
    public partial class ImportWindow
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private BackgroundWorker bw = new BackgroundWorker();
        private DataTable dataGridTable;
        private int ignoreCount;
        private DateTime queryEnd;
        private DateTime queryStart;
        private int recordcount;
        private int recordupto;

        public ImportWindow()
        {
            InitializeComponent();

            //TEST:: load import data straight into datagrid
            //this.dataGrid.DataContext = Switcher.PreImportClass.GetImportDataTable;

            load();
            dataGrid.Items.Refresh();
        }



        private void load()
        {
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
            Status.Content = "Querying SIMS database - " + e.ProgressPercentage + "%";
            dataGrid.DataContext = dataGridTable;
            dataGrid.Items.Refresh();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
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
            Switcher.ImportListClass = new ImportList();
            foreach (DataRow row in dataGridTable.Rows)
            {
                Switcher.ImportListClass.AddToList(row);
            }
            Switcher.Switch(new Importing());
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                deleteRow();
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void button_ClickDelete(object sender, RoutedEventArgs e)
        {
            deleteRow();
        }

        private void deleteRow()
        {
            // TODO
        }
    }
}