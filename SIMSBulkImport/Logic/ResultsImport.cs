using System;
using System.Data;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    public class ResultsImport
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private Contact.ResultsImport _contactResults;
        private Pupil.ResultsImport _pupilResults;
        private Staff.ResultsImport _staffResults;

        public ResultsImport()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ResultsImport()");
            CreateResultsDataTable();
        }

        private DataTable resultsTable
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ResultsImport.resultsTable(GET)");
                switch (Switcher.PreImportClass.GetUserType)
                {
                    case Interfaces.UserType.Contact:
                        return _contactResults.GetContactResultsTable;
                    case Interfaces.UserType.Pupil:
                        return _pupilResults.GetPupilResultsTable;
                    case Interfaces.UserType.Staff:
                        return _staffResults.GetStaffResultsTable;
                    default:
                        logger.Log(LogLevel.Error, "ResultsImport.GetResultsTable - UserType not defined");
                        return null;
                }
            }
        }

        private void CreateResultsDataTable()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ResultsImport.CreateResultsDataTable()");
            switch (Switcher.PreImportClass.GetUserType)
            {
                case Interfaces.UserType.Contact:
                    _contactResults = new Contact.ResultsImport();
                    break;
                case Interfaces.UserType.Pupil:
                    _pupilResults = new Pupil.ResultsImport();
                    break;
                case Interfaces.UserType.Staff:
                    _staffResults = new Staff.ResultsImport();
                    break;
            }
        }

        public bool AddToResultsTable(
            string personID, string result, string item, string value, string notes,
            string surname, string forename, string title, string gender, string staffCode,
            string dob, string admissionNumber, string year, string registration,
            string house, string postCode, string town, string main, string primary,
            string location, string device
            )
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.ResultsImport.AddToResultsTable(personid: " + personID + ", result: " +
                result + ", item: " + item + ", value: " + value + ", notes: " + notes + ", surname ," + surname +
                ",  forename ," + forename + "title ," + title + " gender ," + gender + " staffCode ," + staffCode +
                " dob ," + dob + " admissionNumber ," + admissionNumber + " year ," + year + " registration ," +
                registration + " house ," + house + " postCode ," + postCode + " town ," + town + ", main: " + main +
                ", primary: " + primary + ", location: " + location + ", device: " + device + ")");
            string friendlyResult;
            switch (result)
            {
                case "False":
                    friendlyResult = "Not imported";
                    break;
                case "True":
                    friendlyResult = "Imported";
                    break;
                default:
                    friendlyResult = result;
                    break;
            }

            try
            {
                switch (Switcher.PreImportClass.GetUserType)
                {
                    case Interfaces.UserType.Contact:
                        _contactResults.AddToResultsTable(surname, forename, postCode, town, personID, friendlyResult,
                            item, value, notes);
                        break;
                    case Interfaces.UserType.Pupil:
                        _pupilResults.AddToResultsTable(surname, forename, gender, admissionNumber, dob, year,
                            registration, house, personID, friendlyResult, item, value, notes);
                        break;
                    case Interfaces.UserType.Staff:
                        _staffResults.AddToResultsTable(surname, forename, gender, staffCode, dob, personID,
                            friendlyResult, item, value, notes, title);
                        break;
                }
            }
            catch (Exception AddToResultsTable_Exception)
            {
                logger.Log(LogLevel.Error, AddToResultsTable_Exception);
                return false;
            }
            return true;
        }

        public void OpenResultsReport()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ResultsImport.OpenResultsReport()");
            var results = new Results(resultsTable, Switcher.PreImportClass.GetUserType);
        }
    }
}