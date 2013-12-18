/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.IO;
using System.Windows.Documents;
using System.Windows.Forms;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    /// Interaction logic for Logs.xaml
    /// </summary>
    public partial class Logs
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Logs()
        {
            InitializeComponent();
            readLog();
        }

        private void readLog()
        {
            TextRange range;
            FileStream fStream;
            if (File.Exists(_logFile))
            {
                range = new TextRange(logTextBox.Document.ContentStart, logTextBox.Document.ContentEnd);
                fStream = new FileStream(_logFile, FileMode.Open);
                range.Load(fStream, DataFormats.Text);
                fStream.Close();
            }
        }

        private void backClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Switcher.Switch(new Menu());
        }

        private void submitClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Switcher.Switch(new Submit());
        }

        public string _logFile
        {
            get
            {
                NLog.Targets.FileTarget t = (NLog.Targets.FileTarget)LogManager.Configuration.FindTargetByName("system");
                var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
                return t.FileName.Render(logEventInfo);
            }
        } 
    }
}
