/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;

namespace Matt40k.SIMSBulkImport.Staff
{
    public class ResultsImport
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private DataTable staffTable;

        public ResultsImport()
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Staff.ResultsImport()");
            CreateResultTable();
        }

        private void CreateResultTable()
        {
            logger.Log(NLog.LogLevel.Debug, "Generating Staff Result table");

            staffTable = new DataTable("Staff Import Results");
            staffTable.Columns.Add(new DataColumn("Surname", typeof(string)));
            staffTable.Columns.Add(new DataColumn("Forename", typeof(string)));
            staffTable.Columns.Add(new DataColumn("Gender", typeof(string)));
            staffTable.Columns.Add(new DataColumn("Staff_Code", typeof(string)));
            staffTable.Columns.Add(new DataColumn("DOB", typeof(string)));
            staffTable.Columns.Add(new DataColumn("PersonID", typeof(string)));
            staffTable.Columns.Add(new DataColumn("Result", typeof(string)));
            staffTable.Columns.Add(new DataColumn("Item", typeof(string)));
            staffTable.Columns.Add(new DataColumn("Value", typeof(string)));
            staffTable.Columns.Add(new DataColumn("Notes", typeof(string)));
        }

        public bool AddToResultsTable(string surname, string forename, string personID, string result, string item, string value, string notes)
        {
            try
            {
                DataRow newrow = staffTable.NewRow();
                newrow["Surname"] = surname;
                newrow["Forename"] = forename;
                newrow["Gender"] = "";
                newrow["Staff_Code"] = "";
                newrow["DOB"] = "";
                newrow["PersonID"] = personID;
                newrow["Result"] = result;
                newrow["Item"] = item;
                newrow["Value"] = value;
                newrow["Notes"] = notes;
                staffTable.Rows.Add(newrow);
            }
            catch (Exception AddToResultsTable_Exception)
            {
                logger.Log(NLog.LogLevel.Error, AddToResultsTable_Exception);
                return false;
            }
            return true;
        }

        public DataTable GetStaffResultsTable
        {
            get
            {
                return staffTable;
            }
        }
    }
}