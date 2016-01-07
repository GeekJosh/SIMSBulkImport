using System;
using System.Data;
using NLog;

namespace Matt40k.SIMSBulkImport.Staff
{
    public class ResultsImport
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private DataTable staffTable;

        public ResultsImport()
        {
            logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Staff.ResultsImport()");
            CreateResultTable();
        }

        public DataTable GetStaffResultsTable
        {
            get { return staffTable; }
        }

        private void CreateResultTable()
        {
            logger.Log(LogLevel.Debug, "Generating Staff Result table");

            staffTable = new DataTable("Staff Import Results");
            staffTable.Columns.Add(new DataColumn("Surname", typeof (string)));
            staffTable.Columns.Add(new DataColumn("Forename", typeof (string)));
            staffTable.Columns.Add(new DataColumn("Gender", typeof (string)));
            staffTable.Columns.Add(new DataColumn("PersonID", typeof (string)));
            staffTable.Columns.Add(new DataColumn("Result", typeof (string)));
            staffTable.Columns.Add(new DataColumn("Item", typeof (string)));
            staffTable.Columns.Add(new DataColumn("Value", typeof (string)));
            staffTable.Columns.Add(new DataColumn("Notes", typeof (string)));
            staffTable.Columns.Add(new DataColumn("Title", typeof (string)));
            staffTable.Columns.Add(new DataColumn("Staff-Code", typeof (string)));
            staffTable.Columns.Add(new DataColumn("Date-of-Birth", typeof (string)));
        }

        public bool AddToResultsTable(string surname, string forename, string gender, string staffcode, string dob,
            string personID, string result, string item, string value, string notes, string title)
        {
            try
            {
                DataRow newrow = staffTable.NewRow();
                newrow["Surname"] = surname;
                newrow["Forename"] = forename;
                newrow["Gender"] = gender;
                newrow["Staff-Code"] = staffcode;
                newrow["Date-of-Birth"] = dob;
                newrow["Title"] = personID;
                newrow["PersonID"] = personID;
                newrow["Result"] = result;
                newrow["Item"] = item;
                newrow["Value"] = value;
                newrow["Notes"] = notes;
                staffTable.Rows.Add(newrow);
            }
            catch (Exception AddToResultsTable_Exception)
            {
                logger.Log(LogLevel.Error, AddToResultsTable_Exception);
                return false;
            }
            return true;
        }
    }
}