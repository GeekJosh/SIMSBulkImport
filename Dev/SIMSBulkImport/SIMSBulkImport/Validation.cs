/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    class Validation
    {
        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected internal static bool IsValidEmail(string email)
        {
            RegexUtilities util = new RegexUtilities();
            bool _isValid = util.IsValidEmail(email);
            logger.Log(NLog.LogLevel.Debug, email + " - " + _isValid);
            return _isValid;
        }

        internal static DataTable DeDuplicatation(DataTable dt)
        {
            DataTable deduplicatedDt = new DataTable();
            logger.Log(NLog.LogLevel.Debug, "DeDuplicatation");
            HashSet<DataRow> previousLines = new HashSet<DataRow>();

            foreach (DataRow row in dt.Rows)
            {
                // Add returns true if it was actually added,
                // false if it was already there
                if (previousLines.Add(row))
                {
                    deduplicatedDt.Rows.Add(row);
                }
                else
                {
                    logger.Log(NLog.LogLevel.Debug, "Duplicate: " + row);
                }
            }
            return deduplicatedDt;
        }
    }
}
