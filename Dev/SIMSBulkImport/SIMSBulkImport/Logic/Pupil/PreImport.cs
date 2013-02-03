/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;

namespace Matt40k.SIMSBulkImport.Pupil
{
    public class PreImport
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public DataTable CreateDataTable
        {
            get
            {
                logger.Log(NLog.LogLevel.Info, "Generating Pupil Table");

                DataTable pupilTable = new DataTable();
                pupilTable.Columns.Add(new DataColumn("Status", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Surname", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Forename", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Gender", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Admission Number", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Date of Birth", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Year", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Registration", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("House", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Import email", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Import UDF", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("SIMS email addresses", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("SIMS UDF", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("PersonID", typeof(string)));
                return pupilTable;
            }
        }
    }
}
