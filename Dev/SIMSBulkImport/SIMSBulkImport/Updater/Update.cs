/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using NLog;

namespace Matt40k.SIMSBulkImport.Updater
{
    public static class Update
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static Requestor _requestor;
        private static Proxy _proxy;
        private static string serverVersion;

        public static void Check()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Updater.Check");

            string localVersion = GetExe.Version;
            logger.Log(LogLevel.Trace, "localVersion :: " + localVersion);

            var bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += getServerVersion_DoWork;
            bw.RunWorkerCompleted += getServerVersion_RunWorkerCompleted;

            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }
            else
            {
                logger.Log(LogLevel.Info, "Not checking for updates");
            }
        }

        private static void getServerVersion()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Updater.getServerVersion()");
            DataTable address = _requestor.GetVersion;
            DataRow dt = address.Rows[0];
            serverVersion = dt["latestversion"].ToString();
        }

        private static void getServerVersion_DoWork(object sender, DoWorkEventArgs e)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.Updater.getServerVersion_DoWork(sender: " + sender + ", e: " + e + ")");
            try
            {
                _requestor = new Requestor();
                _proxy = new Proxy();
                _proxy.SetUrl = ConfigMan.UpdateUrl;
                _requestor.SetApiUrl = ConfigMan.UpdateUrl;
                getServerVersion();
            }
            catch (Exception getServerVersion_DoWork_Exception)
            {
                logger.Log(LogLevel.Error, getServerVersion_DoWork_Exception);
            }
        }

        private static void runUpdate()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Updater.runUpdate()");
            string outOfDate = "http://www.matt40k.co.uk/simsbulkimport.html"; // TODO
            try
            {
                var process = new Process();
                process.StartInfo.FileName = outOfDate;
                process.Start();
            }
            catch (Exception buttonAppUrl_Exception)
            {
                logger.Log(LogLevel.Error, buttonAppUrl_Exception);
            }
        }

        private static void getServerVersion_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.Updater.getServerVersion_RunWorkerCompleted(sender: " + sender + ", e: " +
                e + ")");
            string localVersion = GetExe.Version;

            if (!string.IsNullOrEmpty(serverVersion))
            {
                //Check versions
                logger.Log(LogLevel.Debug, "Client: " + localVersion);
                logger.Log(LogLevel.Debug, "Server: " + serverVersion);
                switch (localVersion.CompareTo(serverVersion))
                {
                    case 0:
                        logger.Log(LogLevel.Info, "Application is up-to-date");
                        break;
                    case 1:
                        logger.Log(LogLevel.Info, "Application is newer then server - server is out-of date?!?");
                        break;
                    case -1:
                        logger.Log(LogLevel.Info, "Application is out-of-date, please update");
                        runUpdate();
                        break;
                }
            }
        }
    }
}