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

        private static void GetServerVersion()
        {
                DataTable address = _requestor.GetVersion;
                DataRow dt = address.Rows[0];
                serverVersion = dt["latestversion"].ToString();
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