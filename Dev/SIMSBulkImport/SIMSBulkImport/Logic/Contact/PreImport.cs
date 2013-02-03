/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;

namespace Matt40k.SIMSBulkImport.Contact
{
    public class PreImport
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public DataTable CreateDataTable
        {
            get
            {
                logger.Log(NLog.LogLevel.Debug, "Generating Contact Table");

                DataTable contactTable = new DataTable();
                contactTable.Columns.Add(new DataColumn("Status", typeof(string)));
                contactTable.Columns.Add(new DataColumn("Surname", typeof(string)));
                contactTable.Columns.Add(new DataColumn("Forename", typeof(string)));
                contactTable.Columns.Add(new DataColumn("Postcode", typeof(string)));
                contactTable.Columns.Add(new DataColumn("Town", typeof(string)));
                contactTable.Columns.Add(new DataColumn("Import email", typeof(string)));
                contactTable.Columns.Add(new DataColumn("Import UDF", typeof(string)));
                contactTable.Columns.Add(new DataColumn("SIMS email addresses", typeof(string)));
                contactTable.Columns.Add(new DataColumn("SIMS UDF", typeof(string)));
                contactTable.Columns.Add(new DataColumn("PersonID", typeof(string)));
                return contactTable;
            }
        }
    }
}
