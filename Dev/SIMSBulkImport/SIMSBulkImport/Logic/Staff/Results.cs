/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;

namespace Matt40k.SIMSBulkImport.Staff
{
    public class Results
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public DataTable CreateResultTable
        {
            get
            {
                logger.Log(NLog.LogLevel.Debug, "Generating Staff Result table");

                DataTable staffTable = new DataTable("Staff Import Results");
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
                return staffTable;
            }
        }
    }
}
