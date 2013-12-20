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
    /// Interaction logic for Submit.xaml
    /// </summary>
    public partial class Submit
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Submit()
        {
            InitializeComponent();
            StatID.Text = getStatId;
        }

        private string getStatId
        {
            get
            {
                Stats stats = new Stats();
                logger.Log(NLog.LogLevel.Info, "ID for Stats: " + stats.ReadID);
                return stats.ReadID;
            }
        }

        private void backClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Switcher.Switch(new Logs());
        }

        private void submitClick(object sender, System.Windows.RoutedEventArgs e)
        {
            DataTable submittionResults = Support.Submit.Logs(Email.Text, readLogFile);
            Switcher.Switch(new Menu());
        }

        private string _logFile
        {
            get
            {
                NLog.Targets.FileTarget t = (NLog.Targets.FileTarget)LogManager.Configuration.FindTargetByName("system");
                var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
                return t.FileName.Render(logEventInfo);
            }
        }

        private string readLogFile
        {
            get
            {
                string _log = null;
                string logFile = _logFile;
                if (File.Exists(logFile))
                {
                    StreamReader log = new StreamReader(logFile);
                    _log = log.ReadToEnd();
                    log.Close();
                }
                return _log;
            }
        }

    }
}
