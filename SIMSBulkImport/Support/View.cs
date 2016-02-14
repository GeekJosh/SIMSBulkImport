﻿using System;
using System.Data;
using System.IO;
using NLog;
using NLog.Targets;

namespace SIMSBulkImport.Support
{
    /// <summary>
    ///     Allows user to submit the log file to our web server
    /// </summary>
    internal class View
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private string log;

        public string SetLog
        {
            set
            {
                switch(value)
                {
                    case "support":
                        log = "support";
                        break;
                    default:
                        log = "system";
                        break;
                }
            }
        }

        private string _logFile
        {
            get
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Support.View._logFile(GET)");
                var t = (FileTarget) LogManager.Configuration.FindTargetByName(log);
                var logEventInfo = new LogEventInfo {TimeStamp = DateTime.Now};
                return t.FileName.Render(logEventInfo);
            }
        }

        public DataTable ReadLog
        {
            get
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Support.View.ReadLog(GET)");
                string logFile = _logFile;
                if (File.Exists(logFile))
                {
                    DataTable log = logTable;
                    int counter = 0;
                    string line;
                    //string format = "yyyy-MM-dd hh:mm:ss.FFFF";

                    // Read the file and display it line by line.
                    var file = new StreamReader(logFile);
                    while ((line = file.ReadLine()) != null)
                    {
                        string[] lineParts = line.Split('|');
                        if (lineParts.Length == 3)
                        {
                            DataRow newrow = log.NewRow();
                            try
                            {
                                newrow["Date"] = Convert.ToDateTime(lineParts[0]);
                            }
                            catch (Exception e)
                            {
                                logger.Log(LogLevel.Debug, lineParts[0] + " :: " + e);
                            }
                            newrow["Level"] = lineParts[1];
                            newrow["Message"] = lineParts[2];
                            try
                            {
                                log.Rows.Add(newrow);
                            }
                            catch (Exception e)
                            {
                                logger.Log(LogLevel.Debug, e.ToString());
                            }
                        }
                        counter++;
                    }

                    file.Close();

                    return log;
                }
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Support.View.ReadLog(GET) FALLEN OVER");
                return null;
            }
        }

        private DataTable logTable
        {
            get
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Support.View.logTable(GET)");
                var log = new DataTable("Log");
                log.Columns.Add(new DataColumn("Date", typeof (DateTime)));
                log.Columns.Add(new DataColumn("Level", typeof (string)));
                log.Columns.Add(new DataColumn("Message", typeof (string)));
                return log;
            }
        }
    }
}