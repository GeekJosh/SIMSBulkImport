/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System.Windows.Controls;
using NLog;

namespace Matt40k.SIMSBulkImport
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

        public static void Switch(UserControl newPage)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Switcher.Switch(newPage: " + newPage + ")");
            pageSwitcher.Navigate(newPage);
        }

        public static void Switch(UserControl newPage, object state)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.Switcher.Switch(newPage: " + newPage + ", state: " + state + ")");
            pageSwitcher.Navigate(newPage, state);
        }
    }
}