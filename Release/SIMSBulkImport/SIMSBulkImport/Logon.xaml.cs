/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using SIMS.Processes;
using SIMS.Entities;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    /// Interaction logic for Logon.xaml
    /// </summary>
    public partial class Logon : Window
    {
        private SIMSAPI SimsApi;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        string serverName;
        string databaseName;
        string userServer;
        string userDatabase;
        string connectDir;

        public Logon(SIMSAPI simsApi)
        {

            SimsApi = simsApi;
            serverName = SimsApi.GetServerName;
            databaseName = SimsApi.GetDatabaseName;
            connectDir = SimsApi.GetConnectPath;

            InitializeComponent();

            this.textUser.Focus();
            this.radioButtonWindows.IsEnabled = false;
            this.Title = "Logon - " + GetExe.Title;
            this.textServer.Text = serverName;
            this.textDatabase.Text = databaseName;

            /*
            if (IsServerAndDatabaseEmpty)
            {
                this.expander.IsExpanded = true;
            }
            
            if (SimsApi.IsDemo)
            {
                this.textUser.Text = "blacka";
                this.passwordBox.Password = "abcd";
            }*/
        }

        private bool IsServerAndDatabaseEmpty
        {
            get
            {
                return (string.IsNullOrEmpty(serverName) || string.IsNullOrEmpty(databaseName));
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            logger.Log(NLog.LogLevel.Debug, serverName);
            logger.Log(NLog.LogLevel.Debug, databaseName);

           
                if (!string.IsNullOrWhiteSpace(this.textUser.Text))
                {
                    SimsApi.SetSimsUser = this.textUser.Text;
                    SimsApi.SetSimsPass = this.passwordBox.Password;

                    SimsApi.SetRestoreConnect = connectDir;
                    /*
                    if (IsChangedConnect)
                    {
                        logger.Log(NLog.LogLevel.Debug, "Using user-defined connect.ini");
                        SimsApi.SetUserConnect(System.IO.Path.GetTempPath(), userServer, userDatabase);
                    }
                     * */
                    if (SimsApi.Connect)
                    {
                        this.Close();
                    }
                }
                if (IsServerAndDatabaseEmpty)
                {
                    logger.Log(NLog.LogLevel.Error, "IsServerAndDatabaseEmpty: " + IsServerAndDatabaseEmpty);
                }
        }

        private void expander_Expanded(object sender, RoutedEventArgs e)
        {
            bool isexpan = this.expander.IsExpanded;
            logger.Log(NLog.LogLevel.Debug, "Logon advance: " + isexpan);
            string strExpan = isexpan.ToString();

            if (strExpan == "True")
            {
                this.Height = 315;
            }
            else
            {
                this.Height = 240;
            }
        }

        private void textConnect_TextChanged(object sender, TextChangedEventArgs e)
        {
            /*
            string configServer = SimsApi.GetServerName;
            string configDatabase = SimsApi.GetDatabaseName;
            */

            userServer = this.textServer.Text;
            userDatabase = this.textDatabase.Text;
        }

        private bool IsChangedConnect
        {
            get
            {
                if (userServer != serverName)
                {
                    return true;
                }
                else
                {
                    if (userDatabase != databaseName)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private void radioButtonSql_Checked(object sender, RoutedEventArgs e)
        {
            //this.radioButtonWindows.IsChecked = false;
        }

        private void radioButtonWindows_Checked(object sender, RoutedEventArgs e)
        {
            this.radioButtonSql.IsChecked = false;
        }
    }
}
