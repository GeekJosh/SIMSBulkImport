using System;
using System.Data;
using NLog;
using SIMS.Entities;
using SIMS.Processes;
using Exception = System.Exception;
using PersonCache = SIMS.Entities.PersonCache;
using SIMSBulkImport;

namespace SIMSBulkImport.Classes
{
    /// <summary>
    ///     Staff
    /// </summary>
    public class Staff
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();

        public int DefaultStaffPersonId = 0;

        private string simsudf;
        private DataTable staffUdfs;
        private EmployeeFilter staffUserFilter = EmployeeFilter.AllCurrent;

        #region GET
        public int GetDefaultStaffPersonId
        {
            get
            {
                if (DefaultStaffPersonId == 0)
                {
                    var employees = new BrowseEmployee();
                    employees.Load(GetStaffUserFilter, "%", "%", "%", "%", "%", false, DateTime.Now);
                    foreach (EmployeeSummary empSum in employees.Employees)
                    {
                        logger.Log(LogLevel.Debug, "Default StaffID: " + empSum.PersonID);
                        DefaultStaffPersonId = empSum.PersonID;
                        return DefaultStaffPersonId;
                    }
                }
                return DefaultStaffPersonId;
            }
        }

        private DataTable createUdfTable
        {
            get
            {
                var udfTable = new DataTable("SIMS UDFs");
                udfTable.Columns.Add("Type");
                udfTable.Columns.Add("Name");
                udfTable.Columns.Add("Min");
                udfTable.Columns.Add("Max");
                return udfTable;
            }
        }

        public DataTable GetStaffUDFs
        {
            get
            {
                if (staffUdfs == null)
                {
                    staffUdfs = createUdfTable;
                    try
                    {
                        var empprocess = new EditEmployee();
                        empprocess.Load(GetDefaultStaffPersonId, DateTime.Now);

                        foreach (UDFValue udfVal in empprocess.Employee.UDFValues)
                        {
                            if (udfVal.FieldTypeCode == UDFFieldType.STRING1_CODE ||
                                udfVal.FieldTypeCode == UDFFieldType.STRINGM_CODE ||
                                //udfVal.FieldTypeCode == UDFFieldType.DOUBLE_CODE ||
                                //udfVal.FieldTypeCode == UDFFieldType.DOUBLE2_CODE ||
                                //udfVal.FieldTypeCode == UDFFieldType.DATE_CODE ||
                                //udfVal.FieldTypeCode == UDFFieldType.INTEGER_CODE ||
                                udfVal.FieldTypeCode == UDFFieldType.LOOKUP1_CODE ||
                                udfVal.FieldTypeCode == UDFFieldType.BOOLEAN_CODE
                                )
                            {
                                string type = SIMSAPI.GetFriendlyNameFromUDFFieldType(udfVal.FieldTypeCode);
                                string name = udfVal.TypedValueAttribute.Description;
                                int min = udfVal.TypedValueAttribute.MinLength;
                                int max = udfVal.TypedValueAttribute.MaxLength;

                                logger.Log(LogLevel.Debug, "GetStaffUDFs:: " + name + " (" + type + ")");
                                DataRow udfRow = staffUdfs.NewRow();
                                udfRow["Type"] = type;
                                udfRow["Name"] = name;
                                udfRow["Min"] = min;
                                udfRow["Max"] = max;
                                staffUdfs.Rows.Add(udfRow);
                            }
                        }
                        empprocess.Dispose();
                    }
                    catch (Exception GetStaffUDFs_Exception)
                    {
                        logger.Log(LogLevel.Error, "GetStaffUDFs:: " + GetStaffUDFs_Exception);
                    }
                }
                return staffUdfs;
            }
        }

        private EmployeeFilter GetStaffUserFilter
        {
            get
            {
                return staffUserFilter;
            }
        }

