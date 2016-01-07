using System;
using System.Data;
using NLog;

namespace Matt40k.SIMSBulkImport.Pupil
{
    public class ResultsImport
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private DataTable pupilTable;

        public ResultsImport()
        {
            logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Pupil.ResultsImport()");
            CreateResultTable();
        }

        public DataTable GetPupilResultsTable
        {
            get { return pupilTable; }
        }

        private void CreateResultTable()
        {
            logger.Log(LogLevel.Debug, "Generating Pupil Result table");

            pupilTable = new DataTable("Pupil Import Results");
            pupilTable.Columns.Add(new DataColumn("Surname", typeof (string)));
            pupilTable.Columns.Add(new DataColumn("Forename", typeof (string)));
            pupilTable.Columns.Add(new DataColumn("Gender", typeof (string)));
            pupilTable.Columns.Add(new DataColumn("Admission-Number", typeof (string)));
            pupilTable.Columns.Add(new DataColumn("Date-of-Birth", typeof (string)));
            pupilTable.Columns.Add(new DataColumn("Year", typeof (string)));
            pupilTable.Columns.Add(new DataColumn("Registration", typeof (string)));
            pupilTable.Columns.Add(new DataColumn("House", typeof (string)));
            pupilTable.Columns.Add(new DataColumn("PersonID", typeof (string)));
            pupilTable.Columns.Add(new DataColumn("Result", typeof (string)));
            pupilTable.Columns.Add(new DataColumn("Item", typeof (string)));
            pupilTable.Columns.Add(new DataColumn("Value", typeof (string)));
            pupilTable.Columns.Add(new DataColumn("Notes", typeof (string)));
        }

        public bool AddToResultsTable(string surname, string forename, string gender, string admissionnumber, string dob,
            string year, string registration, string house, string personID, string result, string item, string value,
            string notes)
        {
            try
            {
                DataRow newrow = pupilTable.NewRow();
                newrow["Surname"] = surname;
                newrow["Forename"] = forename;
                newrow["Gender"] = gender;
                newrow["Admission-Number"] = admissionnumber;
                newrow["Date-of-Birth"] = dob;
                newrow["Year"] = year;
                newrow["Registration"] = registration;
                newrow["House"] = house;
                newrow["PersonID"] = personID;
                newrow["Result"] = result;
                newrow["Item"] = item;
                newrow["Value"] = value;
                newrow["Notes"] = notes;
                pupilTable.Rows.Add(newrow);
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