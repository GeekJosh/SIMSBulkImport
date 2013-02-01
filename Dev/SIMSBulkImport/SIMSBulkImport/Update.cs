/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Net;

namespace Matt40k.SIMSBulkImport
{
    class Update
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

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
        }
    }
}