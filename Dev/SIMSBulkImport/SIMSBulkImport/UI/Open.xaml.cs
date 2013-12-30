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
    public partial class Open
    {
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private BackgroundWorker bwOpen = new BackgroundWorker();
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        internal Open()
        {
            InitializeComponent();

            getTitle();
            this.pathBox.Focus();

            openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "CSV (Comma delimited)(*.csv)|*csv|XLS (Excel Workbook) (*.xls, *.xlsx)|*xls;*xlsx|XML Document (*.xml)|*xml|All Files(*.*)|*";
            openFileDialog.Title = "Import file";
        }

        private void getTitle()
        {
            Interfaces.UserType userType = Switcher.PreImportClass.GetUserType;
            switch (userType)
            {
                case Interfaces.UserType.Staff:
                    this.title.Content = "Staff";
                    break;
                case Interfaces.UserType.Pupil:
                    this.title.Content = "Pupil";
                    break;
                case Interfaces.UserType.Contact:
                    this.title.Content = "Contact";
                    break;
            }
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
            if (string.IsNullOrWhiteSpace(this.pathBox.Text)) { MessageBox.Show("Please select a file"); }
            else
            {
                if (!File.Exists(this.pathBox.Text)) { MessageBox.Show("Please select a valid file"); }
                else
                {
                    showLoad(true);
                    logger.Log(NLog.LogLevel.Info, "User selected file for import: " + this.pathBox.Text);
                    Switcher.ImportFileClass.SetImportFilePath = this.pathBox.Text;

                    bwOpen = new BackgroundWorker();
                    bwOpen.WorkerReportsProgress = true;
                    bwOpen.WorkerSupportsCancellation = true;
                    bwOpen.DoWork += new DoWorkEventHandler(bwOpen_DoWork);
                    bwOpen.ProgressChanged += new ProgressChangedEventHandler(bwOpen_ProgressChanged);
                    bwOpen.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwOpen_RunWorkerCompleted);

                    if (bwOpen.IsBusy != true)
                        bwOpen.RunWorkerAsync();
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
                this.title.Visibility = Visibility.Hidden;
                this.backButton.Visibility = Visibility.Hidden;
            }
            else
            {
                this.gridLoad.Visibility = Visibility.Hidden;
                this.gridOpen.Visibility = Visibility.Visible;
                this.title.Visibility = Visibility.Visible;
                this.backButton.Visibility = Visibility.Visible;
            }
        }

        private void bwOpen_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                //Switcher.ImportFileClass.ImportCompleted = false;
            }
            else if (!(e.Error == null))
            {
                //Switcher.ImportFileClass.ImportCompleted = false;
            }
            else
            {
                Switcher.Switch(new Match());
            }  
        }

        private void bwOpen_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string prog = e.ProgressPercentage.ToString();
            if (prog == "50")
                this.labelLoad.Content = "Loading UDFs...";
        }

        private void bwOpen_DoWork(object sender, DoWorkEventArgs e)
        {
            bwOpen = sender as BackgroundWorker;

            // Import the file into a dataset
            Switcher.ImportFileClass.GetImportDataSet();

            bwOpen.ReportProgress(50);

            // Load UDFs from SIMS
            Switcher.SimsApiClass.LoadUdfs();
        }

        private void backClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Switcher.Switch(new Menu());
        }
    }
}
