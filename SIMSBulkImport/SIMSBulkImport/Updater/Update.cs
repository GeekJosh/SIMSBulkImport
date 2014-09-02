/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Matt40k.SIMSBulkImport.Updater
{
    public static class Update
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static Requestor _requestor;
        private static Proxy _proxy;
        private static string serverVersion;

        public static void Check()
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Updater.Check");

            string localVersion = GetExe.Version;
            logger.Log(NLog.LogLevel.Trace, "localVersion :: " + localVersion);

            logger.Log(NLog.LogLevel.Trace, "checkUrl :: " + ConfigMan.UpdateUrl);

            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(GetServerVersion_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GetServerVersion_RunWorkerCompleted);

            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }
            else
            {
                logger.Log(NLog.LogLevel.Info, "Not checking for updates");
            }
        }

        public static void GetServerVersion()
        {
            DataTable address = _requestor.GetVersion;
            DataRow dt = address.Rows[0];
            serverVersion = dt["version"].ToString();
        }

        private static void GetServerVersion_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _requestor = new Requestor();
                _proxy = new Proxy();
                _proxy.SetUrl = ConfigMan.UpdateUrl;
                _requestor.SetApiUrl = ConfigMan.UpdateUrl;
                GetServerVersion();
            }
            catch (Exception GetServerVersion_DoWork_Exception)
            {
                logger.Log(NLog.LogLevel.Error, GetServerVersion_DoWork_Exception);
            }
        }

        private static void runUpdate()
        {
            string outOfDate = ConfigMan.UpdateUrl + "simsbulkimport.html";
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = outOfDate;
                process.Start();
            }
            catch (Exception buttonAppUrl_Exception)
            {
                logger.Log(NLog.LogLevel.Error, buttonAppUrl_Exception);
            }
        }

        private static void GetServerVersion_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string localVersion = GetExe.Version;

            if (!string.IsNullOrEmpty(serverVersion))
            {
                //Check versions
                logger.Log(NLog.LogLevel.Debug, "Client: " + localVersion);
                logger.Log(NLog.LogLevel.Debug, "Server: " + serverVersion);
                switch (localVersion.CompareTo(serverVersion))
                {
                    case 0:
                        logger.Log(NLog.LogLevel.Info, "Application is up-to-date");
                        break;
                    case 1:
                        logger.Log(NLog.LogLevel.Info, "Application is newer then server - server is out-of date?!?");
                        break;
                    case -1:
                        logger.Log(NLog.LogLevel.Info, "Application is out-of-date, please update");
                        runUpdate();
                        break;
                }
            }
        }



    }
}



/*

 BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.WorkerSupportsCancellation = true;
                bw.DoWork += new DoWorkEventHandler(GetServerVersion_DoWork);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GetServerVersion_RunWorkerCompleted);

                if (bw.IsBusy != true)
                {
                    bw.RunWorkerAsync();
                }
            }
            else
            {
                logger.Log(NLog.LogLevel.Info, "Not checking for updates");
            }
        }

        private static bool CheckDefaultProxyForRequest(Uri resource)
        {
            WebProxy proxy = (WebProxy)WebProxy.GetDefaultProxy();

            // See what proxy is used for resource.
            Uri resourceProxy = proxy.GetProxy(resource);

            // Test to see whether a proxy was selected.
            if (resourceProxy == resource)
            {
                logger.Log(NLog.LogLevel.Debug, "Proxy: None");
                return false;
            }
            else
            {
                logger.Log(NLog.LogLevel.Debug, "Proxy: " + resourceProxy.ToString());
                return true;
            }
        }


        private static void GetServerVersion_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _ds = new DataSet("bindhub");
                _requestor = new Requestor();
                _proxy = new Proxy();
                _proxy.SetUrl = ConfigMan.UpdateUrl;

                
            }
        }
 * 
        private static string serverVersion;

        public static void Check()
        {
            if (ConfigMan.CheckForUpdates)
            {
                logger.Log(NLog.LogLevel.Info, "Checking for updates");
                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.WorkerSupportsCancellation = true;
                bw.DoWork += new DoWorkEventHandler(GetServerVersion_DoWork);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GetServerVersion_RunWorkerCompleted);

                if (bw.IsBusy != true)
                {
                    bw.RunWorkerAsync();
                }
            }
            else
            {
                logger.Log(NLog.LogLevel.Info, "Not checking for updates");
            }
        }

        private static bool CheckDefaultProxyForRequest(Uri resource)
        {
            WebProxy proxy = (WebProxy)WebProxy.GetDefaultProxy();

            // See what proxy is used for resource.
            Uri resourceProxy = proxy.GetProxy(resource);

            // Test to see whether a proxy was selected.
            if (resourceProxy == resource)
            {
                logger.Log(NLog.LogLevel.Debug, "Proxy: None");
                return false;
            }
            else
            {
                logger.Log(NLog.LogLevel.Debug, "Proxy: " + resourceProxy.ToString());
                return true;
            }
        }

        private static void GetServerVersion_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Uri updateUrl = new Uri(ConfigMan.UpdateUrl + "version.php");
                bool useProxy = CheckDefaultProxyForRequest(updateUrl);
                WebRequest request = WebRequest.Create(updateUrl);
                if (useProxy)
                {
                    WebProxy proxy = (WebProxy)WebRequest.DefaultWebProxy;
                    request.Proxy = proxy;
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                logger.Log(NLog.LogLevel.Debug, response.StatusCode + " - " + response.StatusDescription);
                Stream dataStream = response.GetResponseStream ();
                StreamReader reader = new StreamReader (dataStream);
                serverVersion = reader.ReadToEnd();
            }
            catch (Exception GetServerVersionException)
            {
                logger.Log(NLog.LogLevel.Debug, GetServerVersionException);
            }
        }

        private static void GetServerVersion_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string localVersion = GetExe.Version;

            if (!string.IsNullOrEmpty(serverVersion))
            {
                //Check versions
                logger.Log(NLog.LogLevel.Debug, "Client: " + localVersion);
                logger.Log(NLog.LogLevel.Debug, "Server: " + serverVersion);
                switch (localVersion.CompareTo(serverVersion))
                {
                    case 0:
                        logger.Log(NLog.LogLevel.Info, "Application is up-to-date");
                        break;
                    case 1:
                        logger.Log(NLog.LogLevel.Info, "Application is newer then server - server is out-of date?!?");
                        break;
                    case -1:
                        logger.Log(NLog.LogLevel.Info, "Application is out-of-date, please update");
                        Switcher.Switch(new NeedToUpdate());
                        break;
                }
            }
        }*/