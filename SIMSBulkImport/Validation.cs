using System;
using System.Collections.Generic;
using System.Data;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    internal class Validation
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected internal static bool IsValidEmail(string email)
        {
            logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Validation.IsValidEmail(email: " + email + ")");
            var util = new RegexUtilities();
            bool _isValid = util.IsValidEmail(email);
            util = null;
            return _isValid;
        }

        // Reference: http://dengkefu.com/blog/programming/c-sharp-and-dot-net/remove-duplicate-records-in-a-datatable-the-easy-way.html
        internal static DataTable DeDuplicatation(DataTable dt)
        {
            logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Validation.DeDuplicatation(dt: " + dt + ")");
            var dv = new DataView(dt);
            string[] strColumns = getColumnNames(dt);
            dt = dv.ToTable(true, strColumns);
            return dt;
        }

        // Reference: http://www.techno-soft.com/index.php?/how-to-get-column-names-of-a-datatable-in-c
        private static string[] getColumnNames(DataTable table)
        {
            logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Validation.getColumnNames(table: " + table + ")");
            if (table != null)
            {
                var column = new List<string>();

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
            logger.Log(LogLevel.Debug,
                "Trace:: Matt40k.SIMSBulkImport.Validation.IsValidTelephone(telephone: " + telephone + ")");
            var util = new RegexUtilities();
            bool _isValid = util.IsValidTelephone(telephone);
            util = null;
            return _isValid;
        }

        protected internal static bool IsBool(string input)
        {
            logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Validation.IsBool(input: " + input + ")");
            bool result = false;
            try
            {
                bool.TryParse(input, out result);
            }
            catch (Exception IsBool_Exception)
            {
                logger.Log(LogLevel.Debug, "IsBool:: " + IsBool_Exception);
            }
            return result;
        }

        protected internal static bool IsDouble(string input)
        {
            logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Validation.IsDouble(input: " + input + ")");
            bool result = false;
            try
            {
                double t1;
                result = double.TryParse(input, out t1);
            }
            catch (Exception IsDouble_Exception)
            {
                logger.Log(LogLevel.Debug, "IsDouble:: " + IsDouble_Exception);
            }
            return result;
        }

        protected internal static bool IsInt(string input)
        {
            logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Validation.IsInt(input: " + input + ")");
            bool result = false;
            try
            {
                int t1;
                result = int.TryParse(input, out t1);
            }
            catch (Exception IsInt_Exception)
            {
                logger.Log(LogLevel.Debug, "IsInt:: " + IsInt_Exception);
            }
            return result;
        }
    }
}