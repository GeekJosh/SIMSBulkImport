/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;

namespace Matt40k.SIMSBulkImport
{
    public class ResultsImport
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private Contact.ResultsImport _contactResults;
        private Pupil.ResultsImport _pupilResults;
        private Staff.ResultsImport _staffResults;

        public ResultsImport()
        {
            CreateResultsDataTable();
        }

        private void CreateResultsDataTable()
        {
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

        public bool AddToResultsTable(string surname, string forename, string personID, string result, string item, string value, string notes)
        {
            try
            {
                switch (Switcher.PreImportClass.GetUserType)
                {
                    case Interfaces.UserType.Contact:
                        _contactResults.AddToResultsTable(surname, forename, personID, result, item, value, notes);
                        break;
                    case Interfaces.UserType.Pupil:
                        _pupilResults.AddToResultsTable(surname, forename, personID, result, item, value, notes);
                        break;
                    case Interfaces.UserType.Staff:
                        _staffResults.AddToResultsTable(surname, forename, personID, result, item, value, notes);
                        break;
                }
            }
            catch (Exception AddToResultsTable_Exception)
            {
                logger.Log(NLog.LogLevel.Error, AddToResultsTable_Exception);
                return false;
            }
            return true;
        }

        public void OpenResultsReport()
        {
            Results results = new Results(resultsTable, Switcher.PreImportClass.GetUserType);
        }

        private DataTable resultsTable
        {
            get
            {
                switch (Switcher.PreImportClass.GetUserType)
                {
                    case Interfaces.UserType.Contact:
                        return _contactResults.GetContactResultsTable;
                        break;
                    case Interfaces.UserType.Pupil:
                        return _pupilResults.GetPupilResultsTable;
                        break;
                    case Interfaces.UserType.Staff:
                        return _staffResults.GetStaffResultsTable;
                        break;
                    default:
                        logger.Log(NLog.LogLevel.Error, "ResultsImport.GetResultsTable - UserType not defined");
                        return null;
                }
            }
        }
    }
}
