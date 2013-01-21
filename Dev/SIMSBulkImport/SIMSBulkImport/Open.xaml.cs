/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Threading;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Open : Window
    {
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private ImportFile _importFile;
        private BackgroundWorker bw = new BackgroundWorker();
        private SIMSAPI _simsApi;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        internal Open(SIMSAPI simsApi, ImportFile importFile)
        {
            _importFile = importFile;
            _simsApi = simsApi;
            InitializeComponent();

            this.pathBox.Focus();

            openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "CSV (Comma delimited)(*.csv)|*csv|XLS (Excel Workbook) (*.xls, *.xlsx)|*xls;*xlsx|XML Document (*.xml)|*xml|All Files(*.*)|*";
            openFileDialog.Title = "Import file";
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.DialogResult.OK == openFileDialog.ShowDialog())
            {
                pathBox.Text = openFileDialog.FileName;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(pathBox.Text)) { MessageBox.Show("Please select a file"); }
            else
            {
                if (!File.Exists(pathBox.Text)) { MessageBox.Show("Please select a valid file"); }
                else
                {
                    showLoad(true);
                    logger.Log(NLog.LogLevel.Info, "User selected file for import: " + pathBox.Text);
                    _importFile.SetImportFilePath = pathBox.Text;

                    bw = new BackgroundWorker();
                    bw.WorkerReportsProgress = true;
                    bw.WorkerSupportsCancellation = true;
                    bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                    //bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
                    bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

                    if (bw.IsBusy != true)
                        bw.RunWorkerAsync();
                }
            }
        }

        private void pathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (File.Exists(pathBox.Text)) { button.IsEnabled = true; } else { button.IsEnabled = false; }
        }

        private void showLoad(bool value)
        {
            if (value)
            {
                this.gridLoad.Visibility = Visibility.Visible;
                this.gridOpen.Visibility = Visibility.Hidden;
            }
            else
            {
                this.gridLoad.Visibility = Visibility.Hidden;
                this.gridOpen.Visibility = Visibility.Visible;
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                
            }
            else if (!(e.Error == null))
            {
                
            }
            else
            {
                
            }
            Close();
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string prog = e.ProgressPercentage.ToString();
            MessageBox.Show(prog);
            if (prog == "50")
                this.labelLoad.Content = "Loading UDFs...";
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            // Import the file into a dataset
            _importFile.GetImportDataSet();

            worker.ReportProgress(50);

            // Load UDFs from SIMS
            _simsApi.LoadUdfs();
        }
    }
}
