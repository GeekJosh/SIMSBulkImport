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

        private DataTable resultTable;
        private Contact.ResultsImport _contactResults;
        private Pupil.ResultsImport _pupilResults;
        private Staff.ResultsImport _staffResults;

        public void CreateResultsDataTable()
        {
            switch (Switcher.PreImportClass.GetUserType)
            {
                case Interfaces.UserType.Contact:
                    _contactResults = new Contact.ResultsImport();
                    resultTable = _contactResults.CreateResultTable;
                    break;
                case Interfaces.UserType.Pupil:
                    _pupilResults = new Pupil.ResultsImport();
                    resultTable = _pupilResults.CreateResultTable;
                    break;
                case Interfaces.UserType.Staff:
                    _staffResults = new Staff.ResultsImport();
                    resultTable = _staffResults.CreateResultTable;
                    break;
            }
        }
    }
}
