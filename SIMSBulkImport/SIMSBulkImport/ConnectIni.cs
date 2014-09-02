/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.IO;

namespace Matt40k.SIMSBulkImport
{
    public class ConnectIni
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private string connectPath;
        private string redirect;

        public ConnectIni()
        {
            
        }

        public string SetSimsDir
        {
            set
            {
                // If the application has been given a local connect.ini - use that (overwrite)
                if (File.Exists(System.IO.Path.Combine(curdir, "connect.ini")))
                {
                    logger.Log(NLog.LogLevel.Debug, "Connect.ini found in " + GetExe.Title);
                    connectPath = curdir;
                    return;
                }

                if (!string.IsNullOrEmpty(value))
                {
                    if (File.Exists(System.IO.Path.Combine(value, "connect.ini")))
                    {
                        logger.Log(NLog.LogLevel.Debug, "Connect.ini found in SIMS Application");
                        connectPath = value;
                        return;
                    }
                }

                string tmp3 = @"c:\program files\sims\sims .net";
                string tmp4 = @"c:\program files (x86)\sims\sims .net";


                if (File.Exists(System.IO.Path.Combine(tmp3, "connect.ini")))
                {
                    logger.Log(NLog.LogLevel.Debug, "Connect.ini found in failsafe");
                    connectPath = tmp3;
                    return;
                }


                if (File.Exists(System.IO.Path.Combine(tmp4, "connect.ini")))
                {
                    logger.Log(NLog.LogLevel.Debug, "Connect.ini found in failsafe (x86)");
                    connectPath = tmp4;
                    return;
                }

                if (ConnectWithRedirect)
                {
                    if (File.Exists(System.IO.Path.Combine(redirect, "connect.ini")))
                    {
                        logger.Log(NLog.LogLevel.Debug, "Connect.ini found in redirect");
                        connectPath = redirect;
                        return;
                    }
                }
            }
        }

        public string GetServerName
        {
            get
            {
                try
                {
                    //System.Windows.Forms.MessageBox.Show("Boo! Did I scare you?");
                    IniFile iniFile = new IniFile(System.IO.Path.Combine(connectPath, "connect.ini"));
                    return iniFile.Read("SIMSConnection", "ServerName");
                }
                catch (System.Exception GetServerNameException)
                {
                    logger.Log(NLog.LogLevel.Error, GetServerNameException);
                }
                return null;
            }
        }

        public string GetDatabaseName
        {
            get
            {
                try
                {
                    IniFile iniFile = new IniFile(System.IO.Path.Combine(connectPath, "connect.ini"));
                    return iniFile.Read("SIMSConnection", "DatabaseName");
                }
                catch (System.Exception GetDatabaseNameException)
                {
                    logger.Log(NLog.LogLevel.Error, GetDatabaseNameException);
                }
                return null;
            }
        }

        private bool ConnectWithRedirect
        {
            get
            {
                try
                {
                    redirect = null;
                    IniFile iniFile = new IniFile(System.IO.Path.Combine(connectPath, "connect.ini"));
                    redirect = iniFile.Read("SIMSConnection", "Redirect");
                    if (!string.IsNullOrEmpty(redirect))
                    {
                        logger.Log(NLog.LogLevel.Debug, "Redirect found: " + redirect);
                        return true;
                    }
                }
                catch (Exception ConnectWithRedirectException)
                {
                    logger.Log(NLog.LogLevel.Error, ConnectWithRedirectException);
                }
                return false;
            }
        }

        public string SetServerName
        {
            set
            {
                string connect = System.IO.Path.Combine(connectPath, "Connect.ini");
                IniFile iniFile = new IniFile(connect);
                iniFile.Write("SIMSConnection", "ServerName", value);
            }
        }

        public string SetDatabaseName
        {
            set
            {
                string connect = System.IO.Path.Combine(connectPath, "Connect.ini");
                IniFile iniFile = new IniFile(connect);
                iniFile.Write("SIMSConnection", "DatabaseName", value);
            }
        }

        private string SetConnectPath
        {
            set
            {
                connectPath = value;
            }
        }

        public string GetConnectPath
        {
            get
            {
                return connectPath;
            }
        }

        private string curdir
        {
            get
            {
                return Directory.GetCurrentDirectory();
            }
        }
    }
}
