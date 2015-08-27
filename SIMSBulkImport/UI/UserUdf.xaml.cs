/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NLog;
using MessageBox = System.Windows.MessageBox;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for UserUdf.xaml
    /// </summary>
    public partial class UserUdf
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private List<string> udfs;
        private BackgroundWorker bwLoadUdfs = new BackgroundWorker();
        DataTable defaultUserData;

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
            Switcher.Switch(new UserGen(defaultUserData, yearGroups));
        }

        private void AddExistsUdfs()
        {
            if (udfs != null)
            {
                foreach (string udf in udfs)
                {
                    this.udfSelection.Items.Add(udf);
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

        private string[] yearGroups;
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
                //Switcher.ImportFileClass.ImportCompleted = false;
            }
            else if (!(e.Error == null))
            {
                //Switcher.ImportFileClass.ImportCompleted = false;
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