/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

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
            foreach (string udf in udfs)
            {
                this.udfSelection.Items.Add(udf);
            }
        }

        private List<string> udfs = new List<string>();

        private void GetUdfs()
        {
            udfs = Switcher.SimsApiClass.GetPupilUsernameUDFs;
        }

        private void udfSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsNewUdf)
                this.udfNew.Text = "New";
            else
                this.udfNew.Text = "";
        }

        private bool IsNewUdf
        {
            get
            {
                string value = null;
                try
                {
                    value = this.udfSelection.SelectedValue.ToString();
                    SetOKButtonEnable = true;
                }
                catch
                {
                    SetOKButtonEnable = false;
                }
                return !udfs.Contains(value);
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