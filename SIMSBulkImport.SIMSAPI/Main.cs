using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using SIMSBulkImport.Classes;
using NLog;
using SIMS.Entities;
using SIMS.Processes;
using Contacts = SIMSBulkImport.Classes.Contacts;
using Exception = System.Exception;
using PersonCache = SIMS.Entities.PersonCache;

namespace SIMSBulkImport
{
    public class SIMSAPI
    {
        public enum UdfTypes
        {
            STRING1,
            STRINGM,
            DATE,
            INTEGER
        };

        public static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string simsDir;

        private Contacts _contacts;
        private Pupils _pupils;
        private Staff _staff;
        private string errorMessage;
        private Interfaces.UserType importType;
        private string laCode;
        private bool loginTrusted;

        private Login loginprocess;
        private string pass;
        private string schCode;
        private string schName;
        private Image schoolLogo;
        private bool setupRequired;
        private string simsVersion;
        private string[] emailLocations;
        private string[] telephoneLocations;
        private string[] telephoneDevices;
        private string user;
        private string userFilter;
        private string userName;

        /// <summary>
        /// </summary>
        /// <param name="simsDir">The folder where SIMS .net is installed</param>
        public SIMSAPI(string _simsdir)
        {
            try
            {
                simsDir = _simsdir;
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.AssemblyResolve += currentDomain_AssemblyResolve;
            }
            catch (Exception SIMSAPI_Exception)
            {
                errorMessage = SIMSAPI_Exception.ToString();
            }
        }

        /// <summary>
        /// Set the SIMS username to be used (assuming SIMS SQL authentication is used)
        /// </summary>
        public string SetSimsUser
        {
            set { user = value; }
        }

        /// <summary>
        /// Set the SIMS password to be used (assuming SIMS SQL authentication is used)
        /// </summary>
        public string SetSimsPass
        {
            set { pass = value; }
        }

        /// <summary>
        /// Create a connection to the SIMS .net database
        /// </summary>
        public bool Connect
        {
            get
            {
                bool result = true;

                try
                {
                    logger.Log(LogLevel.Info, "Connect.ini directory: " + simsDir);
                    Cache.ConnectINIDirectoryLocation = simsDir;

                    // Set the security mode
                    //   Trusted - Windows Authentication
                    //   SQLServer - SIMS SQL Authentication
                    if (loginTrusted)
                        Cache.SecurityMode = SecurityModeEnum.Trusted;
                    else
                        Cache.SecurityMode = SecurityModeEnum.SQLServer;

                    // Load Login
                    loginprocess = new Login();

                    // If using Windows, set the username as the Windows Username
                    if (Cache.SecurityMode != SecurityModeEnum.SQLServer)
                    {
                        user = Environment.UserDomainName + "\\" + Environment.UserName;
                        pass = user;
                    }

                    int signature = loginprocess.GetDatabaseSignature(user, pass, "ATW", true);
                    loginprocess.Init(signature, user, pass);
                    var dm = new DatabaseMode(false);
                    Cache.CurrentDatabase.DatabaseMode = dm;

                    // Set the  SIMS log file path (not used?)
                    Cache.LogFile = (Environment.SpecialFolder.CommonApplicationData) +
                                    "\\SIMSBulkImport\\SIMS_Log.txt";

                    // Confirm we are a third party - because we're nice ;)
                    Cache.ThirdPartyLogin = true;

                    // Pull out some basic info.
                    laCode = Cache.CurrentSchool.LEANumber;
                    schCode = Cache.CurrentSchool.SchoolNumber;
                    schName = Cache.CurrentSchool.SchoolName;
                    userName = Cache.CurrentUser.ShortName;
                    setupRequired = Cache.SetupRequired;
                    simsVersion = Cache.CurrentDatabase.DatabaseServerVersion();
                    schoolLogo = Cache.CurrentSchool.SchoolLogo.Value as Image;

                    logger.Log(LogLevel.Debug, "Setup required: " + setupRequired);
                    logger.Log(LogLevel.Debug, "Cache.CurrentSchool.LEANumber: " + Cache.CurrentSchool.LEANumber);
                    logger.Log(LogLevel.Debug, "Cache.CurrentSchool.SchoolNumber: " + Cache.CurrentSchool.SchoolNumber);
                }
                catch (Exception ConnectException)
                {
                    logger.Log(LogLevel.Error, ConnectException);
                    errorMessage = ConnectException.ToString();
                    return false;
                }
                return result;
            }
        }

