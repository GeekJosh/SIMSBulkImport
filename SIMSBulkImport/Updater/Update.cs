﻿using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using NLog;

namespace SIMSBulkImport.Updater
{
    public static class Update
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static Requestor _requestor;
        private static Proxy _proxy;
       // private static string serverVersion;
        private static bool needUpdate;

        public static void Check()
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Updater.Check");

            string localVersion = GetExe.Version;
            logger.Log(LogLevel.Debug, "localVersion :: " + localVersion);

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
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Updater.getServerVersion()");
            needUpdate = _requestor.NeedUpdate;
        }

        private static void getServerVersion_DoWork(object sender, DoWorkEventArgs e)
        {
            logger.Log(LogLevel.Debug,
                "Trace:: SIMSBulkImport.Updater.getServerVersion_DoWork(sender: " + sender + ", e: " + e + ")");
            try
            {
                _requestor = new Requestor();
                _proxy = new Proxy();
                //_proxy.SetUrl = ConfigMan.UpdateUrl;
                //_requestor.SetApiUrl = ConfigMan.UpdateUrl;
                getServerVersion();
            }
            catch (Exception getServerVersion_DoWork_Exception)
            {
                logger.Log(LogLevel.Error, getServerVersion_DoWork_Exception);
            }
        }

        private static void runUpdate()
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Updater.runUpdate()");
            string outOfDate = "https://simsbulkimport.uk/download/";
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
            logger.Log(LogLevel.Debug,
                "Trace:: SIMSBulkImport.Updater.getServerVersion_RunWorkerCompleted(sender: " + sender + ", e: " +
                e + ")");
            if (needUpdate)
                runUpdate();
        }
    }
}
