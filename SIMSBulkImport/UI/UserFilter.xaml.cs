using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for UserFilter.xaml
    /// </summary>
    public partial class UserFilter
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private BackgroundWorker bwLoadHierarchy = new BackgroundWorker();

        private string[] yearGroups;
        private string[] houses;

        internal UserFilter()
        {
            InitializeComponent();
            GetHierarchy();
        }

        private void GetHierarchy()
        {
            bwLoadHierarchy = new BackgroundWorker();
            bwLoadHierarchy.WorkerReportsProgress = true;
            bwLoadHierarchy.WorkerSupportsCancellation = true;
            bwLoadHierarchy.DoWork += bwLoadHierarchy_DoWork;
            bwLoadHierarchy.RunWorkerCompleted += bwLoadHierarchy_RunWorkerCompleted;

            if (bwLoadHierarchy.IsBusy != true)
                bwLoadHierarchy.RunWorkerAsync();   
        }

        private void bwLoadHierarchy_DoWork(object sender, DoWorkEventArgs e)
        {
            bwLoadHierarchy = sender as BackgroundWorker;
            yearGroups = Switcher.SimsApiClass.GetPupilYearGroups;
            houses = Switcher.SimsApiClass.GetPupilHouses;
        }

        private void bwLoadHierarchy_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                
            }
            else if (!(e.Error == null))
            {
                
            }
            else
            {
                this.loadGrid.Visibility = Visibility.Hidden;
                this.pupilHierarchy.Visibility = Visibility.Visible;
                AddToHierarchy();
            }
        }

        private void AddToHierarchy()
        {
            foreach (string house in houses)
            {
                TreeViewItem newHouseChild = new TreeViewItem();
                newHouseChild.Header = house;
                this.houseTree.Items.Add(newHouseChild);
            }

            foreach (string year in yearGroups)
            {
                TreeViewItem newYearChild = new TreeViewItem();
                newYearChild.Header = year;
                this.yearTree.Items.Add(newYearChild);
            }
        }
    }
}