        /// <summary>
        /// Did an error occur when connecting to the SIMS .net database
        /// </summary>
        public string GetConnectError
        {
            get { return errorMessage; }
        }

        /// <summary>
        /// Returns the school name
        /// </summary>
        public string GetCurrentSchool
        {
            get { return schName; }
        }

        /// <summary>
        /// Returns the LA code
        /// </summary>
        public int? GetCurrentLA
        {
            get
            {
                if (string.IsNullOrEmpty(laCode))
                    return null;
                else
                    return Int16.Parse(laCode);
            }
        }

        /// <summary>
        /// Returns the DfE code
        /// </summary>
        public int? GetCurrentDfE
        {
            get
            {
                if (string.IsNullOrEmpty(schCode))
                    return null;
                else
                    return Int16.Parse(schCode);
            }
        }



        /// <summary>
        /// Returns the SIMS current user
        /// </summary>
        public string GetCurrentUser
        {
            get
            {
                // Not sure if this would be correct output.
                return userName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Interfaces.UserType SetUserType
        {
            set
            {
                importType = value;
                switch (value)
                {
                    case Interfaces.UserType.Contact:
                        _contacts = new Contacts();
                        break;
                    case Interfaces.UserType.Pupil:
                        _pupils = new Pupils();
                        break;
                    case Interfaces.UserType.Staff:
                        _staff = new Staff();
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SetUserFilter
        {
            set
            {
                userFilter = value;
                switch (importType)
                {
                    case Interfaces.UserType.Contact:
                        _contacts.SetContactUserFilter = value;
                        break;
                    case Interfaces.UserType.Pupil:
                        _pupils.SetPupilUserFilter = value;
                        break;
                    case Interfaces.UserType.Staff:
                        _staff.SetStaffUserFilter = value;
                        break;
                }
            }
        }

        /// <summary>
        ///     Gets the Email Locations from the SIMS database - normally Home, Work, Other
        /// </summary>
        public string[] GetEmailLocations
        {
            get
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.SIMSAPI(Main).GetEmailLocations(GET)");

                if (emailLocations == null)
                {
                    logger.Log(LogLevel.Debug, "Loading Email Locations");
                    string tmp = "";
                    try
                    {
                        // Get the Email Location count
                        int count = PersonCache.EmailLocations.Count;
                        logger.Log(LogLevel.Debug, "Email Location count: " + count);

                        // Load Email Locations
                        EmailLocations locations = PersonCache.EmailLocations;

                        foreach (EmailLocation location in locations)
                        {
                            if (string.IsNullOrEmpty(tmp))
                            {
                                tmp = location.Description;
                            }
                            else
                            {
                                tmp = tmp + "," + location.Description;
                            }
                        }
                        emailLocations = tmp.Split(',');
                    }
                    catch (Exception GetEmailLocationsException)
                    {
                        logger.Log(LogLevel.Error, GetEmailLocationsException);
                    }
                }
                return emailLocations;
            }
        }

        /// <summary>
        ///     Gets the Telephone Locations from the SIMS database - normally Home, Work, Mobile, Other, Alternate Home, Minicom (hearing impaired/disabled)
        /// </summary>
        public string[] GetTelephoneLocations
        {
            get
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.SIMSAPI(Main).GetTelephoneLocations(GET)");

                if (telephoneLocations == null)
                {
                    logger.Log(LogLevel.Debug, "Loading Telephone Locations");
                    string tmp = "";
                    try
                    {
                        // Get the Telephone Location count 
                        int count = PersonCache.TelephoneLocations.Count;
                        logger.Log(LogLevel.Debug, "Telephone Location count: " + count);

                        // Load Telephone Locations
                        TelephoneLocations locations = PersonCache.TelephoneLocations;

                        foreach (TelephoneLocation location in locations)
                        {
                            if (string.IsNullOrEmpty(tmp))
                            {
                                tmp = location.Description;
                            }
                            else
                            {
                                tmp = tmp + "," + location.Description;
                            }
                        }
                        telephoneLocations = tmp.Split(',');
                    }
                    catch (Exception GetTelephoneLocations_Exception)
                    {
                        logger.Log(LogLevel.Error, GetTelephoneLocations_Exception);
                    }
                }
                return telephoneLocations;
            }
        }

        /// <summary>
        ///     Gets the Telephone Devices from the SIMS database - normally 
        /// </summary>
        public string[] GetTelephoneDevices
        {
            get
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.SIMSAPI(Main).GetTelephoneDevices(GET)");

                if (telephoneDevices == null)
                {
                    logger.Log(LogLevel.Debug, "Loading Telephone Devices");
                    string tmp = "";
                    try
                    {
                        // Get the Telephone Devices count 
                        int count = PersonCache.TelephoneDevices.Count;
                        logger.Log(LogLevel.Debug, "Telephone Devices count: " + count);

                        // Load Telephone Devices
                        TelephoneDevices devices = PersonCache.TelephoneDevices;

                        foreach (TelephoneDevice device in devices)
                        {
                            if (string.IsNullOrEmpty(tmp))
                            {
                                tmp = device.Description;
                            }
                            else
                            {
                                tmp = tmp + "," + device.Description;
                            }
                        }
                        telephoneDevices = tmp.Split(',');
                    }
                    catch (Exception GetTelephoneDevices_Exception)
                    {
                        logger.Log(LogLevel.Error, GetTelephoneDevices_Exception);
                    }
                }
                return telephoneDevices;
            }
        }

        /// <summary>
        /// Returns the first Email Location
        /// </summary>
        public string GetDefaultEmailLocation
        {
            get
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.SIMSAPI(Main).GetDefaultEmailLocation(GET)");
                // Load Email locations
                EmailLocations locations = PersonCache.EmailLocations;
                foreach (EmailLocation location in locations)
                {
                    return location.Description;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the first Telephone Location
        /// </summary>
        public string GetDefaultTelephoneLocation
        {
            get
            {
                logger.Log(LogLevel.Debug,
                    "Trace:: SIMSBulkImport.SIMSAPI(Main).GetDefaultTelephoneLocation(GET)");
                // Load Telephone Location
                TelephoneLocations locations = PersonCache.TelephoneLocations;
                foreach (TelephoneLocation location in locations)
                {
                    return location.Description;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the first Telephone Device
        /// </summary>
        public string GetDefaultTelephoneDevice
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: SIMSBulkImport.SIMSAPI(Main).GetDefaultTelephoneDevice(GET)");
                // Load Telephone Device
                TelephoneDevices devices = PersonCache.TelephoneDevices;
                foreach (TelephoneDevice device in devices)
                {
                    return device.Description;
                }
                return null;
            }
        }

        /// <summary>
        /// Is Windows Trusted authentication used
        /// False = SIMS SQL authentication
        /// </summary>
        public bool SetIsTrusted
        {
            set
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.SIMSAPI(MAIN).SetIsTrusted(" + value + ")");
                loginTrusted = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SetSIMSUDF
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: SIMSBulkImport.SIMSAPI(MAIN).SetSIMSUDF(" + value + ")");
                switch (importType)
                {
                    case Interfaces.UserType.Staff:
                        SetStaffSIMSUDF = value;
                        break;
                    case Interfaces.UserType.Pupil:
                        SetPupilSIMSUDF = value;
                        break;
                    case Interfaces.UserType.Contact:
                        SetContactSIMSUDF = value;
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BitmapImage GetCurrentSchoolLogo
        {
            get
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.SIMSAPI(MAIN).GetCurrentSchoolLogo(GET)");
                try
                {
                    // ImageSource ...
                    var bi = new BitmapImage();
                    bi.BeginInit();
                    var ms = new MemoryStream();
                    schoolLogo.Save(ms, ImageFormat.Bmp);
                    ms.Seek(0, SeekOrigin.Begin);
                    bi.StreamSource = ms;
                    bi.EndInit();
                    return bi;
                }
                catch (Exception GetSchoolLogo_Exception)
                {
                    logger.Log(LogLevel.Error, GetSchoolLogo_Exception);
                }
                return null;
            }
        }

        #region Contact

        public DataTable GetContactUDFs
        {
            get { return _contacts.GetContactUDFs; }
        }

        public string SetContactSIMSUDF
        {
            set { _contacts.SetContactSIMSUDF = value; }
        }

        public string GetContactPersonID(string surname, string forename, string town, string postCode)
        {
            return _contacts.GetContactPersonID(surname, forename, town, postCode);
        }

        public string GetContactEmail(Int32 personid)
        {
            return _contacts.GetContactEmail(personid);
        }

        public string GetContactTelephone(Int32 personid)
        {
            return _contacts.GetContactTelephone(personid);
        }

        public bool SetContactEmail(Int32 personid, string email, string main, string primary, string notes,
            string location)
        {
            return _contacts.SetContactEmail(personid, email, main, primary, notes, location);
        }

        public bool SetContactTelephone(Int32 personid, string telephone, string main, string primary, string notes,
            string location, string device)
        {
            return _contacts.SetContactTelephone(personid, telephone, main, primary, notes, location, device);
        }

        public bool SetContactUDF(Int32 personid, string udf)
        {
            return _contacts.SetContactUDF(personid, udf);
        }

        #endregion

        #region Pupil

        public string[] GetPupilYearGroups
        {
            get { return _pupils.GetPupilYearGroups; }
        }

        public string[] GetPupilHouses
        {
            get { return _pupils.GetPupilHouses; }
        }

        public string[] GetPupilRegistrationGroups
        {
            get { return _pupils.GetPupilRegistrationGroups; }
        }

        public DataTable GetPupilUDFs
        {
            get { return _pupils.GetPupilUDFs; }
        }

        public string SetPupilSIMSUDF
        {
            set { _pupils.SetPupilSIMSUDF = value; }
        }

        public string GetPupilPersonID(string forename, string surname, string reg, string year, string house,
            string admisno)
        {
            return _pupils.GetPupilPersonID(forename, surname, reg, year, house, admisno);
        }

        public string GetPupilForename(Int32 personid)
        {
            return _pupils.GetPupilForename(personid);
        }

        public string GetPupilSurname(Int32 personid)
        {
            return _pupils.GetPupilSurname(personid);
        }

        public string GetPupilDOB(Int32 personid)
        {
            return _pupils.GetPupilDOB(personid);
        }

        public string GetPupilAdmissionNumber(Int32 personid)
        {
            return _pupils.GetPupilAdmissionNumber(personid);
        }

        public string GetPupilGender(Int32 personid)
        {
            return _pupils.GetPupilGender(personid);
        }

        public string GetPupilYear(Int32 personid)
        {
            return _pupils.GetPupilYear(personid);
        }

        public string GetPupilRegistration(Int32 personid)
        {
            return _pupils.GetPupilRegistration(personid);
        }

        public string GetPupilHouse(Int32 personid)
        {
            return _pupils.GetPupilHouse(personid);
        }

        public string GetPupilEmail(Int32 personid)
        {
            return _pupils.GetPupilEmail(personid);
        }

        public string GetPupilUDF(Int32 personid)
        {
            return _pupils.GetPupilUDF(personid);
        }

        public string GetPupilTelephone(Int32 personid)
        {
            return _pupils.GetPupilTelephone(personid);
        }

        public List<string> GetPupilUsernameUDFs
        {
            get { return _pupils.GetPupilUsernameUDFs; }
        }

        public bool SetPupilEmail(Int32 personid, string email, string main, string primary, string notes,
            string location)
        {
            return _pupils.SetPupilEmail(personid, email, main, primary, notes, location);
        }

        public bool SetPupilTelephone(Int32 personid, string telephone, string main, string primary, string notes,
            string location, string device)
        {
            return _pupils.SetPupilTelephone(personid, telephone, main, primary, notes, location, device);
        }

        public bool SetPupilUDF(Int32 personid, string udf)
        {
            logger.Log(LogLevel.Debug, "SIMSAPI-SetPupilUDF1-string - personid: " + personid + " - udfvalue: " + udf);
            return _pupils.SetPupilUDF(personid, udf);
        }

        public bool SetPupilUDF(Int32 personid, bool udf)
        {
            logger.Log(LogLevel.Debug, "SIMSAPI-SetPupilUDF1-bool - personid: " + personid + " - udfvalue: " + udf);
            return _pupils.SetPupilUDF(personid, udf.ToString());
        }

        public bool SetPupilUsernameUDF(string udfName)
        {
            logger.Log(LogLevel.Trace, "SIMSAPISetPupilUser - udfName: " + udfName);
            return _pupils.SetPupilUsernameUDF(udfName);
        }

        public DataTable GetPupilDefaultUsernameData
        {
            get { return _pupils.GetPupilDefaultUsernameData; }
        }

        /*
        public void GetPupilHierarchy()
        {
            var tmp = _pupils.GetPupilHierarchyData;
            tmp = null;
        }

        public DataTable GetPupilHierarchyData
        {
            get
            {
                return _pupils.GetPupilHierarchyData;
            }
        }

        public int GetPupilHierarchyAllCount
        {
            get { return _pupils.GetPupilHierarchyAllCount; }
        }

        public int GetPupilHierarchyItemCount(string type, string item)
        {
            return _pupils.GetPupilHierarchyItemCount(type, item);
        }

        public int GetPupilHierarchyAllCompletedCount
        {
            get { return _pupils.GetPupilHierarchyAllCompletedCount; }
        }

        public int GetPupilHierarchyAllNotCompletedCount
        {
            get { return _pupils.GetPupilHierarchyAllNotCompletedCount; }
        }

        public int GetPupilHierarchyItemCompletedCount(string type, string item)
        {
            return _pupils.GetPupilHierarchyItemCompletedCount(type, item);
        }

        public int GetPupilHierarchyItemNotCompletedCount(string type, string item)
        {
            return _pupils.GetPupilHierarchyItemNotCompletedCount(type, item);
        }
        */

        public int GetPupilUsernameCount
        {
            get
            {
                return _pupils.GetUsernameCount;
            }
        }

        public DataTable GetPupilUsernames
        {
            get
            {
                return _pupils.GetPupilUsernames;
            }
        }

        public string GetPupilUsername(int personid)
        {
            return _pupils.GetPupilUsername(personid, "Username");
        }
        #endregion

        #region Staff

        public DataTable GetStaffUDFs
        {
            get { return _staff.GetStaffUDFs; }
        }

        public string SetStaffSIMSUDF
        {
            set { _staff.SetStaffSIMSUDF = value; }
        }

        public string GetStaffPersonID(string surname, string forename, string title, string gender, string staffcode)
        {
            return _staff.GetStaffPersonID(surname, forename, title, gender, staffcode);
        }

        public string GetStaffSurname(Int32 personid)
        {
            return _staff.GetStaffSurname(personid);
        }

        public string GetStaffForename(Int32 personid)
        {
            return _staff.GetStaffForename(personid);
        }

        public string GetStaffTitle(Int32 personid)
        {
            return _staff.GetStaffTitle(personid);
        }

        public string GetStaffGender(Int32 personid)
        {
            return _staff.GetStaffGender(personid);
        }

        public string GetStaffCode(Int32 personid)
        {
            return _staff.GetStaffCode(personid);
        }

        public string GetStaffDOB(Int32 personid)
        {
            return _staff.GetStaffDOB(personid);
        }

        public string GetStaffEmail(Int32 personid)
        {
            return _staff.GetStaffEmail(personid);
        }

        public string GetStaffUDF(Int32 personid)
        {
            return _staff.GetStaffUDF(personid);
        }

        public string GetStaffTelephone(Int32 personid)
        {
            return _staff.GetStaffTelephone(personid);
        }

        public bool SetStaffEmail(Int32 personid, string email, string main, string primary, string notes, string location)
        {
            return _staff.SetStaffEmail(personid, email, main, primary, notes, location);
        }

        public bool SetStaffTelephone(Int32 personid, string telephone, string main, string primary, string notes, string location, string device)
        {
            return _staff.SetStaffTelephone(personid, telephone, main, primary, notes, location, device);
        }

        public bool SetStaffUDF(Int32 personid, string udf)
        {
            return _staff.SetStaffUDF(personid, udf);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //This handler is called only when the common language runtime tries to bind to the assembly and fails. 
            //Retrieve the list of referenced assemblies in an array of AssemblyName. 
            Assembly MyAssembly, objExecutingAssemblies;
            string strTempAssmbPath = null;
            try
            {
                objExecutingAssemblies = Assembly.GetExecutingAssembly();
                AssemblyName[] arrReferencedAssmbNames = objExecutingAssemblies.GetReferencedAssemblies();

                //Loop through the array of referenced assembly names. 
                foreach (AssemblyName strAssmbName in arrReferencedAssmbNames)
                {
                    //Check for the assembly names that have raised the "AssemblyResolve" event. 
                    if (strAssmbName.FullName.Substring(0, strAssmbName.FullName.IndexOf(",")) ==
                        args.Name.Substring(0, args.Name.IndexOf(",")))
                    {
                        //Build the path of the assembly from where it has to be loaded.                 
                        strTempAssmbPath = simsDir;
                        if (!strTempAssmbPath.EndsWith("\\")) strTempAssmbPath += "\\";
                        strTempAssmbPath += args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll";
                        break;
                    }
                }
            }
            catch (Exception currentDomain_AssemblyResolve_Exception)
            {
                logger.Log(LogLevel.Error, currentDomain_AssemblyResolve_Exception);
            }

            //Load the assembly from the specified path.                     
            MyAssembly = Assembly.LoadFrom(strTempAssmbPath);

            //Return the loaded assembly. 
            return MyAssembly;
        }

        /// <summary>
        /// Load the UDFs
        /// </summary>
        public void LoadUdfs()
        {
            switch (importType)
            {
                case Interfaces.UserType.Staff:
                    DataTable _staffUdfs = GetStaffUDFs;
                    break;
                case Interfaces.UserType.Pupil:
                    DataTable _pupilUdfs = GetPupilUDFs;
                    break;
                case Interfaces.UserType.Contact:
                    DataTable _contactUdfs = GetContactUDFs;
                    break;
            }
        }

        /// <summary>
        /// Trusn the UDF code into the friendly name
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetFriendlyNameFromUDFFieldType(string value)
        {
            switch (value)
            {
                case ("BOOLEAN"):
                    return "True/False";
                case ("STRING1"):
                    return "Text (single-line)";
                case ("STRINGM"):
                    return "Text (multi-line)";
                case ("DOUBLE"):
                    return "Decimal";
                case ("DOUBLE2"):
                    return "Currency";
                case ("DATE"):
                    return "Date";
                case ("INTEGER"):
                    return "Number";
                case ("LOOKUP1"):
                    return "Lookup";
                default:
                    return value;
            }
        }
    }
}