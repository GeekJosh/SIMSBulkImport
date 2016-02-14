using System;
using System.IO;
using NLog;

namespace SIMSBulkImport.Core
{
    public static class SimsIni
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static string appsDir;

        public static string GetSimsDir
        {
            get
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.PowerShell.SimsIni.GetSimsDir(GET)");
                string simsPath = null;

                try
                {
                    if (string.IsNullOrEmpty(appsDir))
                    {
                        if (string.IsNullOrEmpty(simsPath))
                        {
                            string curSims = Path.Combine(curdir, "sims.ini");
                            if (File.Exists(curSims))
                            {
                                simsPath = curSims;
                            }
                            string winSims = Path.Combine(windir, "sims.ini");
                            if (File.Exists(winSims))
                            {
                                simsPath = winSims;
                            }
                        }
                        if (!string.IsNullOrEmpty(simsPath))
                        {
                            var ini = new IniFile(simsPath);
                            string Apps = ini.Read("Setup", "SIMSDotNetDirectory");
                            if (!string.IsNullOrEmpty(Apps))
                            {
                                if (Directory.Exists(Apps))
                                {
                                    appsDir = Apps;
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
                    logger.Log(LogLevel.Debug, "SIMS Application Directory: " + appsDir);
                    return appsDir;
                }
                catch (Exception GetSimsDirException)
                {
                    logger.Log(LogLevel.Fatal, GetSimsDirException);
                    Environment.Exit(100);
                }
                return appsDir;
            }
        }


        private static string curdir
        {
            get
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.PowerShell.SimsIni.curdir(GET)");
                return Directory.GetCurrentDirectory();
            }
        }

        private static string windir
        {
            get
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.PowerShell.SimsIni.windir(GET)");
                return Environment.GetEnvironmentVariable("windir");
            }
        }

        public static string SetSimsDir
        {
            set
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.PowerShell.SimsIni.SetSimsDir(SET: " + value + ")");
                appsDir = value;
            }
        }
    }
}