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

namespace GingerBeard
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            this.Title = "About :: " + GetName.Title;
            this.labelTitle.Content = GetName.Title;
            this.labelDescription.Content = GetName.Description;
            this.labelCopyright.Content = GetName.Copyright;
            this.labelVersion.Content = "Version: " + GetName.Version;
        }
    }
}
