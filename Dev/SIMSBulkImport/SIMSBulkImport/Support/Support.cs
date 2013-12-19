/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;
using System.IO;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    /// Allows user to submit the log file to our web server
    /// </summary>
    internal class Support
    {
        private string _logFile
        {
            get
            {
                NLog.Targets.FileTarget t = (NLog.Targets.FileTarget)LogManager.Configuration.FindTargetByName("system");
                var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
                return t.FileName.Render(logEventInfo);
            }
        }

        public DataTable ReadLog
        {
            get
            {
                string logFile = _logFile;
                if (File.Exists(logFile))
                {
                    DataTable log = logTable;
                    int counter = 0;
                    string line;

                    // Read the file and display it line by line.
                    StreamReader file = new StreamReader(logFile);
                    while ((line = file.ReadLine()) != null)
                    {
                        string[] lineParts = line.Split('|');
                        if (lineParts.Length == 3)
                        {
                            DataRow newrow = log.NewRow();
                            newrow["Date"] = lineParts[0];
                            newrow["Level"] = lineParts[1];
                            newrow["Message"] = lineParts[2];
                            log.Rows.Add(newrow);
                        }
                        counter++;
                    }

                    file.Close();

                    return log;
                }
                return null;
            }
        }

        private DataTable logTable
        {
            get
            {
                DataTable log = new DataTable("Log");
                log.Columns.Add(new DataColumn("Date", typeof(DateTime)));
                log.Columns.Add(new DataColumn("Level", typeof(string)));
                log.Columns.Add(new DataColumn("Message", typeof(string)));
                return log;
            }
        }
    }
}