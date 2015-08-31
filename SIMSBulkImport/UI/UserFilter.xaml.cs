﻿using System;
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
            this.nextButton.Visibility = Visibility.Visible;
            this.nextButton.IsEnabled = true;
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
            this.All.Header = "All (" + allCnt + ")";

            foreach (string house in houses)
            {
                TreeViewItem newHouseChild = new TreeViewItem();
                int houseCnt = Switcher.SimsApiClass.GetPupilHierarchyItemCount("House", house);
                newHouseChild.Header = house + " (" + houseCnt.ToString() + ")";
                this.houseTree.Items.Add(newHouseChild);
            }

            foreach (string year in yearGroups)
            {
                TreeViewItem newYearChild = new TreeViewItem();
                int yearCnt = Switcher.SimsApiClass.GetPupilHierarchyItemCount("Year", year);
                newYearChild.Header = year + " (" + yearCnt.ToString() + ")";
                this.yearTree.Items.Add(newYearChild);
            }
        }

        private void backClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new UserGen());
        }

        private void nextClick(object sender, RoutedEventArgs e)
        {
            //Switcher.Switch(new UserFilter());
        }
    }
}