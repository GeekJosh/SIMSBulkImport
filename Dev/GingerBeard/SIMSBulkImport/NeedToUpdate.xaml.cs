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
using System.Windows.Shapes;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    /// Interaction logic for NeedToUpdate.xaml
    /// </summary>
    public partial class NeedToUpdate : Window
    {
        public NeedToUpdate()
        {
            InitializeComponent();
            ShowUpdatePage();
        }

        private void ShowUpdatePage()
        {
            string outOfDate = ConfigMan.UpdateUrl + "outofdate.html";
            this.webBrowser.Navigate(outOfDate);
        }
    }
}
