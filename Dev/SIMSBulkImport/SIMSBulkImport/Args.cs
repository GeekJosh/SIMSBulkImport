/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.IO;
using System.Windows;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    public class Args
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        
        private string _simsDir;

        public Args(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string str = args[i].ToLower();
                if (str.StartsWith("--simsdirectory"))
                {
                    _simsDir = GetParameterValue(args, "--simsdirectory");
                }
                if (Directory.Exists(_simsDir))
                {
                    // Set SIMS Directory
                    Application application = new Application();
                    application.Run(new PageSwitcher());
                }
            }
        }

        /// <summary>
        /// Gets the setting from the command line.
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
                logger.Log(NLog.LogLevel.Error, GetParameterValue_Exception);
            }
            return "";
        }

    }
}
