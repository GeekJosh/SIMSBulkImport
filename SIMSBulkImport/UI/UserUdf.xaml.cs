using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using NLog;
using UserGen;

namespace SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for UserUdf.xaml
    /// </summary>
    public partial class UserUdf
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private List<string> udfs;
        private BackgroundWorker bwLoadUdfs = new BackgroundWorker();
        private DataTable defaultUserData;
        private string[] yearGroups;


        internal UserUdf()
        {
            InitializeComponent();
            GetUdfs();
        }

        private void backClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Menu());
        }

        private void okClick(object sender, RoutedEventArgs e)
        {
            Switcher.UserGenClass.SetSchoolYearGroups = yearGroups;
            Switcher.UserGenClass.SetYearStart = "2015-09-01";
            Switcher.UserGenClass.SetSchoolYearGroups = yearGroups;
            Switcher.UserGenClass.SetDefaultUserData = defaultUserData;
            Switcher.SimsApiClass.SetPupilSIMSUDF = (string) this.udfSelection.SelectedValue;
            Switcher.Switch(new UserGen());
        }

        private void AddExistsUdfs()
        {
            if (udfs != null)
            {
                if (udfs.Count > 0)
                {
                    foreach (string udf in udfs)
                    {
                        this.udfSelection.Items.Add(udf);
                    }
                }
                else
                {
                    this.udfSelection.Visibility = Visibility.Hidden;
                    this.noneLabel1.Visibility = Visibility.Visible;
                    this.noneLabel2.Visibility = Visibility.Visible;
                    // TODO - Enable OK button, but change text to launch SIMS .net  - Pulsar.exe /START:"Tools|Setups|User Defined Fields"
                }
            }
        }

        private void GetUdfs()
        {
            bwLoadUdfs = new BackgroundWorker();
            bwLoadUdfs.WorkerReportsProgress = true;
            bwLoadUdfs.WorkerSupportsCancellation = true;
            bwLoadUdfs.DoWork += bwLoadUdfs_DoWork;
            bwLoadUdfs.RunWorkerCompleted += bwLoadUdfs_RunWorkerCompleted;

            if (bwLoadUdfs.IsBusy != true)
                bwLoadUdfs.RunWorkerAsync();         
        }

        private void udfSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetOKButtonEnable = IsValidSelection;
        }

        private bool IsValidSelection
        {
            get
            {
                return !string.IsNullOrEmpty((string)this.udfSelection.SelectedValue);
            }
        }

        private bool SetOKButtonEnable
        {
            set
            {
                this.okButton.IsEnabled = value;
                if (value)
                    this.okButton.Visibility = Visibility.Visible;
                else
                    this.okButton.Visibility = Visibility.Hidden;
            }
        }

        private void bwLoadUdfs_DoWork(object sender, DoWorkEventArgs e)
        {
            bwLoadUdfs = sender as BackgroundWorker;
            udfs = Switcher.SimsApiClass.GetPupilUsernameUDFs;
            defaultUserData = Switcher.SimsApiClass.GetPupilDefaultUsernameData;
            yearGroups = Switcher.SimsApiClass.GetPupilYearGroups;

        }

        private void bwLoadUdfs_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {

            }
            else if (!(e.Error == null))
            {

            }
            else
            {
                this.gridLoad.Visibility = Visibility.Hidden;
                this.gridMain.Visibility = Visibility.Visible;
                AddExistsUdfs();
            }
        }
    }
}