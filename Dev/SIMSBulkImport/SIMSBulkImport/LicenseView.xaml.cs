/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    /// Interaction logic for License.xaml
    /// </summary>
    public partial class LicenseView : Window
    {
        private License license;

        public LicenseView()
        {
            license = new License();
            InitializeComponent();
            this.Title = "License - " + GetExe.Title;
            this.labelTitle.Content = GetExe.Title;
            this.labelType.Content = "License type: " + license.GetLicenseType;
            this.labelDfe.Content = "DfE code: " + license.GetDfE;
            this.labelName.Content = "Name: " + license.GetName;
            this.textboxKey.Text = license.GetKey;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
           bool result = license.SetKey(this.textboxKey.Text);
           if (result)
           { 
               this.labelType.Content = "License type: " + license.GetLicenseType;
               this.labelDfe.Content = "DfE code: " + license.GetDfE;
               this.labelName.Content = "Name: " + license.GetName;
               MessageBox.Show("License key ok");
               this.Close();
           }
           else
           {
               this.textboxKey.Text = license.GetKey;
               MessageBox.Show("Invalid License key entered!");
           }
        }

        private void textboxKey_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.textboxKey.Text == license.GetKey) { this.buttonSave.IsEnabled = false; }
            else { this.buttonSave.IsEnabled = true; }
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
