/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using NLog;
using MessageBox = System.Windows.MessageBox;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Open
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly OpenFileDialog openFileDialog;
        private BackgroundWorker bwOpen = new BackgroundWorker();

        internal Open()
        {
            InitializeComponent();

            getTitle();
            pathBox.Focus();

            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter =
                "XLS (Excel Workbook) (*.xls, *.xlsx)|*xls;*xlsx|CSV (Comma delimited)(*.csv)|*csv|XML Document (*.xml)|*xml|All Files(*.*)|*";
            openFileDialog.Title = "Import file";
        }

        private void getTitle()
        {
            Interfaces.UserType userType = Switcher.PreImportClass.GetUserType;
            switch (userType)
            {
                case Interfaces.UserType.Staff:
                    title.Content = "Staff";
                    break;
                case Interfaces.UserType.Pupil:
                    title.Content = "Pupil";
                    break;
                case Interfaces.UserType.Contact:
                    title.Content = "Contact";
                    break;
            }
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            if (DialogResult.OK == openFileDialog.ShowDialog())
            {
                pathBox.Text = openFileDialog.FileName;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(pathBox.Text))
            {
                MessageBox.Show("Please select a file");
            }
            else
            {
                if (!File.Exists(pathBox.Text))
                {
                    MessageBox.Show("Please select a valid file");
                }
                else
                {
                    showLoad(true);
                    logger.Log(LogLevel.Info, "User selected file for import: " + pathBox.Text);
                    Switcher.ImportFileClass.SetImportFilePath = pathBox.Text;

                    bwOpen = new BackgroundWorker();
                    bwOpen.WorkerReportsProgress = true;
                    bwOpen.WorkerSupportsCancellation = true;
                    bwOpen.DoWork += bwOpen_DoWork;
                    bwOpen.ProgressChanged += bwOpen_ProgressChanged;
                    bwOpen.RunWorkerCompleted += bwOpen_RunWorkerCompleted;

                    if (bwOpen.IsBusy != true)
                        bwOpen.RunWorkerAsync();
                }
            }
        }

        private void pathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (File.Exists(pathBox.Text))
            {
                button.IsEnabled = true;
            }
            else
            {
                button.IsEnabled = false;
            }
        }

        private void showLoad(bool value)
        {
            if (value)
            {
                gridLoad.Visibility = Visibility.Visible;
                gridOpen.Visibility = Visibility.Hidden;
                title.Visibility = Visibility.Hidden;
                backButton.Visibility = Visibility.Hidden;
            }
            else
            {
                gridLoad.Visibility = Visibility.Hidden;
                gridOpen.Visibility = Visibility.Visible;
                title.Visibility = Visibility.Visible;
                backButton.Visibility = Visibility.Visible;
            }
        }

        private void bwOpen_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
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
                labelLoad.Content = "Loading UDFs...";
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

        private void backClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Menu());
        }
    }
}