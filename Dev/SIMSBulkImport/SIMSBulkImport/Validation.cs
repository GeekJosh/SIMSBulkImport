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
            util = null;
            logger.Log(NLog.LogLevel.Debug, email + " - " + _isValid);
            return _isValid;
        }

        // Reference: http://dengkefu.com/blog/programming/c-sharp-and-dot-net/remove-duplicate-records-in-a-datatable-the-easy-way.html
        internal static DataTable DeDuplicatation(DataTable dt)
        {
            DataView dv = new DataView(dt);
            string[] strColumns = getColumnNames(dt);
            dt = dv.ToTable(true, strColumns);
            return dt;
        }

        // Reference: http://www.techno-soft.com/index.php?/how-to-get-column-names-of-a-datatable-in-c
        private static string[] getColumnNames(DataTable table)
        {
            if (table != null)
            {
                List<string> column = new List<string>();

                foreach (DataColumn col in table.Columns)
                {
                    column.Add(col.ColumnName);
                }
                return column.ToArray();
            }
            return null;
        }

        protected internal static bool IsValidTelephone(string telephone)
        {
            RegexUtilities util = new RegexUtilities();
            bool _isValid = util.IsValidTelephone(telephone);
            util = null;
            logger.Log(NLog.LogLevel.Debug, telephone + " - " + _isValid);
            return _isValid;
        }
    }
}
