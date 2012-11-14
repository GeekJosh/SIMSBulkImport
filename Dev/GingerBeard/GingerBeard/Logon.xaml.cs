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

namespace GingerBeard
{
    /// <summary>
    /// Interaction logic for Logon.xaml
    /// </summary>
    public partial class Logon : Window
    {
        private SIMSAPI SimsApi;

        public Logon(SIMSAPI simsApi)
        {
            SimsApi = simsApi;

            InitializeComponent();

            this.textUser.Focus();

            this.Title = "Logon :: " + GetName.Title;

            this.textServer.Text = SimsApi.GetServerName;
            this.textDatabase.Text = SimsApi.GetDatabaseName;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrWhiteSpace(this.textUser.Text))
            {
                SimsApi.SetSimsUser = this.textUser.Text;
                SimsApi.SetSimsPass = this.passwordBox.Password;
                if (SimsApi.Connect)
                {
                    this.Close();
                }
            }
        }

        private void expander_Expanded(object sender, RoutedEventArgs e)
        {
            bool isexpan = this.expander.IsExpanded;
            string strExpan = isexpan.ToString();

            if (strExpan == "True")
            {
                this.Height = 280;
            }
            else
            {
                this.Height = 210;
            }
        }

        private string userConnectIni = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Connect.ini");

        private void textConnect_TextChanged(object sender, TextChangedEventArgs e)
        {
            string configServer = SimsApi.GetServerName;
            string configDatabase = SimsApi.GetDatabaseName;

            string userServer = this.textServer.Text;
            string userDatabase = this.textDatabase.Text;

            if (!string.IsNullOrWhiteSpace(userDatabase) && !string.IsNullOrWhiteSpace(userServer))
            {
                if (IsChangedConnect(configServer, configDatabase, userServer, userDatabase))
                {
                    SimsApi.SetUserServer = userServer;
                    SimsApi.SetUserDatabase = userDatabase;
                    SimsApi.SetUserConnect = userConnectIni;
                    //connectPath = userConnectIni;
                }
             }
        }

        private bool IsChangedConnect(string configServer, string configDatabase,
            string userServer, string userDatabase)
        {
            if (userServer != configServer)
            {
                return true;
            }
            else
            {
                if (userDatabase != configDatabase)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
