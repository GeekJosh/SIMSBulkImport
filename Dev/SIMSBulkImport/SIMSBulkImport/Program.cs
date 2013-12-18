/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    public class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [STAThread]
        static void Main(string[] args)
        {
            bool freeToRun;
            if (args.Length == 0)
            {
                try
                {
                    string safeName = "Local\\SimsBulkImport";
                    using (Mutex m = new Mutex(true, safeName, out freeToRun))
                        if (freeToRun)
                        {
                            Application application = new Application();
                            application.Run(new PageSwitcher());
                        }
                        else
                        {
                            logger.Log(NLog.LogLevel.Error, "Application is already running!");
                            Environment.Exit(5);
                        }
                }
                catch (Exception Main_Exception)
                {
                    logger.Log(NLog.LogLevel.Error, Main_Exception);
                    Environment.Exit(5);
                }
            }
            else
            {
                Args _args = new Args(args);
            }
        }
    }
}
