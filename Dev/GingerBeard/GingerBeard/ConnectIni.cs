using System;
using System.IO;

namespace GingerBeard
{
    public class ConnectIni
    {
        private string connectPath = null;

        public string SetSimsDir
        {
            set
            {
                //DEBUG System.Windows.MessageBox.Show("Connect.ini path is null");
                // TODO Add support for same directory (run as) and application directory
                if (!string.IsNullOrEmpty(value))
                {
                    if (File.Exists(Path.Combine(value, "Connect.ini")))
                    {
                        SetConnectPath = value;
                    }
                }
                if (File.Exists(Path.Combine(connectPath, "Connect.ini")))
                {
                    SetConnectPath = value;
                }
                else throw new Exception("Error: Connect.ini does not exist! " + connectPath);
            }
        }

        public string GetServerName
        {
            get
            {
                string connect = System.IO.Path.Combine(connectPath, "Connect.ini");
                IniFile iniFile = new IniFile(connect);
                return iniFile.Read("SIMSConnection", "ServerName");
            }
        }

        public string GetDatabaseName
        {
            get
            {
                string connect = System.IO.Path.Combine(connectPath, "Connect.ini");
                IniFile iniFile = new IniFile(connect);
                return iniFile.Read("SIMSConnection", "DatabaseName");
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

        public string SetConnectPath
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
    }
}
