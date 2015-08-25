/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
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

        internal UserUdf()
        {
            InitializeComponent();
            GetUdfs();
            AddExistsUdfs();
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
            udfs = Switcher.SimsApiClass.GetPupilUsernameUDFs;
        }

        private void udfSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetEnableNewUdf = IsCreateNew;
            SetOKButtonEnable = true;
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
                }
                else
                {
                    string curUdfValue = this.udfNew.Text;
                    if (!string.IsNullOrEmpty(curUdfValue))
                        tmpUdfValue = curUdfValue;
                    this.udfNew.Text = "";
                    this.udfNew.IsEnabled = false;
                }
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
    }
}