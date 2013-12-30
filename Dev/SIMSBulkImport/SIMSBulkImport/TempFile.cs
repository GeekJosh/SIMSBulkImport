/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.IO;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    class TempFile
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Creates a (hopefully) unique pretty* temporary file
        /// </summary>
        public static string GetNewTempFile
        {
            get
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.TempFile.GetNewTempFile(GET");
                string _filename = Prefix.GetPrefix + Path.GetRandomFileName();
                _filename = Path.ChangeExtension(_filename, ".tmp");
                _filename = Path.Combine(Path.GetTempPath(), _filename);
                using (File.Create(_filename)) {};
                return _filename;
            }
        }
    }
}
