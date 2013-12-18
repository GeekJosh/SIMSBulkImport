/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using Microsoft.Win32;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    /// To identify the number of unique users, without forcing people to give up their 
    /// anonymity we give them a unique ID (GUID), generated at installation (TODO).
    /// </summary>
    internal class Stats
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // Reads the GUID from the registry
        public string ReadID
        {
            get
            {
                try
                {
                    RegistryKey regKey1 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\SIMS Bulk Import", false);
                    return (string)regKey1.GetValue("ID");
                }
                catch (Exception ReadID_exception)
                {
                    logger.Log(LogLevel.Error, ReadID_exception);
                }
                return Guid.NewGuid().ToString();
            }
        }
        
    }
}
