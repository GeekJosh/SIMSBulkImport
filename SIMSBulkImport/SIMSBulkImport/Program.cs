/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Threading;
using System.Windows;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    public class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [STAThread]
        private static void Main(string[] args)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Program.Main()");
            bool freeToRun;
            if (args.Length == 0)
            {
                try
                {
                    string safeName = "Local\\SimsBulkImport";
                    using (var m = new Mutex(true, safeName, out freeToRun))
                        if (freeToRun)
                        {
                            var application = new Application();
                            application.Run(new PageSwitcher());
                        }
                        else
                        {
                            logger.Log(LogLevel.Error, "Application is already running!");
                            Environment.Exit(5);
                        }
                }
                catch (Exception Main_Exception)
                {
                    logger.Log(LogLevel.Error, Main_Exception);
                    Environment.Exit(5);
                }
            }
            else
            {
                var _args = new Args(args);
            }
        }
    }
}