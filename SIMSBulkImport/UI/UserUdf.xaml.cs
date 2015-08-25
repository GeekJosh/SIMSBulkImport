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
            Switcher.Switch(new UserGen());
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
            SetEnableNewUdf = IsCreateNew;
            SetOKButtonEnable = IsValidSelection;
        }

        private string tmpUdfValue = "";

        private bool SetEnableNewUdf
        {
            set
            {
                if (value)
                {
                    string curUdfValue = this.udfNew.Text;
                    if (string.IsNullOrEmpty(curUdfValue))
                        if (!string.IsNullOrEmpty(tmpUdfValue))
                            this.udfNew.Text = tmpUdfValue;
                    this.udfNew.IsEnabled = true;
                    this.BorderThickness = new Thickness(1);
                }
                else
                {
                    string curUdfValue = this.udfNew.Text;
                    if (!string.IsNullOrEmpty(curUdfValue))
                        tmpUdfValue = curUdfValue;
                    this.udfNew.Text = "";
                    this.udfNew.IsEnabled = false;
                    this.BorderThickness = new Thickness(0);
                }
            }
        }

        private void udfNew_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsCreateNew)
            {
                if (IsValidNewName)
                {
                    SetOKButtonEnable = true;
                    this.udfNew.BorderBrush = Brushes.Green;
                }
                else
                {
                    SetOKButtonEnable = false;
                    this.udfNew.BorderBrush = Brushes.Red;
                }
            }
            else
                this.udfNew.ClearValue(TextBox.BorderBrushProperty);
        }

        private bool IsValidNewName
        {
            get
            {
                string value = this.udfNew.Text;
                if (string.IsNullOrWhiteSpace(value))
                    return false;
                foreach (string udf in udfs)
                {
                    if (udf.ToUpper() == value.ToUpper())
                        return false;
                }
                return true;
            }
        }

        private bool IsValidSelection
        {
            get
            {
                if (IsCreateNew)
                {
                    return IsValidNewName;
                }
                return true;
            }
        }

        private bool IsCreateNew
        {
            get
            {
                string dropdown = this.udfSelection.SelectedValue.ToString();
                if (dropdown == "[ Create New UDF ]")
                    return true;
                return false;
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
                udfs.Add("[ Create New UDF ]");
                this.gridLoad.Visibility = Visibility.Hidden;
                this.gridMain.Visibility = Visibility.Visible;
                AddExistsUdfs();
            }
        }
    }
}