        public string GetStaffEmail(int pid)
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Classes.Staff.GetStaffEmail(pid=" + pid + ")");
            string result = "";
            try
            {
                var empprocess = new EditEmployee();
                empprocess.Load(pid, DateTime.Now);
                if (empprocess.Employee.EMails.Value.Count > 0)
                {
                    int workcount = 1;
                    string emailaddresses = null;

                    foreach (EMail item in empprocess.Employee.EMails.Value)
                    {
                        //int location = item.Location.ID;
                        //if (location == emailLocationId)

                        if (workcount == 1)
                        {
                            emailaddresses = item.Address.ToLower();
                        }
                        else
                        {
                            emailaddresses = emailaddresses + "," + item.Address.ToLower();
                        }
                    }
                    return emailaddresses;
                }
                return "<BLANK>";
            }
            catch (Exception GetPersonEmailException)
            {
                logger.Log(LogLevel.Error, GetPersonEmailException);
            }
            return result;
        }

        public string GetStaffTelephone(int pid)
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Classes.Staff.GetStaffTelephone(pid=" + pid + ")");
            string result = "";
            try
            {
                var empprocess = new EditEmployee();
                empprocess.Load(pid, DateTime.Now);
                if (empprocess.Employee.Telephones.Value.Count > 0)
                {
                    int workcount = 1;
                    string telephone = null;
                    foreach (Telephone item in empprocess.Employee.Telephones.Value)
                    {
                        //int location = item.Location.ID;
                        //if (location == telephoneLocationId)
                        if (workcount == 1)
                        {
                            telephone = item.Number.ToLower();
                        }
                        else
                        {
                            telephone = telephone + "," + item.Number.ToLower();
                        }
                        workcount = workcount + 1;
                    }
                    if (string.IsNullOrEmpty(telephone))
                    {
                        return "<NO TYPE TELEPHONE>";
                    }
                    return telephone;
                }
                return "<BLANK>";
            }
            catch (Exception GetPersonEmailException)
            {
                logger.Log(LogLevel.Error, GetPersonEmailException);
            }
            return result;
        }

        public string GetStaffUDF(int pid)
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Classes.Staff.GetStaffUDF(pid=" + pid + ")");
            string result = "";

            try
            {
                var empprocess = new EditEmployee();
                empprocess.Load(pid, DateTime.Now);

                foreach (UDFValue udfVal in empprocess.Employee.UDFValues)
                {
                    if (udfVal.FieldTypeCode == UDFFieldType.STRING1_CODE)
                    {
                        string type = udfVal.TypedValueAttribute.Description;

                        if (type == simsudf)
                        {
                            return ((StringAttribute)udfVal.TypedValueAttribute).Value;
                        }
                    }
                }

                empprocess.Dispose();
            }
            catch (Exception GetStaffUdfException)
            {
                logger.Log(LogLevel.Error, GetStaffUdfException);
            }

            return result;
        }

        public string GetStaffDOB(int pid)
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Classes.Staff.GetStaffDOB(pid=" + pid + ")");
            string result = "";
            try
            {
                var empprocess = new EditEmployee();
                empprocess.Load(pid, DateTime.Now);
                DateTime dt = empprocess.Employee.DateOfBirth;
                result = dt.ToShortDateString();
            }
            catch (Exception GetPersonDobException)
            {
                logger.Log(LogLevel.Error, GetPersonDobException);
            }
            return result;
        }

        public string GetStaffCode(int pid)
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Classes.Staff.GetStaffCode(pid=" + pid + ")");
            string result = "";
            try
            {
                var empprocess = new EditEmployee();
                empprocess.Load(pid, DateTime.Now);
                result = empprocess.Employee.EmployeeCode;
            }
            catch (Exception GetPersonCodeException)
            {
                logger.Log(LogLevel.Error, GetPersonCodeException);
            }
            return result;
        }

        public string GetStaffForename(int pid)
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Classes.Staff.GetStaffForename(pid=" + pid + ")");
            string result = "";
            try
            {
                var empprocess = new EditEmployee();
                empprocess.Load(pid, DateTime.Now);
                result = empprocess.Employee.Forename;
            }
            catch (Exception GetPersonForenameException)
            {
                logger.Log(LogLevel.Error, GetPersonForenameException);
            }
            return result;
        }

        public string GetStaffTitle(int pid)
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Classes.Staff.GetStaffTitle(pid=" + pid + ")");
            string result = "";
            try
            {
                EditEmployee empprocess = new EditEmployee();
                empprocess.Load(pid, DateTime.Now);
                result = empprocess.Employee.Title.ToString();
            }
            catch (Exception GetPersonTitleException)
            {
                logger.Log(LogLevel.Error, GetPersonTitleException);
            }
            return result;
        }

        public string GetStaffGender(int pid)
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Classes.Staff.GetStaffGender(pid=" + pid + ")");
            string result = "";
            try
            {
                var empprocess = new EditEmployee();
                empprocess.Load(pid, DateTime.Now);
                result = empprocess.Employee.Gender.ToString();
            }
            catch (Exception GetPersonGenderException)
            {
                logger.Log(LogLevel.Error, GetPersonGenderException);
            }
            return result;
        }

        public string GetStaffSurname(int pid)
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Classes.Staff.GetStaffSurname(pid=" + pid + ")");
            string result = "";
            try
            {
                var empprocess = new EditEmployee();
                empprocess.Load(pid, DateTime.Now);
                result = empprocess.Employee.Surname;
            }
            catch (Exception GetPersonSurnameException)
            {
                logger.Log(LogLevel.Error, GetPersonSurnameException);
            }
            return result;
        }

        public string GetStaffPersonID(string surname, string forename, string title, string gender, string staffcode)
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Classes.Staff.GetStaffPersonID(surname=" + surname + ")");
            int count = 0;
            string value = "0";

            if (string.IsNullOrWhiteSpace(surname))
            {
                surname = "%";
            }
            if (string.IsNullOrWhiteSpace(forename))
            {
                forename = "%";
            }
            if (string.IsNullOrWhiteSpace(title))
            {
                title = "%";
            }
            if (string.IsNullOrWhiteSpace(gender))
            {
                gender = "%";
            }
            if (string.IsNullOrWhiteSpace(staffcode))
            {
                staffcode = "%";
            }


            var employees = new BrowseEmployee();
            try
            {
                employees.Load(GetStaffUserFilter, surname, forename, title, gender, staffcode, false, DateTime.Now);
            }
            catch (Exception GetStaffPersonid_Exception)
            {
                logger.Log(LogLevel.Error, GetStaffPersonid_Exception);
            }

            foreach (EmployeeSummary empSum in employees.Employees)
            {
                count = count + 1;
                value = empSum.PersonID.ToString();

                if (count == 1)
                {
                    value = empSum.PersonID.ToString();
                }
                else
                {
                    value = value + "," + empSum.PersonID;
                }
            }
            return value;
        }
        #endregion

        #region SET
        public string SetStaffUserFilter
        {
            set
            {
                string userFilter = value;
                switch (userFilter)
                {
                    case "Staff, all Current":
                        staffUserFilter = EmployeeFilter.AllCurrent;
                        break;
                    case "Teaching staff, all Current":
                        staffUserFilter = EmployeeFilter.CurrentTeachers;
                        break;
                    case "Support Staff, all Current":
                        staffUserFilter = EmployeeFilter.CurrentNonTeachers;
                        break;
                    case "Staff, all Future":
                        staffUserFilter = EmployeeFilter.AllFuture;
                        break;
                    case "Teaching staff, all Leavers":
                        staffUserFilter = EmployeeFilter.ExTeachers;
                        break;
                    case "Support Staff, all Leavers":
                        staffUserFilter = EmployeeFilter.AllLeaverSupportStaff;
                        break;
                    case "Staff, all Leavers":
                        staffUserFilter = EmployeeFilter.AllLeaverStaff;
                        break;
                    case "All":
                        staffUserFilter = EmployeeFilter.All;
                        break;
                    default:
                        staffUserFilter = EmployeeFilter.AllCurrent;
                        break;
                }
            }
        }

        public string SetStaffSIMSUDF
        {
            set { simsudf = value; }
        }

        /// <summary>
        /// SetStaffEmail
        /// </summary>
        /// <param name="personid"></param>
        /// <param name="emailValue"></param>
        /// <param name="main">1=Yes, 2=Yes (overwrite), 3=No</param>
        /// <param name="primary"></param>
        /// <returns></returns>
        public bool SetStaffEmail(Int32 personid, string emailValue, string main, string primary, string notes, string location)
        {
            try
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Classes.Staff.SetStaffEmail(personid=" + personid + ", emailValue=" + emailValue +
                    ", main=" + main + ", primary=" + primary + ", notes=" + notes + ", location=" + location);
                var empprocess = new EditEmployee();
                
                // Load employee\staff record
                empprocess.Load(personid, DateTime.Now);

                // Create a new Email record
                var email = new EMail();

                // Set the email address
                email.AddressAttribute.Value = emailValue;

                // Set Main
                switch (main)
                {
                    case "Yes":
                        email.MainAttribute.Value = (empprocess.Employee.EMails.Value.Count > 0)
                            ? EMailMainCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : EMailMainCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                    case "Yes (Overwrite)":
                        email.MainAttribute.Value = EMailMainCollection.GetValues().Item(1);
                        break;
                    case "No":
                        email.MainAttribute.Value = EMailMainCollection.GetValues().Item(0);
                        break;
                    default:
                        email.MainAttribute.Value = (empprocess.Employee.EMails.Value.Count > 0)
                            ? EMailMainCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : EMailMainCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                }

                // Set Primary
                switch (primary)
                {
                    case "Yes":
                        email.PrimaryAttribute.Value = (empprocess.Employee.EMails.Value.Count > 0)
                            ? EMailPrimaryCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : EMailPrimaryCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                    case "Yes (Overwrite)":
                        email.PrimaryAttribute.Value = EMailPrimaryCollection.GetValues().Item(1);
                        break;
                    case "No":
                        email.PrimaryAttribute.Value = EMailPrimaryCollection.GetValues().Item(0);
                        break;
                    default:
                        email.PrimaryAttribute.Value = (empprocess.Employee.EMails.Value.Count > 0)
                            ? EMailPrimaryCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : EMailPrimaryCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                }

                // Set the notes
                if (!string.IsNullOrWhiteSpace(notes))
                    email.NotesAttribute.Value = notes;

                // Set the location
                email.LocationAttribute.Value = PersonCache.EmailLocations.ItemByDescription(location);

                // Run Validation against the new record
                email.Validate();
                logger.Log(LogLevel.Debug, "Email Valid: " + email.Valid());

                // Save the new record to the database
                empprocess.Employee.EMails.Add(email);
                return empprocess.Save(DateTime.Now);
            }
            catch (Exception SetStaffEmail_Exception)
            {
                logger.Log(LogLevel.Error, "SetStaffEmail_Exception " + SetStaffEmail_Exception);
                return false;
            }
        }

        /// <summary>
        /// SetStaffTelephone
        /// </summary>
        /// <param name="personid"></param>
        /// <param name="telephone"></param>
        /// <param name="main"></param>
        /// <param name="primary"></param>
        /// <param name="notes"></param>
        /// <param name="location"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public bool SetStaffTelephone(Int32 personid, string telephone, string main, string primary, string notes, string location, string device)
        {
            try
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Classes.Staff.SetStaffTelephone(personid=" + personid + ", telephone=" + telephone +
                    ", main=" + main + ", primary=" + primary + ", notes=" + notes + ", location=" + location + ", device=" + device);
                var empprocess = new EditEmployee();

                // Load employee\staff record 
                empprocess.Load(personid, DateTime.Now);

                // Create a new telephone number
                var phone = new Telephone();

                // Set the telephone number
                phone.NumberAttribute.Value = telephone;

                // Set Main
                switch (main)
                {
                    case "Yes":
                        logger.Log(LogLevel.Debug, "SetStaffTelephone Main: Yes");
                        phone.MainAttribute.Value = (empprocess.Employee.Telephones.Value.Count > 0)
                            ? TelephoneMainCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : TelephoneMainCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                    case "Yes (Overwrite)":
                        logger.Log(LogLevel.Debug, "SetStaffTelephone Main: Yes (Overwrite)");
                        phone.MainAttribute.Value = TelephoneMainCollection.GetValues().Item(1);
                        break;
                    case "No":
                        logger.Log(LogLevel.Debug, "SetStaffTelephone Main: No");
                        phone.MainAttribute.Value = TelephoneMainCollection.GetValues().Item(0);
                        break;
                    default:
                        logger.Log(LogLevel.Debug, "SetStaffTelephone Main: default");
                        phone.MainAttribute.Value = (empprocess.Employee.Telephones.Value.Count > 0)
                            ? TelephoneMainCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : TelephoneMainCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                }

                // Set Primary
                switch (primary)
                {
                    case "Yes":
                        logger.Log(LogLevel.Debug, "SetStaffTelephone primary: Yes");
                        phone.PrimaryAttribute.Value = (empprocess.Employee.Telephones.Value.Count > 0)
                            ? TelephoneMainCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : TelephoneMainCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                    case "Yes (Overwrite)":
                        logger.Log(LogLevel.Debug, "SetStaffTelephone primary: Yes (Overwrite)");
                        phone.PrimaryAttribute.Value = TelephoneMainCollection.GetValues().Item(1);
                        break;
                    case "No":
                        logger.Log(LogLevel.Debug, "SetStaffTelephone primary: No");
                        phone.PrimaryAttribute.Value = TelephoneMainCollection.GetValues().Item(0);
                        break;
                    default:
                        logger.Log(LogLevel.Debug, "SetStaffTelephone primary: default");
                        phone.PrimaryAttribute.Value = (empprocess.Employee.Telephones.Value.Count > 0)
                            ? TelephoneMainCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : TelephoneMainCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                }

                // Set the notes
                if (!string.IsNullOrWhiteSpace(notes))
                {
                    logger.Log(LogLevel.Debug, "SetStaffTelephone notes: " + notes);
                    phone.NotesAttribute.Value = notes;
                }

                // Set the location
                phone.LocationAttribute.Value = PersonCache.TelephoneLocations.ItemByDescription(location);

                // Set the device (telephone\fax)
                phone.DeviceAttribute.Value = PersonCache.TelephoneDevices.ItemByDescription(device);

                // Run Validation against the new record
                phone.Validate();
                logger.Log(LogLevel.Debug, "Telephone Valid: " + phone.Valid());

                // Writes the new record to the database
                empprocess.Employee.Telephones.Add(phone);
                bool result = empprocess.Save(DateTime.Now);
                logger.Log(LogLevel.Debug, "SetStaffTelephone result: " + result);
                return true;
            }
            catch (Exception SetStaffTelephone_Exception)
            {
                logger.Log(LogLevel.Error, "SetStaffTelephone " + SetStaffTelephone_Exception);
                return false;
            }
        }

        public bool SetStaffUDF(Int32 personid, string udf)
        {
            try
            {
                using (EditEmployee empprocess = new EditEmployee())
                {
                    empprocess.Load(personid, DateTime.Now);
                    foreach (UDFValue udfVal in empprocess.Employee.UDFValues)
                    {
                        if (simsudf == udfVal.TypedValueAttribute.Description)
                        {
                            if (Core.SetUdf(udfVal, udf))
                            {
                                empprocess.Save(DateTime.Now);
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    logger.Error("UDF {0} not found.", simsudf);
                    return false;
                }
            }
            catch (Exception SetStaffUDF_Exception)
            {
                logger.Log(LogLevel.Error, "SetStaffUDF (string)" + SetStaffUDF_Exception);
                return false;
            }
        }

        public bool SetStaffUDF(Int32 personid, double udf)
        {
            try
            {
                var empprocess = new EditEmployee();
                empprocess.Load(personid, DateTime.Now);
                foreach (UDFValue udfVal in empprocess.Employee.UDFValues)
                {
                    if (simsudf == udfVal.TypedValueAttribute.Description)
                    {
                        ((DoubleAttribute)udfVal.TypedValueAttribute).Value = udf;
                    }
                    logger.Log(LogLevel.Debug, "SetStaffUDF (double) - " + udfVal.Valid());
                }
                empprocess.Save(DateTime.Now);
                empprocess.Dispose();
            }
            catch (Exception SetStaffUDF_Exception)
            {
                logger.Log(LogLevel.Error, "SetStaffUDF (double)" + SetStaffUDF_Exception);
                return false;
            }
            return true;
        }

        public bool SetStaffUDF(Int32 personid, DateTime udf)
        {
            try
            {
                var empprocess = new EditEmployee();
                empprocess.Load(personid, DateTime.Now);
                foreach (UDFValue udfVal in empprocess.Employee.UDFValues)
                {
                    if (simsudf == udfVal.TypedValueAttribute.Description)
                    {
                        ((DateAttribute)udfVal.TypedValueAttribute).Value = udf;
                    }
                    logger.Log(LogLevel.Debug, "SetStaffUDF (double) - " + udfVal.Valid());
                }
                empprocess.Save(DateTime.Now);
                empprocess.Dispose();
            }
            catch (Exception SetStaffUDF_Exception)
            {
                logger.Log(LogLevel.Error, "SetStaffUDF (double)" + SetStaffUDF_Exception);
                return false;
            }
            return true;
        }

        public bool SetStaffUDF(Int32 personid, Int32 udf)
        {
            try
            {
                var empprocess = new EditEmployee();
                empprocess.Load(personid, DateTime.Now);
                foreach (UDFValue udfVal in empprocess.Employee.UDFValues)
                {
                    if (simsudf == udfVal.TypedValueAttribute.Description)
                    {
                        ((IntegerAttribute)udfVal.TypedValueAttribute).Value = udf;
                    }
                    logger.Log(LogLevel.Debug, "SetStaffUDF (integer) - " + udfVal.Valid());
                }
                empprocess.Save(DateTime.Now);
                empprocess.Dispose();
            }
            catch (Exception SetStaffUDF_Exception)
            {
                logger.Log(LogLevel.Error, "SetStaffUDF (integer)" + SetStaffUDF_Exception);
                return false;
            }
            return true;
        }
        #endregion
    }
}
