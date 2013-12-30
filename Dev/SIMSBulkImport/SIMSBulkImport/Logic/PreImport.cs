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
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetImportFileRecordCount(GET)");
                return importFileRecordCount;
            }
        }

        public bool SetMatchIgnoreFirstRow
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchIgnoreFirstRow(SET: " + value + ")");
                ignoreFirstRow = value;
            }
        }

        public DataTable SetImportDataset
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetImportDataset(SET: " + value + ")");

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
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.CreateDataTable(GET)");

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
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.AddToDataTable(recordupto: " + recordupto + ")");
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
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchFirstname(SET: " + value + ")");
                firstname = value;
            }
        }

        public string SetMatchSurname
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchSurname(SET: " + value + ")");
                surname = value;
            }
        }

        public string SetMatchEmail
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchEmail(SET: " + value + ")");
                email = value;
            }
        }

        public string SetMatchTelephone
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchTelephone(SET: " + value + ")");
                telephone = value;
            }
        }

        public string SetMatchStaffcode
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchStaffcode(SET: " + value + ")");
                staffcode = value;
            }
        }

        public string SetMatchGender
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchGender(SET: " + value + ")");
                gender = value;
            }
        }

        public string SetMatchTitle
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchTitle(SET: " + value + ")");
                title = value;
            }
        }

        public string SetMatchUDF
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchUDF(SET: " + value + ")");
                udf = value;
            }
        }

        public string SetMatchYear
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchYear(SET: " + value + ")");
                year = value;
            }
        }

        public string SetMatchReg
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchReg(SET: " + value + ")");
                reg = value;
            }
        }

        public string SetMatchHouse
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchHouse(SET: " + value + ")");
                house = value;
            }
        }

        public string SetMatchTown
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchTown(SET: " + value + ")");
                town = value;
            }
        }

        public string SetMatchPostcode
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchPostcode(SET: " + value + ")");
                postcode = value;
            }
        }

        public string SetMatchSIMSUDF
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchSIMSUDF(SET: " + value + ")");
                Switcher.SimsApiClass.SetSIMSUDF = value; 
            }
        }

        public string SetMatchEmailLocation
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchEmailLocation(SET: " + value + ")");
                Switcher.SimsApiClass.SetEmailLocation = value;
            }
        }

        public string SetMatchEmailMainId
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchEmailMainId(SET: " + value + ")");
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
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchEmailPrimaryId(SET: " + value + ")");
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

        public string SetMatchTelephoneLocation
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchTelephoneLocation(SET: " + value + ")");
                Switcher.SimsApiClass.SetTelephoneLocation = value;
            }
        }

        public string SetMatchTelephoneMainId
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchTelephoneMainId(SET: " + value + ")");
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
                logger.Log(NLog.LogLevel.Trace, "Trace:: SimsApiClass.SetMatchTelephoneMainId:: " + mainId);
                Switcher.SimsApiClass.SetTelephoneMainId = mainId;
            }
        }

        public string SetMatchTelephonePrimaryId
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchTelephonePrimaryId(SET: " + value + ")");
                int primaryId = 0;
                string cleanValue = value.Substring(38);
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
                Switcher.SimsApiClass.SetTelephonePrimaryId = primaryId;
            }
        }

        public string GetMatchFirstname
        {
            get
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchFirstname(GET)");
                return firstname;
            }
        }

        public string GetMatchSurname
        {
            get 
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchSurname(GET)");
                return surname; 
            }
        }

        public string GetMatchEmail
        {
            get 
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchEmail(GET)");
                return email;
            }
        }

        public string GetMatchTelephone
        {
            get
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchTelephone(GET)");
                return telephone;
            }
        }

        public string GetMatchStaffcode
        {
            get 
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchStaffcode(GET)");
                return staffcode;
            }
        }

        public string GetMatchGender
        {
            get
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchGender(GET)");
                return gender; 
            }
        }

        public string GetMatchTitle
        {
            get
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchTitle(GET)");
                return title;
            }
        }

        public string GetMatchUDF
        {
            get
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchUDF(GET)");
                return udf;
            }
        }

        public string GetMatchYear
        {
            get 
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchYear(GET)");
                return year;
            }
        }

        public string GetMatchReg
        {
            get 
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchReg(GET)"); 
                return reg;
            }
        }

        public string GetMatchHouse
        {
            get
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchHouse(GET)"); 
                return house;
            }
        }

        public string GetMatchTown
        {
            get
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchTown(GET)"); 
                return town; 
            }
        }

        public string GetMatchPostcode
        {
            get
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchPostcode(GET)"); 
                return postcode;
            }
        }

        public string GetStatus(string personid,
                                string email, string simsEmail,
                                string udf, string simsUdf,
                                string telephone, string simsTelephone)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetStatus(personid: " + personid + ", email: " + email + ", simsEmail: " + simsEmail + ", udf: " + udf + ", simsUdf: " + simsUdf + ", telephone: " + telephone + ", simsTelephone: " + simsTelephone +")"); 
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
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.IsNotDuplicate(personid: " + personid + ")");
            string[] pids = personid.Split(',');
            int noPids = pids.Length;
            if (noPids == 1) { return true; }
            return false;
        }

        private bool IsImport(string import, string current)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.IsNotDuplicate(import: " + import + ", current: " + current + ")");
            if (!IsNotSame(import, current)) { return false; }
            return true;
        }

        private bool IsNotSame(string import, string current)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.IsNotDuplicate(import: " + import + ", current: " + current + ")");
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
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetUserType(SET: " + value + ")");
                userType = value;
                Switcher.SimsApiClass.SetUserType = value;
            }
        }

        public Interfaces.UserType GetUserType
        {
            get
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetUserType(GET)");
                return userType;
            }
        }

        public string SetUserFilter
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetUserFilter(SET: " + value + ")");
                userFilter = value;
                Switcher.SimsApiClass.SetUserFilter = value;
            }
        }
    }
}
