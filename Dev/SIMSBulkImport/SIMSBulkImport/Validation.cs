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
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Validation.IsValidEmail(email: " + email + ")");
            RegexUtilities util = new RegexUtilities();
            bool _isValid = util.IsValidEmail(email);
            util = null;
            return _isValid;
        }

        // Reference: http://dengkefu.com/blog/programming/c-sharp-and-dot-net/remove-duplicate-records-in-a-datatable-the-easy-way.html
        internal static DataTable DeDuplicatation(DataTable dt)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Validation.DeDuplicatation(dt: " + dt + ")");
            DataView dv = new DataView(dt);
            string[] strColumns = getColumnNames(dt);
            dt = dv.ToTable(true, strColumns);
            return dt;
        }

        // Reference: http://www.techno-soft.com/index.php?/how-to-get-column-names-of-a-datatable-in-c
        private static string[] getColumnNames(DataTable table)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Validation.getColumnNames(table: " + table + ")");
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
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Validation.IsValidTelephone(telephone: " + telephone + ")");
            RegexUtilities util = new RegexUtilities();
            bool _isValid = util.IsValidTelephone(telephone);
            util = null;
            return _isValid;
        }

        protected internal static bool IsBool(string input)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Validation.IsBool(input: " + input + ")");
            bool result = false;
            try
            {
                bool.TryParse(input, out result); 
            }
            catch (Exception IsBool_Exception)
            {
                logger.Log(NLog.LogLevel.Debug, "IsBool:: " + IsBool_Exception);
            }
            return result;
        }

        protected internal static bool IsDouble(string input)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Validation.IsDouble(input: " + input + ")");
            bool result = false;
            try
            {
                double t1;
                result = double.TryParse(input, out t1);
            }
            catch (Exception IsDouble_Exception)
            {
                logger.Log(NLog.LogLevel.Debug, "IsDouble:: " + IsDouble_Exception);
            }
            return result;
        }

        protected internal static bool IsInt(string input)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Validation.IsInt(input: " + input + ")");
            bool result = false;
            try
            {
                int t1;
                result = int.TryParse(input, out t1);
            }
            catch (Exception IsInt_Exception)
            {
                logger.Log(NLog.LogLevel.Debug, "IsInt:: " + IsInt_Exception);
            }
            return result;
        }
    }
}
