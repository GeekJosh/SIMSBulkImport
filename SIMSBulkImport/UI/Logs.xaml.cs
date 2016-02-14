﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SIMSBulkImport.Support;
using NLog;

namespace SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for Logs.xaml
    /// </summary>
    public partial class Logs
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private View support;
        private ICollectionView cvLog;
        private string logType = "support";
        private bool firstLoad = true;

        public Logs()
        {
            support = new View();
            InitializeComponent();
            firstLoad = false;
            LoadLog();
        }

        private void LoadLog()
        {
            if (!firstLoad)
            {
                logDataGrid.DataContext = null;
                support.SetLog = logType;
                logDataGrid.DataContext = support.ReadLog;
            }
        }

        private DataTable filterTable
        {
            get
            {
                DataTable unFiltered = support.ReadLog;
                DataTable filtered = unFiltered.Clone();

                var filterLevelItem = (ComboBoxItem) filterLevel.SelectedItem;
                DateTime? startDT1 = startDate.SelectedDate;
                DateTime? endDT1 = endDate.SelectedDate;
                DateTime startDT;
                DateTime endDT;

                string levelFilter = null;
                try
                {
                    levelFilter = filterLevelItem.Content.ToString();
                }
                catch (Exception)
                {
                }

                string messageFilter = filterMessage.Text;
                string cFilter = null;

                if (levelFilter != "")
                {
                    cFilter = "(Level='" + levelFilter + "')";
                }

                if (!string.IsNullOrWhiteSpace(messageFilter))
                {
                    string msgFilter = "(Message LIKE '%" + messageFilter + "%')";

                    if (string.IsNullOrWhiteSpace(cFilter))
                    {
                        cFilter = msgFilter;
                    }
                    else
                    {
                        cFilter = cFilter + " AND " + msgFilter;
                    }
                }

                if (startDT1.HasValue || endDT1.HasValue)
                {
                    string dtFilter = null;

                    var start = new DateTime(2011, 1, 1);
                    DateTime end = DateTime.Now;

                    startDT = startDT1 ?? start;
                    endDT = endDT1 ?? end;

                    dtFilter = "(DATE >= '" + startDT.ToString("yyyy-MM-dd hh:mm:ss.FFF") + "')";
                        // AND DATE <='" +endDT.ToString("yyyy-MM-dd hh:mm:ss.FFF") + "')";
                    if (string.IsNullOrWhiteSpace(cFilter))
                    {
                        cFilter = dtFilter;
                    }
                    else
                    {
                        cFilter = cFilter + " AND " + dtFilter;
                    }
                }

                logger.Log(LogLevel.Debug, "cFilter :: " + cFilter);

                if (cFilter != " ")
                {
                    logger.Log(LogLevel.Debug, "cFilter applied");
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

        private void backClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Menu());
        }

        private void submitClick(object sender, RoutedEventArgs e)
        {
            // Force only sending the support logs
            Switcher.Switch(new Submit("support"));
        }

        private void UserControlLoaded(object sender, RoutedEventArgs e)
        {
            IEnumerable source = logDataGrid.ItemsSource;
            cvLog = CollectionViewSource.GetDefaultView(source);
            if (cvLog != null)
            {
                cvLog.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));
            }
        }

        private void filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            logDataGrid.DataContext = filterTable;
            CollectionViewSource.GetDefaultView(logDataGrid.ItemsSource).Refresh();
        }

        private void filterMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            logDataGrid.DataContext = filterTable;
            CollectionViewSource.GetDefaultView(logDataGrid.ItemsSource).Refresh();
        }

        private void openFolder(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\SIMSBulkImport\\Logs";
                prc.Start();
            }
            catch (System.Exception MenuItem_Click_LogsException)
            {
                logger.Log(NLog.LogLevel.Error, MenuItem_Click_LogsException);
            }
        }

        private void type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            logType = this.type.SelectedValue.ToString();
            LoadLog();
        }
    }
}