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
        private string simsUdf;

        private string emailMain;
        private string emailPrimary;
        private string emailLocation;
        private string emailNotes;

        private string telephoneMain;
        private string telephonePrimary;
        private string telephoneLocation;
        private string telephoneNotes;
        private string telephoneDevice;

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

        /// <summary>
        /// Gets if the first row should be ignored (ie it's the column names)
        /// </summary>
        public bool GetMatchIgnoreFirstRow
        {
            get
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.GETMatchIgnoreFirstRow(GET)");
                return ignoreFirstRow;
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
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchSIMSUDF(SET: " + value + ")");
                simsUdf = value;
                Switcher.SimsApiClass.SetSIMSUDF = simsUdf;
            }
        }

        /// <summary>
        /// Sets the user-defined column to be used for Email Location
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
        /// Sets the user-defined column to be used for Email Main
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
        /// Sets the user-defined column to be used for Email Primary
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
        /// Sets the user-defined column to be used for Email Notes
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
        /// Sets the user-defined column to be used for Telephone Location
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
        /// Sets the user-defined column to be used for Telephone Main
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
        /// Sets the user-defined column to be used for Telephone Primary
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
        /// Sets the user-defined column to be used for Telephone Notes
        /// </summary>
        public string SetMatchTelephoneNotes
        {
            set
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchTelephoneNotes(SET: " + value + ")");
                telephoneNotes = value;
            }
        }

        /// <summary>
        /// Sets the user-defined column to be used for Telephone Devices
        /// </summary>
        public string SetMatchTelephoneDevice
        {
            set
            {
                logger.Log(LogLevel.Trace,
                    "Trace:: Matt40k.SIMSBulkImport.PreImport.SetMatchTelephoneDevice(SET: " + value + ")");
                telephoneDevice = value;
            }
        }
        #endregion

        #region GetMatch
        /// <summary>
        /// Get the user-defined column to be used for PersonID
        /// </summary>
        public string GetMatchPersonID
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchPersonID(GET)");
                return personId;
            }
        }

        /// <summary>
        /// Get the user-defined column to be used for Forename
        /// </summary>
        public string GetMatchFirstname
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchFirstname(GET)");
                return firstname;
            }
        }

        /// <summary>
        /// Get the user-defined column to be used for Surname
        /// </summary>
        public string GetMatchSurname
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchSurname(GET)");
                return surname;
            }
        }

        /// <summary>
        /// Get the user-defined column to be used for Email address
        /// </summary>
        public string GetMatchEmail
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchEmail(GET)");
                return email;
            }
        }

        /// <summary>
        /// Get the user-defined column to be used for Telephone number
        /// </summary>
        public string GetMatchTelephone
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchTelephone(GET)");
                return telephone;
            }
        }

        /// <summary>
        /// Get the user-defined column to be used for Staff code
        /// </summary>
        public string GetMatchStaffcode
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchStaffcode(GET)");
                return staffcode;
            }
        }

        /// <summary>
        /// Get the user-defined column to be used for Gender
        /// </summary>
        public string GetMatchGender
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchGender(GET)");
                return gender;
            }
        }

        /// <summary>
        /// Get the user-defined column to be used for  
        /// </summary>
        public string GetMatchTitle
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchTitle(GET)");
                return title;
            }
        }

        /// <summary>
        /// Get the user-defined column to be used for UDF
        /// </summary>
        public string GetMatchUDF
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchUDF(GET)");
                return udf;
            }
        }      

        /// <summary>
        /// Get the user-defined column to be used for Year
        /// </summary>
        public string GetMatchYear
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchYear(GET)");
                return year;
            }
        }

        /// <summary>
        /// Get the user-defined column to be used for Registration
        /// </summary>
        public string GetMatchReg
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchReg(GET)");
                return reg;
            }
        }

        /// <summary>
        /// Get the user-defined column to be used for House
        /// </summary>
        public string GetMatchHouse
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchHouse(GET)");
                return house;
            }
        }

        /// <summary>
        /// Get the user-defined column to be used for Town
        /// </summary>
        public string GetMatchTown
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchTown(GET)");
                return town;
            }
        }

        /// <summary>
        /// Get the user-defined column to be used for Postcode
        /// </summary>
        public string GetMatchPostcode
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchPostcode(GET)");
                return postcode;
            }
        }

        /// <summary>
        /// Get the user-defined column to be used for Email Main
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
        /// Get the user-defined column to be used for Email Primary
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
        /// Get the user-defined column to be used for Email Location
        /// </summary>
        public string GetMatchEmailLocation
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchEmailLocation(GET)");
                return emailLocation;
            }
        }

        /// <summary>
        /// Get the user-defined column to be used for Email Notes
        /// </summary>
        public string GetMatchEmailNotes
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchEmailNotes(GET)");
                return emailNotes;
            }
        }

        /// <summary>
        ///Get the user-defined column to be used for Telephone Main
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
        /// Get the user-defined column to be used for Telephone Primary
        /// </summary>
        public string GetMatchTelephonePrimary
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchTelephonePrimary(GET)");
                return telephonePrimary;
            }
        }

        /// <summary>
        /// Get the user-defined column to be used for Telephone Notes
        /// </summary>
        public string GetMatchTelephoneNotes
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchTelephoneNotes(GET)");
                return telephoneNotes;
            }
        }

        /// <summary>
        /// Get the user-defined column to be used for Telephone Device
        /// </summary>
        public string GetMatchTelephoneDevice
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchTelephoneDevice(GET)");
                return telephoneDevice;
            }
        }

        /// <summary>
        /// Get the user-defined column to be used for Telephone Location
        /// </summary>
        public string GetMatchTelephoneLocation
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchTelephoneLocation(GET)");
                return telephoneLocation;
            }
        }

        /// <summary>
        /// Get the user-defined column to be used for SIMS UDF
        /// </summary>
        public string GetMatchSIMSUDF
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetMatchSIMSUDF(GET)");
                return simsUdf;
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public Interfaces.UserType SetUserType
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetUserType(SET: " + value + ")");
                userType = value;
                Switcher.SimsApiClass.SetUserType = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Interfaces.UserType GetUserType
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetUserType(GET)");
                return userType;
            }
        }
        
        public string SetUserFilter
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.SetUserFilter(SET: " + value + ")");
                userFilter = value;
                Switcher.SimsApiClass.SetUserFilter = value;
            }
        }

        public string GetUserFilter
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.PreImport.GetUserFilter(GET)");
                return userFilter;
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