/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;

namespace Matt40k.SIMSBulkImport
{
    public class PreImport
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private bool ignoreFirstRow;
        private DataTable importDataTable;
        private string firstname;
        private string surname;
        private string email;
        private string staffcode;
        private string gender;
        private string title;
        private string udf;
        private string year;
        private string reg;
        private string house;
        private string town;
        private string postcode;
        private Contact.PreImport _contactPre;
        private Pupil.PreImport _pupilPre;
        private Staff.PreImport _staffPre;

        public int GetImportFileRecordCount
        {
            get
            {
                int importFileRecordCount = importDataTable.Rows.Count;
                logger.Log(NLog.LogLevel.Debug, "RecordCount: " + importFileRecordCount);
                return importFileRecordCount;
            }
        }

        public bool SetMatchIgnoreFirstRow
        {
            set
            {
                logger.Log(NLog.LogLevel.Debug, "ignoreFirstRow: " + value);
                ignoreFirstRow = value;
            }
        }

        public DataTable SetImportDataset
        {
            set
            {
                // Remove First row (header) if set
                if (ignoreFirstRow)
                    value.Rows[0].Delete();

                // Remove duplicate entries
                value = Validation.DeDuplicatation(value);

                switch (Switcher.SimsApiClass.GetUserType)
                {
                    case SIMSAPI.UserType.Contact:
                        _contactPre = new Contact.PreImport();
                        _contactPre.SetImportDataTable = value;
                        break;
                    case SIMSAPI.UserType.Pupil:
                        _pupilPre = new Pupil.PreImport();
                        _pupilPre.SetImportDataTable = value;
                       break;
                    case SIMSAPI.UserType.Staff:
                        _staffPre = new Staff.PreImport();
                        _staffPre.SetImportDataTable = value;
                        break;
                }
                // TOBE REMOVED
                importDataTable = value;
            }
        }

        // TOBE REMOVED
        public DataTable GetImportDataTable
        {
            get
            {
                return importDataTable;
            }
        }

        public DataTable CreateDataTable
        {
            get
            {
                switch (Switcher.SimsApiClass.GetUserType)
                {
                    case SIMSAPI.UserType.Contact:
                        return _contactPre.CreateDataTable;
                    case SIMSAPI.UserType.Pupil:
                        return _pupilPre.CreateDataTable;
                    case SIMSAPI.UserType.Staff:
                        return _staffPre.CreateDataTable;
                    default:
                        return null;
                }
            }
        }

        public DataTable AddToDataTable(int recordupto)
        {
            switch (Switcher.SimsApiClass.GetUserType)
            {
                case SIMSAPI.UserType.Contact:
                    return _contactPre.AddToDataTable(recordupto);
                case SIMSAPI.UserType.Pupil:
                    return _pupilPre.AddToDataTable(recordupto);
                case SIMSAPI.UserType.Staff:
                    return _staffPre.AddToDataTable(recordupto);
                default:
                    return null;
            }
        }

        public string SetMatchFirstname
        {
            set { firstname = value; }
        }

        public string SetMatchSurname
        {
            set { surname = value; }
        }

        public string SetMatchEmail
        {
            set { email = value; }
        }

        public string SetMatchStaffcode
        {
            set { staffcode = value; }
        }

        public string SetMatchGender
        {
            set { gender = value; }
        }

        public string SetMatchTitle
        {
            set { title = value; }
        }

        public string SetMatchUDF
        {
            set { udf = value; }
        }

        public string SetMatchYear
        {
            set { year = value; }
        }

        public string SetMatchReg
        {
            set { reg = value; }
        }

        public string SetMatchHouse
        {
            set { house = value; }
        }

        public string SetMatchTown
        {
            set { town = value; }
        }

        public string SetMatchPostcode
        {
            set { postcode = value; }
        }

        public string GetMatchFirstname
        {
            get { return firstname; }
        }

        public string GetMatchSurname
        {
            get { return surname; }
        }

        public string GetMatchEmail
        {
            get { return email; }
        }

        public string GetMatchStaffcode
        {
            get { return staffcode; }
        }

        public string GetMatchGender
        {
            get { return gender; }
        }

        public string GetMatchTitle
        {
            get { return title; }
        }

        public string GetMatchUDF
        {
            get { return udf; }
        }

        public string GetMatchYear
        {
            get { return year; }
        }

        public string GetMatchReg
        {
            get { return reg; }
        }

        public string GetMatchHouse
        {
            get { return house; }
        }

        public string GetMatchTown
        {
            get { return town; }
        }

        public string GetMatchPostcode
        {
            get { return postcode; }
        }
    }
}
