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
        private string simsudf;
        private string emailLocation;
        private Contact.PreImport _contactPre;
        private Pupil.PreImport _pupilPre;
        private Staff.PreImport _staffPre;
        private SIMSAPI.UserType userType;
        private int userFilter;

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

                switch (Switcher.PreImportClass.GetUserType)
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
            }
        }

        public DataTable CreateDataTable
        {
            get
            {
                switch (Switcher.PreImportClass.GetUserType)
                {
                    case UserType.Contact:
                        return _contactPre.CreateDataTable;
                    case Switcher.SimsApiClass.UserType.Pupil:
                        return _pupilPre.CreateDataTable;
                    case Switcher.SimsApiClass.UserType.Staff:
                        return _staffPre.CreateDataTable;
                    default:
                        return null;
                }
            }
        }

        public DataTable AddToDataTable(int recordupto)
        {
            switch (Switcher.PreImportClass.GetUserType)
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

        public string SetMatchSIMSUDF
        {
            set { simsudf = value; }
        }

        public string SetMatchEmailLocation
        {
            set { emailLocation = value; }
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

        public string GetStatus(string personid,
                                string email, string simsEmail,
                                string udf, string simsUdf)
        {

            if (personid == "0") { return "Missing"; }
            if (!IsNotDuplicate(personid)) { return "Duplicate"; }

            bool emailImport = false;
            bool udfImport = false;

            if (!string.IsNullOrEmpty(email))
            {
                emailImport = IsEmailImport(email, simsEmail);
            }
            if (!string.IsNullOrEmpty(udf))
            {
                udfImport = IsUdfImport(udf, simsUdf);
            }

            string status = null;
            if (emailImport) { status = "Import email"; }
            if (udfImport)
            {
                if (string.IsNullOrEmpty(status))
                {
                    status = "Import UDF";
                }
                else
                {
                    status = status + ", UDF";
                }
            }
            if (string.IsNullOrEmpty(status)) { return "Ignore"; }
            return status;
        }

        private bool IsNotDuplicate(string personid)
        {
            string[] pids = personid.Split(',');
            int noPids = pids.Length;
            if (noPids == 1) { return true; }
            return false;
        }

        private bool IsEmailImport(string import, string current)
        {
            if (!IsNotSame(import, current)) { return false; }
            return true;
        }

        private bool IsUdfImport(string import, string current)
        {
            if (!IsNotSame(import, current)) { return false; }
            return true;
        }

        private bool IsNotSame(string import, string current)
        {
            if (!string.IsNullOrWhiteSpace(current))
            {
                string[] emails = current.Split(',');
                switch (emails.Length)
                {
                    case 0:
                        return true;
                    case 1:
                        if (current == import)
                        {
                            return false;
                        }
                        return true;
                    default:
                        foreach (string email in emails)
                        {
                            if (email == import)
                            {
                                return false;
                            }
                        }
                        return true;
                }
            }
            return true;
        }        

        public SIMSAPI.UserType SetUserType
        {
            set
            {
                userType = value;
                Switcher.SimsApiClass.SetUserType = value;
            }
        }

        public SIMSAPI.UserType GetUserType
        {
            get
            {
                return userType;
            }
        }

        public int SetUserFilter
        {
            set
            {
                userFilter = value;
            }
        }

        public int GetUserFilter
        {
            get
            {
                return userFilter;
            }
        }
    }
}
