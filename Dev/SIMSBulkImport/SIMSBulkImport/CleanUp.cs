/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.IO;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    internal class ClearUp
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Checks for any temporary files the application has created previously and then removes them.
        /// </summary>
        public static void ClearTmp()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ClearTmp()");
            string[] files = Directory.GetFiles(Path.GetTempPath(), Prefix.GetPrefix + "*");

            foreach (string file in files)
            {
                removeFile(file);
            }
        }

        private static void removeFile(string file)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.removeFile(" + file + ")");
            if (File.Exists(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch (IOException removeFile_IOException)
                {
                    logger.Log(LogLevel.Debug, removeFile_IOException);
                }
                catch (Exception removeFile_Exception)
                {
                    logger.Log(LogLevel.Debug, removeFile_Exception);
                }
            }
            else
            {
                logger.Log(LogLevel.Debug, "removeFile - File has disappeared!!");
            }
        }
    }
}