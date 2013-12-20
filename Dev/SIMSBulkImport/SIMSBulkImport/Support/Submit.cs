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

namespace Matt40k.SIMSBulkImport.Support
{
    public static class Submit
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static Requestor _requestor;
        private static Proxy _proxy;
        private static string _email;
        private static string _log;
        private static string _result = null;
        private static string _logId = null;
        private static DataTable address;

        public static DataTable Logs(string email, string log)
        {
            _email = email;
            _log = log;
            address = null;

            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Support.Submit.Logs");

            logger.Log(NLog.LogLevel.Trace, "checkUrl :: " + ConfigMan.UpdateUrl);

            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(PostLogs_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(PostLogs_RunWorkerCompleted);

            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }
            else
            {
                logger.Log(NLog.LogLevel.Info, "Not checking for updates");
            }

            return address;
        }

        private static void postLogs()
        {
            address = _requestor.SubmitLog(_email, _log);
        }

        private static void PostLogs_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _requestor = new Requestor();
                _proxy = new Proxy();
                _proxy.SetUrl = ConfigMan.UpdateUrl;
                _requestor.SetApiUrl = ConfigMan.UpdateUrl;
                postLogs();
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

        private static void PostLogs_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
    }
}