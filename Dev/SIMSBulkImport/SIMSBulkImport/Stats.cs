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
    ///     To identify the number of unique users, without forcing people to give up their
    ///     anonymity we give them a unique ID (GUID), generated at installation (TODO).
    /// </summary>
    internal static class Stats
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        // Reads the GUID from the registry
        internal static string ReadID
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Stats.ReadID(GET)");
                string id = null;
                try
                {
                    RegistryKey regKey1 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\SIMS Bulk Import", false);
                    id = (string) regKey1.GetValue("ID");
                    regKey1.Close();
                }
                catch (Exception ReadID_exception)
                {
                    logger.Log(LogLevel.Error, ReadID_exception);
                }
                if (string.IsNullOrEmpty(id))
                    id = Guid.NewGuid().ToString();
                return id;
            }
        }
    }
}