/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.IO;

namespace Matt40k.SIMSBulkImport
{
    public class SimsIni
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private string appsDir = null;
        private string simsPath = null;

        public string GetSimsDir
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(appsDir))
                    {
                        if (string.IsNullOrEmpty(simsPath))
                        {
                            string curSims = Path.Combine(curdir, "sims.ini");
                            if (File.Exists(curSims)) { simsPath = curSims; }
                            string winSims = Path.Combine(windir, "sims.ini");
                            if (File.Exists(winSims)) { simsPath = winSims; }
                        }
                        if (!string.IsNullOrEmpty(simsPath))
                        {
                            IniFile ini = new IniFile(simsPath);
                            string Apps = ini.Read("Setup", "SIMSDotNetDirectory");
                            if (!string.IsNullOrEmpty(Apps))
                            {

                                if (Directory.Exists(Apps))
                                {
                                    appsDir = Apps;
                                    logger.Log(NLog.LogLevel.Debug, "SIMS Application Directory: " + appsDir);
                                    return appsDir;
                                }
                                else
                                {
                                    throw new Exception("SIMS .net Application directory does not exist! - " + appsDir);
                                }
                            }
                            else
                            {
                                throw new Exception("SIMS Applications does not appear to be installed!");
                            }
                        }
                        else
                        {
                            throw new Exception("Unable to find SIMS.ini");
                        }
                    }
                }
                catch (Exception GetSimsDirException)
                {
                    logger.Log(NLog.LogLevel.Fatal, GetSimsDirException);
                    System.Windows.MessageBox.Show(GetSimsDirException.ToString(), "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
                    Environment.Exit(100);
                }
                return appsDir;
            }
        }
       

        private string curdir
        {
            get
            {
                return Directory.GetCurrentDirectory();
            }
        }

        private string windir
        {
            get
            {
                return Environment.GetEnvironmentVariable("windir");
            }
        }
    }
}
