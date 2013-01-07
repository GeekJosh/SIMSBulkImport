/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.IO;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    class ClearUp
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Checks for any temporary files the application has created previously and then removes them.
        /// </summary>
        public static void ClearTmp()
        {
            string[] files = Directory.GetFiles(Path.GetTempPath(), Prefix.GetPrefix + "*");

            foreach (string file in files)
            {
                removeFile(file);
            }
        }

        private static void removeFile(string file)
        {
            if (File.Exists(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch (IOException removeFile_IOException)
                {

                }
                catch (Exception removeFile_Exception)
                {

                }
            }
            else
            {
                // File has disappeared!!
            }
        }
    }
}
