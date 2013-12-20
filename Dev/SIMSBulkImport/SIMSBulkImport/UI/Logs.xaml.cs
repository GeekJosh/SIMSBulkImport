/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    /// Interaction logic for Logs.xaml
    /// </summary>
    public partial class Logs
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        
        private ICollectionView cvLog;
        private Support.View support;

        public Logs()
        {
            InitializeComponent();
            support = new Support.View();
            logDataGrid.DataContext = support.ReadLog;
        }

        private void backClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Switcher.Switch(new Menu());
        }

        private void submitClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Switcher.Switch(new Submit());
        }

        private void UserControlLoaded(object sender, RoutedEventArgs e)
        {
            IEnumerable source = logDataGrid.ItemsSource;
            cvLog = (CollectionView)CollectionViewSource.GetDefaultView(source);
            if (cvLog != null)
            {
                cvLog.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));
            }
        }

        private DataTable filterTable
        {
            get
            {
                DataTable unFiltered = support.ReadLog;
                DataTable filtered = unFiltered.Clone();

                ComboBoxItem filterLevelItem = (ComboBoxItem)filterLevel.SelectedItem;

                string levelFilter = filterLevelItem.Content.ToString();
                string messageFilter = filterMessage.Text;
                string cFilter = null;

                if (!string.IsNullOrWhiteSpace(levelFilter))
                {
                    cFilter = "(Level='" + levelFilter + "')";
                }
                if (!string.IsNullOrWhiteSpace(messageFilter))
                {
                    if (!string.IsNullOrEmpty(cFilter))
                    {
                        cFilter = "(Message LIKE '%" + messageFilter + "%')";
                    }
                    else
                    {
                        cFilter = cFilter + " AND (Message LIKE '%" + messageFilter + "%')";
                    }
                }

                logger.Log(NLog.LogLevel.Debug, cFilter);

                if (!string.IsNullOrWhiteSpace(cFilter))
                {
                    filtered.Clear();

                    DataRow[] rows = unFiltered.Select(cFilter);

                    foreach (DataRow row in rows)
                    {
                        filtered.Rows.Add(row.ItemArray);
                    }
                }

                return filtered;
            }
        }

        private void filterLevel_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            logDataGrid.DataContext = filterTable;
            CollectionViewSource.GetDefaultView(logDataGrid.ItemsSource).Refresh();
        }
    }
}
