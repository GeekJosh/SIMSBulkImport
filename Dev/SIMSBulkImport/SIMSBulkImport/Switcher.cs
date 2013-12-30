/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System.Windows.Controls;

namespace Matt40k.SIMSBulkImport
{
    // Reference: http://azerdark.wordpress.com/2010/04/23/multi-page-application-in-wpf/
    public static class Switcher
  	{
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

    	public static PageSwitcher pageSwitcher;

    	public static void Switch(UserControl newPage)
    	{
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Switcher.Switch(newPage: " + newPage + ")");
      		pageSwitcher.Navigate(newPage);
    	}

    	public static void Switch(UserControl newPage, object state)
    	{
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Switcher.Switch(newPage: " + newPage + ", state: " + state + ")");
      		pageSwitcher.Navigate(newPage, state);
    	}

        public static SIMSAPI SimsApiClass;
        public static ImportFile ImportFileClass;
        public static PreImport PreImportClass;
        public static ResultsImport ResultsImportClass;
        public static ImportList ImportListClass;
        public static Import ImportClass;
    }
}