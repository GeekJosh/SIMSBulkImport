/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;

namespace Matt40k.SIMSBulkImport.Staff
{
    public class PreImport
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public DataTable CreateDataTable
        {
            get
            {
                logger.Log(NLog.LogLevel.Info, "Generating Staff Table");

                DataTable staffTable = new DataTable();
                staffTable.Columns.Add(new DataColumn("Status", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Surname", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Forename", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Title", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Gender", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Staff Code", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Date of Birth", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Import email", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Import UDF", typeof(string)));
                staffTable.Columns.Add(new DataColumn("SIMS email addresses", typeof(string)));
                staffTable.Columns.Add(new DataColumn("SIMS UDF", typeof(string)));
                staffTable.Columns.Add(new DataColumn("PersonID", typeof(string)));
                return staffTable;
            }
        }
    }
}
