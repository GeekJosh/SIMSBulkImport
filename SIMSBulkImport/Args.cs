using System;
using System.IO;
using System.Windows;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    public class Args
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly string _simsDir;

        public Args(string[] args)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Args()");
            for (int i = 0; i < args.Length; i++)
            {
                string str = args[i].ToLower();
                if (str.StartsWith("--simsdirectory"))
                {
                    _simsDir = GetParameterValue(args, "--simsdirectory");
                }
                if (Directory.Exists(_simsDir))
                {
                    SimsIni.SetSimsDir = _simsDir;
                    var application = new Application();
                    application.Run(new PageSwitcher());
                }
            }
        }

        /// <summary>
        ///     Gets the setting from the command line.
        /// </summary>
        /// <param name="commandParameters"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        private static string GetParameterValue(string[] commandParameters, string parameterName)
        {
            try
            {
                for (int i = 0; i < commandParameters.Length; i++)
                {
                    string str = commandParameters[i];
                    if (str.ToUpper().StartsWith(parameterName.ToUpper()))
                    {
                        if (parameterName == str)
                        {
                            return "";
                        }
                        return str.Substring(parameterName.Length + 1);
                    }
                }
            }
            catch (Exception GetParameterValue_Exception)
            {
                logger.Log(LogLevel.Error, GetParameterValue_Exception);
            }
            return "";
        }
    }
}