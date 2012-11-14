using System;
using System.IO;

namespace GingerBeard
{
    public class SimsIni
    {
        private string appsDir = null;
        private string simsPath = null;

        public string GetSimsDir
        {
            get
            {
                if (string.IsNullOrEmpty(appsDir))
                {
                    if (string.IsNullOrEmpty(simsPath))
                    {
                        // TODO Add support for same directory (run as) and application directory
                        if (File.Exists("sims.ini")) { simsPath = "sims.ini"; }
                        string winSims = Path.Combine(windir, "sims.ini");
                        if (File.Exists(winSims)) { simsPath = winSims; }
                    }
                    if (!string.IsNullOrEmpty(simsPath))
                    {
                        IniFile ini = new IniFile(simsPath);
                        string Apps = ini.Read("Setup", "SIMSDotNetDirectory");
                        if (Directory.Exists(Apps))
                        {
                            appsDir = Apps;
                            return appsDir;
                        }
                        else
                        {
                            throw new Exception("Error: SIMS .net Application directory does not exist! - " + appsDir);
                        }
                    }
                    else
                    {
                        throw new Exception("Error: Unable to find SIMS.ini");
                    }
                }
                else return appsDir;
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
