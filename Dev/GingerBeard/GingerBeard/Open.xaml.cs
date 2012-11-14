using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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

namespace GingerBeard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Open : Window
    {
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private SIMSAPI SimsApi;

        public Open(SIMSAPI simsApi)
        {
            SimsApi = simsApi;
            InitializeComponent();

            this.pathBox.Focus();
            this.Title = "Open :: " + GetName.Title;

            openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "CSV (Comma delimited)(*.csv)|*csv|XLS (Excel Workbook) (*.xls)|*xls|XML Document (*.xml)|*xml|All Files(*.*)|*";
            openFileDialog.Title = "Staff import";
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.DialogResult.OK == openFileDialog.ShowDialog())
            {
                pathBox.Text = openFileDialog.FileName;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(pathBox.Text)) { MessageBox.Show("Please select a file"); }
            else
            {
                if (!File.Exists(pathBox.Text)) { MessageBox.Show("Please select a valid file"); }
                else {
                    SimsApi.SetImportFile = pathBox.Text;
                    this.Close();
                }
            }
        }

        private void pathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (File.Exists(pathBox.Text)) { button.IsEnabled = true; } else { button.IsEnabled = false; }
        }
    }
}
