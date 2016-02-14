using System.Windows.Controls;
using NLog;
using UserGen;

namespace SIMSBulkImport
{
    // Reference: http://azerdark.wordpress.com/2010/04/23/multi-page-application-in-wpf/
    public static class Switcher
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static PageSwitcher pageSwitcher;

        public static SIMSAPI SimsApiClass;
        public static ImportFile ImportFileClass;
        public static PreImport PreImportClass;
        public static ResultsImport ResultsImportClass;
        public static ImportList ImportListClass;
        public static Import ImportClass;
        public static ConfigMan ConfigManClass;
        public static Builder UserGenClass;

        public static void Switch(UserControl newPage)
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Switcher.Switch(newPage: " + newPage + ")");
            pageSwitcher.Navigate(newPage);
        }

        public static void Switch(UserControl newPage, object state)
        {
            logger.Log(LogLevel.Debug,
                "Trace:: SIMSBulkImport.Switcher.Switch(newPage: " + newPage + ", state: " + state + ")");
            pageSwitcher.Navigate(newPage, state);
        }
    }
}