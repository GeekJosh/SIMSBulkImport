/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using NLog;

namespace Matt40k.SIMSBulkImport
{
    internal class Prefix
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static string _prefix;

        /// <summary>
        ///     Get the default prefix for all temp files created
        /// </summary>
        public static string GetPrefix
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Prefix.GetPrefix(GET)");
                if (string.IsNullOrEmpty(_prefix))
                {
                    _prefix = "SIMSBulkImport_";
                }
                return _prefix;
            }
        }
    }
}