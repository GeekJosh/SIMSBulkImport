/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;

namespace Matt40k.SIMSBulkImport.Pupil
{
    public class Results
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public DataTable CreateResultTable
        {
            get
            {
                logger.Log(NLog.LogLevel.Debug, "Generating Pupil Result table");

                DataTable pupilTable = new DataTable("Pupil Import Results");
                pupilTable.Columns.Add(new DataColumn("Surname", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Forename", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Gender", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Admission_Number", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("DOB", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Year", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Registration", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("House", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("PersonID", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Result", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Item", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Value", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Notes", typeof(string)));
                return pupilTable;
            }
        }
    }
}
