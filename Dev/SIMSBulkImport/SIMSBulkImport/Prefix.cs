/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    class Prefix
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static string _prefix;

        /// <summary>
        /// Get the default prefix for all temp files created
        /// </summary>
        public static string GetPrefix
        {
            get
            {
                if (string.IsNullOrEmpty(_prefix))
                {
                    _prefix = "SIMSBulkImport_";
                }
                return _prefix;
            }
        }
    }
}
