/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;

namespace Matt40k.SIMSBulkImport.Contact
{
    public class ResultsImport
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public DataTable CreateResultTable
        {
            get
            {
                logger.Log(NLog.LogLevel.Debug, "Generating Contact Result table");

                DataTable contactTable = new DataTable("Contact Import Results");
                contactTable.Columns.Add(new DataColumn("Surname", typeof(string)));
                contactTable.Columns.Add(new DataColumn("Forename", typeof(string)));
                contactTable.Columns.Add(new DataColumn("Postcode", typeof(string)));
                contactTable.Columns.Add(new DataColumn("Town", typeof(string)));
                contactTable.Columns.Add(new DataColumn("PersonID", typeof(string)));
                contactTable.Columns.Add(new DataColumn("Result", typeof(string)));
                contactTable.Columns.Add(new DataColumn("Item", typeof(string)));
                contactTable.Columns.Add(new DataColumn("Value", typeof(string)));
                contactTable.Columns.Add(new DataColumn("Notes", typeof(string)));
                return contactTable;
            }
        }
    }
}
