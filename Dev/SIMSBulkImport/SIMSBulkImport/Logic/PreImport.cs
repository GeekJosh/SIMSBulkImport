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
        private string telephone;
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
        private string telephoneLocation;
        private Contact.PreImport _contactPre;
        private Pupil.PreImport _pupilPre;
        private Staff.PreImport _staffPre;
        private Interfaces.UserType userType;
        private string userFilter;
        private int importFileRecordCount;

        public int GetImportFileRecordCount
        {
            get
            {
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

                // Set the (filtered) record (row) count
                importFileRecordCount = value.Rows.Count;

                switch (Switcher.PreImportClass.GetUserType)
                {
                        
                    case Interfaces.UserType.Contact:
                        _contactPre = new Contact.PreImport();
                        _contactPre.SetImportDataTable = value;
                        break;
                    case Interfaces.UserType.Pupil:  
                        _pupilPre = new Pupil.PreImport();
                        _pupilPre.SetImportDataTable = value;
                        break;
                    case Interfaces.UserType.Staff:
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
                    case Interfaces.UserType.Contact:
                        return _contactPre.CreateDataTable;
                    case Interfaces.UserType.Pupil:
                        return _pupilPre.CreateDataTable;
                    case Interfaces.UserType.Staff:
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
                case Interfaces.UserType.Contact:
                    return _contactPre.AddToDataTable(recordupto);
                case Interfaces.UserType.Pupil:
                    return _pupilPre.AddToDataTable(recordupto);
                case Interfaces.UserType.Staff:
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

        public string SetMatchTelephone
        {
            set { telephone = value; }
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
            set
            {
                Switcher.SimsApiClass.SetEmailLocation = value;
            }
        }

        public string SetMatchTelephoneLocation
        {
            set { telephoneLocation = value; }
        }

        public string SetMatchEmailMainId
        {
            set
            {
                int mainId = 0;
                string cleanValue = value.Substring(38);
                switch (cleanValue)
                {
                    case "Yes":
                        mainId = 0;
                        break;
                    case "Yes (overwrite)":
                        mainId = 1;
                        break;
                    case "No":
                        mainId = 2;
                        break;
                }
                logger.Log(NLog.LogLevel.Trace, "Trace:: SimsApiClass.SetEmailMainId:: " + mainId);
                Switcher.SimsApiClass.SetEmailMainId = mainId;
            }
        }

        public string SetMatchEmailPrimaryId
        {
            set
            {
                int primaryId = 0;
                string cleanValue = value.Substring(38);
                //logger.Log(NLog.LogLevel.Trace, "Trace::" + cleanValue);
                switch (cleanValue)
                {
                    case "Yes":
                        primaryId = 0;
                        break;
                    case "Yes (overwrite)":
                        primaryId = 1;
                        break;
                    case "No":
                        primaryId = 2;
                        break;
                }
                logger.Log(NLog.LogLevel.Trace, "Trace:: SimsApiClass.SetEmailPrimaryId:: " + primaryId);
                Switcher.SimsApiClass.SetEmailPrimaryId = primaryId;
            }
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

        public string GetMatchTelephone
        {
            get { return telephone; }
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

        public string GetMatchTelephoneLocation
        {
            get { return telephoneLocation; }
        }

        public string GetStatus(string personid,
                                string email, string simsEmail,
                                string udf, string simsUdf,
                                string telephone, string simsTelephone)
        {

            if (personid == "0") { return "Missing"; }
            if (!IsNotDuplicate(personid)) { return "Duplicate"; }

            bool emailImport = false;
            bool udfImport = false;
            bool telephoneImport = false;

            if (!string.IsNullOrEmpty(email))
            {
                emailImport = IsImport(email, simsEmail);
            }
            if (!string.IsNullOrEmpty(udf))
            {
                udfImport = IsImport(udf, simsUdf);
            }
            if (!string.IsNullOrEmpty(telephone))
            {
                telephoneImport = IsImport(telephone, simsTelephone);
            }

            string status = null;
            if (emailImport) { status = "Import Email"; }
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
            if (telephoneImport)
            {
                if (string.IsNullOrEmpty(status))
                {
                    status = "Import Telephone";
                }
                else
                {
                    status = status + ", Telephone";
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

        private bool IsImport(string import, string current)
        {
            if (!IsNotSame(import, current)) { return false; }
            return true;
        }

        private bool IsNotSame(string import, string current)
        {
            if (!string.IsNullOrWhiteSpace(current))
            {
                string[] parts = current.Split(',');
                switch (parts.Length)
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
                        foreach (string part in parts)
                        {
                            if (part == import)
                            {
                                return false;
                            }
                        }
                        return true;
                }
            }
            return true;
        }

        public Interfaces.UserType SetUserType
        {
            set
            {
                userType = value;
                Switcher.SimsApiClass.SetUserType = value;
            }
        }

        public Interfaces.UserType GetUserType
        {
            get
            {
                return userType;
            }
        }

        public string SetUserFilter
        {
            set
            {
                userFilter = value;
                Switcher.SimsApiClass.SetUserFilter = value;
            }
        }
    }
}
