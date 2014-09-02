/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;
using NLog;

namespace Matt40k.SIMSBulkImport.Contact
{
    public class ResultsImport
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private DataTable contactTable;

        public ResultsImport()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Contact.ResultsImport()");
            CreateResultTable();
        }

        public DataTable GetContactResultsTable
        {
            get { return contactTable; }
        }

        private void CreateResultTable()
        {
            logger.Log(LogLevel.Debug, "Generating Contact Result table");

            contactTable = new DataTable("Contact Import Results");
            contactTable.Columns.Add(new DataColumn("Surname", typeof (string)));
            contactTable.Columns.Add(new DataColumn("Forename", typeof (string)));
            contactTable.Columns.Add(new DataColumn("Postcode", typeof (string)));
            contactTable.Columns.Add(new DataColumn("Town", typeof (string)));
            contactTable.Columns.Add(new DataColumn("PersonID", typeof (string)));
            contactTable.Columns.Add(new DataColumn("Result", typeof (string)));
            contactTable.Columns.Add(new DataColumn("Item", typeof (string)));
            contactTable.Columns.Add(new DataColumn("Value", typeof (string)));
            contactTable.Columns.Add(new DataColumn("Notes", typeof (string)));
        }

        public bool AddToResultsTable(string surname, string forename, string postcode, string town, string personID,
            string result, string item, string value, string notes)
        {
            try
            {
                DataRow newrow = contactTable.NewRow();
                newrow["Surname"] = surname;
                newrow["Forename"] = forename;
                newrow["Postcode"] = postcode;
                newrow["Town"] = town;
                newrow["PersonID"] = personID;
                newrow["Result"] = result;
                newrow["Item"] = item;
                newrow["Value"] = value;
                newrow["Notes"] = notes;
                contactTable.Rows.Add(newrow);
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