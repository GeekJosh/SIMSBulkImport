﻿using System;
using System.Windows;
using System.Windows.Controls;
using NLog;

namespace SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for PageSwitcher.xaml
    /// </summary>
    public partial class PageSwitcher
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public PageSwitcher()
        {
            LoadMetroUI();

            InitializeComponent();
            Switcher.pageSwitcher = this;
            Switcher.Switch(new Login());
        }

        public void Navigate(UserControl nextPage)
        {
            Content = nextPage;
        }

        public void LoadMetroUI()
        {
            
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml", UriKind.RelativeOrAbsolute)
            });

            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml", UriKind.RelativeOrAbsolute)
            });

            
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source =
                    new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml",
                        UriKind.RelativeOrAbsolute)
            });


            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source =
                    new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml",
                        UriKind.RelativeOrAbsolute)
            });


            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                // Dark - BaseDark
                Source =
                    new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml",
                        UriKind.RelativeOrAbsolute)
            });
        }

        public void Navigate(UserControl nextPage, object state)
        {
            Content = nextPage;
            var s = nextPage as ISwitchable;

            if (s != null)
                s.UtilizeState(state);
            else
                throw new ArgumentException("NextPage is not ISwitchable! "
                                            + nextPage.Name);
        }
    }
}