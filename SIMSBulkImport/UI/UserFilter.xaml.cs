using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro;
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
            Switcher.SimsApiClass.GetPupilHierarchy();
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
                this.mainGrid.Visibility = Visibility.Visible;
                AddToHierarchy();
            }
        }

        private void AddToHierarchy()
        {
            int allCnt = Switcher.SimsApiClass.GetPupilHierarchyAllCount;
            int allFilledCnt = Switcher.SimsApiClass.GetPupilHierarchyAllNotCompletedCount;
            this.All.Header = "All (" + allFilledCnt + " / " + allCnt + ")";
            this.All.IsSelected = true;

            foreach (string house in houses)
            {
                TreeViewItem newHouseChild = new TreeViewItem();
                int houseCnt = Switcher.SimsApiClass.GetPupilHierarchyItemCount("House", house);
                int houseFilledCnt = Switcher.SimsApiClass.GetPupilHierarchyItemNotCompletedCount("House", house);
                newHouseChild.Header = house + " (" + houseFilledCnt + " / " + houseCnt.ToString() + ")";
                this.houseTree.Items.Add(newHouseChild);
            }

            foreach (string year in yearGroups)
            {
                TreeViewItem newYearChild = new TreeViewItem();
                int yearCnt = Switcher.SimsApiClass.GetPupilHierarchyItemCount("Year", year);
                int yearFilledCnt = Switcher.SimsApiClass.GetPupilHierarchyItemNotCompletedCount("Year", year);
                newYearChild.Header = year + " (" + yearFilledCnt + " / " + yearCnt.ToString() + ")";
                this.yearTree.Items.Add(newYearChild);
            }
        }

        private void backClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new UserGen());
        }

        private void nextClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new UserSet());
        }

        private void PupilHierarchySelectedChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string parentName = "All";
            TreeViewItem item = pupilHierarchy.SelectedItem as TreeViewItem;
            TreeViewItem parentItem = item.Parent as TreeViewItem;
            if (parentItem != null)
                parentName = parentItem.Header.ToString();

            string selectedItem = item.Header.ToString();
            DisableNextButton = (selectedItem == "Years" || selectedItem == "Houses");
            Classes.Core.SetUsernameFilter(parentName, selectedItem);
        }

        private bool DisableNextButton
        {
            set
            {
                Visibility vis = Visibility.Visible;
                if (value)
                    vis = Visibility.Hidden;
                this.nextButton.IsEnabled = !value;
                this.nextButton.Visibility = vis;
            }
        }
    }
}