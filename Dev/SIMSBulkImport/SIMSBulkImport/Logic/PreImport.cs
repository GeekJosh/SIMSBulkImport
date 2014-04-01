/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System.Data;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    public class PreImport
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private Contact.PreImport _contactPre;
        private Pupil.PreImport _pupilPre;
        private Staff.PreImport _staffPre;
        private Interfaces.UserType userType;

        private bool ignoreFirstRow;
        //private DataTable importDataTable;
        private int importFileRecordCount;

        private string personId;
        private string email;
        private string telephone;
        private string emailNotes;
        private string telephoneNotes;
        private string firstname;
        private string gender;
        private string house;
        private string postcode;
        private string reg;
        private string staffcode;
        private string surname;
        private string title;
        private string town;
        private string udf;
        private string udfType;
        private string userFilter;
        private string year;

        private string emailMain;
        private string emailPrimary;
        private string emailLocation;
        private string telephoneMain;
        private string telephonePrimary;
        private string telephoneLocation;

        public int GetImportFileRecordCount
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetImportFileRecordCount(GET)");
                return importFileRecordCount;
            }
        }

        /// <summary>
        /// Sets if the first row should be ignored (ie it's the column names)
        /// </summary>
        public bool SetMatchIgnoreFirstRow
        {
            set
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchIgnoreFirstRow(SET: " + value + ")");
                ignoreFirstRow = value;
            }
        }

        public DataTable SetImportDataset
        {
            set
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.SetImportDataset(SET: " + value + ")");

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

        /// <summary>
        /// Creates a pre-import DataTable
        /// </summary>
        public DataTable CreateDataTable
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.CreateDataTable(GET)");

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

        #region SetMatch
        public string SetMatchPersonID
        {
            set
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchPersonID(SET: " + value + ")");
                personId = value;
            }
        }

        public string SetMatchFirstname
        {
            set
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchFirstname(SET: " + value + ")");
                firstname = value;
            }
        }

        public string SetMatchSurname
        {
            set
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchSurname(SET: " + value + ")");
                surname = value;
            }
        }

        public string SetMatchEmail
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchEmail(SET: " + value + ")");
                email = value;
            }
        }

        public string SetMatchTelephone
        {
            set
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchTelephone(SET: " + value + ")");
                telephone = value;
            }
        }

        public string SetMatchStaffcode
        {
            set
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchStaffcode(SET: " + value + ")");
                staffcode = value;
            }
        }

        public string SetMatchGender
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchGender(SET: " + value + ")");
                gender = value;
            }
        }

        public string SetMatchTitle
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchTitle(SET: " + value + ")");
                title = value;
            }
        }

        public string SetMatchUDF
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchUDF(SET: " + value + ")");
                udf = value;
            }
        }

        public string SetMatchYear
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchYear(SET: " + value + ")");
                year = value;
            }
        }

        public string SetMatchReg
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchReg(SET: " + value + ")");
                reg = value;
            }
        }

        public string SetMatchHouse
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchHouse(SET: " + value + ")");
                house = value;
            }
        }

        public string SetMatchTown
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchTown(SET: " + value + ")");
                town = value;
            }
        }

        public string SetMatchPostcode
        {
            set
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchPostcode(SET: " + value + ")");
                postcode = value;
            }
        }

        public string SetMatchSIMSUDF
        {
            set
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchSIMSUDF(SET: " + value + ")");
                Switcher.SimsApiClass.SetSIMSUDF = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SetMatchEmailLocation
        {
            set
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchEmailLocation(SET: " + value + ")");
                emailLocation = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SetMatchEmailMain
        {
            set
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchEmailMain(SET: " + value + ")");
                emailMain = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SetMatchEmailPrimary
        {
            set
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchEmailPrimary(SET: " + value + ")");
                emailPrimary = value;
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        public string SetMatchEmailNotes
        {
            set
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchEmailNotes(SET: " + value + ")");
                telephoneNotes = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SetMatchTelephoneLocation
        {
            set
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchTelephoneLocation(SET: " + value + ")");
                telephoneLocation = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SetMatchTelephoneMain
        {
            set
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchTelephoneMain(SET: " + value + ")");
                telephoneMain = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SetMatchTelephonePrimary
        {
            set
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchTelephonePrimary(SET: " + value + ")");
                telephonePrimary = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SetMatchTelephoneNotes
        {
            set
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchTelephoneNotes(SET: " + value + ")");
                emailNotes = value;
            }
        }
        #endregion

        #region GetMatch
        public string GetMatchPersonID
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchPersonID(GET)");
                return personId;
            }
        }

        public string GetMatchFirstname
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchFirstname(GET)");
                return firstname;
            }
        }

        public string GetMatchSurname
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchSurname(GET)");
                return surname;
            }
        }

        public string GetMatchEmail
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchEmail(GET)");
                return email;
            }
        }

        public string GetMatchTelephone
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchTelephone(GET)");
                return telephone;
            }
        }

        public string GetMatchStaffcode
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchStaffcode(GET)");
                return staffcode;
            }
        }

        public string GetMatchGender
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchGender(GET)");
                return gender;
            }
        }

        public string GetMatchTitle
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchTitle(GET)");
                return title;
            }
        }

        public string GetMatchUDF
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchUDF(GET)");
                return udf;
            }
        }      

        public string GetMatchYear
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchYear(GET)");
                return year;
            }
        }

        public string GetMatchReg
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchReg(GET)");
                return reg;
            }
        }

        public string GetMatchHouse
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchHouse(GET)");
                return house;
            }
        }

        public string GetMatchTown
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchTown(GET)");
                return town;
            }
        }

        public string GetMatchPostcode
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchPostcode(GET)");
                return postcode;
            }
        }

        /// <summary>
        /// Gets the user defined Email Main column name
        /// </summary>
        public string GetMatchEmailMain
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchEmailMain(GET)");
                return emailMain;
            }
        }

        /// <summary>
        /// Gets the user defined Email Primary column name
        /// </summary>
        public string GetMatchEmailPrimary
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchEmailPrimary(GET)");
                return emailPrimary;
            }
        }

        /// <summary>
        /// Gets the user defined Telephone Main column name
        /// </summary>
        public string GetMatchTelephoneMain
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchTelephoneMain(GET)");
                return telephoneMain;
            }
        }

        /// <summary>
        /// Gets the user defined Telephone Primary column name
        /// </summary>
        public string GetMatchTelephonePrimary
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchTelephonePrimary(GET)");
                return telephonePrimary;
            }
        }

        public Interfaces.UserType SetUserType
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetUserType(SET: " + value + ")");
                userType = value;
                Switcher.SimsApiClass.SetUserType = value;
            }
        }

        public Interfaces.UserType GetUserType
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetUserType(GET)");
                return userType;
            }
        }
        #endregion

        public string SetUserFilter
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetUserFilter(SET: " + value + ")");
                userFilter = value;
                Switcher.SimsApiClass.SetUserFilter = value;
            }
        }

        public string SetUDFType
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetUDFType(SET: " + value + ")");
                udfType = value;
            }
        }

        public string GetUDFType
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetUDFType(GET)");
                return udfType;
            }
        }

        public DataTable AddToDataTable(int recordupto)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.PreImport.AddToDataTable(recordupto: " + recordupto + ")");
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

        public string GetStatus(string personid,
            string email, string simsEmail,
            string udf, string simsUdf,
            string telephone, string simsTelephone)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.PreImport.GetStatus(personid: " + personid + ", email: " + email +
                ", simsEmail: " + simsEmail + ", udf: " + udf + ", simsUdf: " + simsUdf + ", telephone: " + telephone +
                ", simsTelephone: " + simsTelephone + ")");
            if (personid == "0")
            {
                return "Missing";
            }
            if (!IsNotDuplicate(personid))
            {
                return "Duplicate";
            }

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
            if (emailImport)
            {
                status = "Import Email";
            }
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

            if (string.IsNullOrEmpty(status))
            {
                return "Ignore";
            }
            return status;
        }

        private bool IsNotDuplicate(string personid)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.PreImport.IsNotDuplicate(personid: " + personid + ")");
            string[] pids = personid.Split(',');
            int noPids = pids.Length;
            if (noPids == 1)
            {
                return true;
            }
            return false;
        }

        private bool IsImport(string import, string current)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.PreImport.IsNotDuplicate(import: " + import + ", current: " + current +
                ")");
            if (!IsNotSame(import, current))
            {
                return false;
            }
            return true;
        }

        private bool IsNotSame(string import, string current)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.PreImport.IsNotDuplicate(import: " + import + ", current: " + current +
                ")");
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
    }
}