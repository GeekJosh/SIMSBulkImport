using System;
using System.Collections.Generic;
using System.Data;
using NLog;
using SIMS.Entities;
using SIMS.Processes;
using Exception = System.Exception;
using PersonCache = SIMS.Entities.PersonCache;

namespace Matt40k.SIMSBulkImport.Classes
{
    /// <summary>
    ///     Students\Pupils
    /// </summary>
    public class Pupils
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private int DefaultStudentPersonId;
        private string[] pupilHouses;
        private string[] pupilRegistrationGroups;
        private DataTable pupilUdfs;
        private StudentFilter pupilUserFilter = StudentFilter.Current;
        private string[] pupilYearGroups;
        private string simsudf;

        #region GET
        public int GetDefaultStudentPersonId
        {
            get
            {
                if (DefaultStudentPersonId == 0)
                {
                    var studentBrowse = new StudentBrowseProcess();
                    StudentSummarys resultStudents = studentBrowse.GetStudents("Any", "%", "%", "%", "%", "%", "%",
                        DateTime.Now, false);

                    foreach (StudentSummary student in resultStudents)
                    {
                        logger.Log(LogLevel.Debug, "Default PupilID: " + student.PersonID);
                        DefaultStudentPersonId = student.PersonID;
                        return DefaultStudentPersonId;
                    }
                }
                return DefaultStudentPersonId;
            }
        }

        /// <summary>
        ///     Returns a string[] of all the Pupil Year Groups from SIMS
        /// </summary>
        public string[] GetPupilYearGroups
        {
            get
            {
                if (pupilYearGroups == null)
                {
                    string yrgrps = null;
                    var studentBrowse = new StudentBrowseProcess();
                    foreach (YearGroup grp in studentBrowse.YearGroups)
                    {
                        if (!string.IsNullOrEmpty(yrgrps))
                        {
                            yrgrps = yrgrps + ",";
                        }
                        yrgrps = yrgrps + grp.Description;
                    }
                    studentBrowse = null;
                    pupilYearGroups = yrgrps.Split(',');
                }
                return pupilYearGroups;
            }
        }

        /// <summary>
        ///     Returns a string[] of all the Student Houses from SIMS
        /// </summary>
        public string[] GetPupilHouses
        {
            get
            {
                if (pupilHouses == null)
                {
                    string houses = null;
                    var studentBrowse = new StudentBrowseProcess();
                    foreach (House grp in studentBrowse.Houses)
                    {
                        if (!string.IsNullOrEmpty(houses))
                        {
                            houses = houses + ",";
                        }
                        houses = houses + grp.Description;
                    }
                    studentBrowse = null;
                    pupilHouses = houses.Split(',');
                }
                return pupilHouses;
            }
        }

        /// <summary>
        ///     Returns a string[] of all the Registration Group from SIMS
        /// </summary>
        public string[] GetPupilRegistrationGroups
        {
            get
            {
                if (pupilRegistrationGroups == null)
                {
                    string reggrps = null;

                    var studentBrowse = new StudentBrowseProcess();
                    foreach (RegistrationGroup grp in studentBrowse.RegistrationGroups)
                    {
                        if (!string.IsNullOrEmpty(reggrps))
                        {
                            reggrps = reggrps + ",";
                        }
                        reggrps = reggrps + grp.Description;
                    }
                    studentBrowse = null;
                    pupilRegistrationGroups = reggrps.Split(',');
                }
                return pupilRegistrationGroups;
            }
        }

        public StudentFilter GetPupilFilter
        {
            get
            {
                string userPupilFilter = null;

                switch (userPupilFilter)
                {
                    case "<Any>":
                        return StudentFilter.Any;
                    case "Current":
                        return StudentFilter.Current;
                    case "Ever On Roll":
                        return StudentFilter.EverOnRoll;
                    case "Guest":
                        return StudentFilter.Guest;
                    case "Leavers":
                        return StudentFilter.Leavers;
                    case "On Roll":
                        return StudentFilter.OnRoll;
                    case "Future":
                        return StudentFilter.NewIntake;
                    default:
                        return StudentFilter.Current;
                }
            }
        }

        public DataTable GetPupilUDFs
        {
            get
            {
                if (pupilUdfs == null)
                {
                    logger.Log(LogLevel.Debug, "GetStudentUDFs");
                    pupilUdfs = createUdfTable;
                    try
                    {
                        var studentsedt = new EditStudentInformationReadOnly();
                        StudentEditResult status = studentsedt.Load(new Person(GetDefaultStudentPersonId), DateTime.Now);

                        foreach (UDFValue udfVal in studentsedt.Student.UDFValues)
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
                                logger.Log(LogLevel.Trace, "GetPupilUDFs:: " + name + " (" + type + ")");
                                DataRow udfRow = pupilUdfs.NewRow();
                                udfRow["Type"] = type;
                                udfRow["Name"] = name;
                                udfRow["Min"] = min;
                                udfRow["Max"] = max;
                                pupilUdfs.Rows.Add(udfRow);
                            }
                        }
                        studentsedt.Dispose();
                    }
                    catch (Exception GetPupilUDFsException)
                    {
                        logger.Log(LogLevel.Error, "GetPupilUDFsException:: " + GetPupilUDFsException);
                    }
                }
                return pupilUdfs;
            }
        }

        private StudentFilter GetPupilUserFilter
        {
            get
            {
                return pupilUserFilter;
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

        public string GetPupilEmail(Int32 personid)
        {
            string result = "";
            try
            {
                var studentsedt = new EditStudentInformationReadOnly();
                StudentEditResult status = studentsedt.Load(new Person(personid), DateTime.Now);
                if (studentsedt.Student.Communication.Emails.Value.Count > 0)
                {
                    logger.Log(LogLevel.Trace, " - eMail: " + studentsedt.Student.Communication.Emails.Value.Count);

                    int workcount = 1;
                    string emailaddresses = null;

                    foreach (EMail item in studentsedt.Student.Communication.Emails.Value)
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
                        workcount = workcount + 1;
                    }
                    if (string.IsNullOrEmpty(emailaddresses))
                    {
                        return "<NO TYPE EMAILS>";
                    }
                    return emailaddresses;
                }
                return "<BLANK>";
            }
            catch (Exception GetPupilEmail_Exception)
            {
                logger.Log(LogLevel.Info, GetPupilEmail_Exception);
            }
            return result;
        }

        public string GetPupilTelephone(Int32 personid)
        {
            string result = "";
            try
            {
                var studentsedt = new EditStudentInformationReadOnly();
                StudentEditResult status = studentsedt.Load(new Person(personid), DateTime.Now);
                if (studentsedt.Student.Communication.Phones.Value.Count > 0)
                {
                    logger.Log(LogLevel.Trace, " - telephone: " + studentsedt.Student.Communication.Phones.Value.Count);

                    int workcount = 1;
                    string telephone = null;

                    foreach (Telephone item in studentsedt.Student.Communication.Phones.Value)
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
                return "<NO TELEPHONE>";
            }
            catch (Exception GetPupilEmail_Exception)
            {
                logger.Log(LogLevel.Info, GetPupilEmail_Exception);
            }
            return result;
        }

        public string GetPupilForename(Int32 personid)
        {
            try
            {
                var studentsedt = new EditStudentInformationReadOnly();
                StudentEditResult status = studentsedt.Load(new Person(personid), DateTime.Now);
                return studentsedt.Student.Forename;
            }
            catch (Exception GetPupilForename_Exception)
            {
                logger.Log(LogLevel.Error, GetPupilForename_Exception);
            }
            return null;
        }

        public string GetPupilSurname(Int32 personid)
        {
            try
            {
                var studentsedt = new EditStudentInformationReadOnly();
                StudentEditResult status = studentsedt.Load(new Person(personid), DateTime.Now);
                return studentsedt.Student.Surname;
            }
            catch (Exception GetPupilSurname_Exception)
            {
                logger.Log(LogLevel.Error, GetPupilSurname_Exception);
            }
            return null;
        }

        public string GetPupilGender(Int32 personid)
        {
            try
            {
                var studentsedt = new EditStudentInformationReadOnly();
                StudentEditResult status = studentsedt.Load(new Person(personid), DateTime.Now);
                return studentsedt.Student.Gender.ToString();
            }
            catch (Exception GetPupilGender_Exception)
            {
                logger.Log(LogLevel.Error, GetPupilGender_Exception);
            }
            return null;
        }

        public string GetPupilYear(Int32 personid)
        {
            try
            {
                var studentsedt = new EditStudentInformationReadOnly();
                StudentEditResult status = studentsedt.Load(new Person(personid), DateTime.Now);
                return studentsedt.Student.YearGroup.Description;
            }
            catch (Exception GetPupilYear_Exception)
            {
                logger.Log(LogLevel.Error, GetPupilYear_Exception);
            }
            return null;
        }

        public string GetPupilHouse(Int32 personid)
        {
            if (pupilHouses != null)
            {
                try
                {
                    var studentsedt = new EditStudentInformationReadOnly();
                    StudentEditResult status = studentsedt.Load(new Person(personid), DateTime.Now);
                    return studentsedt.Student.House.Description;
                }
                catch (Exception GetPupilHouse_Exception)
                {
                    logger.Log(LogLevel.Error, GetPupilHouse_Exception);
                }
            }
            return null;
        }

        public string GetPupilRegistration(Int32 personid)
        {
            try
            {
                var studentsedt = new EditStudentInformationReadOnly();
                StudentEditResult status = studentsedt.Load(new Person(personid), DateTime.Now);
                return studentsedt.Student.RegGroup.Description;
            }
            catch (Exception GetPupilReg_Exception)
            {
                logger.Log(LogLevel.Error, GetPupilReg_Exception);
            }
            return null;
        }

        public string GetPupilDOB(Int32 personid)
        {
            try
            {
                var studentsedt = new EditStudentInformationReadOnly();
                StudentEditResult status = studentsedt.Load(new Person(personid), DateTime.Now);
                return studentsedt.Student.DateOfBirth.ToString().Substring(0, 10);
            }
            catch (Exception GetPupilDOB_Exception)
            {
                logger.Log(LogLevel.Error, GetPupilDOB_Exception);
            }
            return null;
        }

        public string GetPupilAdmissionNumber(Int32 personid)
        {
            try
            {
                var studentsedt = new EditStudentInformationReadOnly();
                StudentEditResult status = studentsedt.Load(new Person(personid), DateTime.Now);
                return studentsedt.Student.AdmissionNumber;
            }
            catch (Exception GetPupilAdmis_Exception)
            {
                logger.Log(LogLevel.Error, GetPupilAdmis_Exception);
            }
            return null;
        }

        public string GetPupilAdmissionDate(Int32 personid)
        {
            try
            {
                var studentsedt = new EditStudentInformationReadOnly();
                StudentEditResult status = studentsedt.Load(new Person(personid), DateTime.Now);
                return studentsedt.Student.DateOfAdmission.ToString();
            }
            catch (Exception GetPupilAdmis_Exception)
            {
                logger.Log(LogLevel.Error, GetPupilAdmis_Exception);
            }
            return null;
        }

        public string GetPupilPersonID(string forename, string surname,
            string reg, string year, string house, string admisno)
        {
            if (string.IsNullOrEmpty(forename))
            {
                forename = "%";
            }
            if (string.IsNullOrEmpty(surname))
            {
                surname = "%";
            }
            if (string.IsNullOrEmpty(reg))
            {
                reg = "%";
            }
            if (string.IsNullOrEmpty(year))
            {
                year = "%";
            }
            if (string.IsNullOrEmpty(house))
            {
                house = "%";
            }
            if (string.IsNullOrEmpty(admisno))
            {
                house = "%";
            }

            string personid = null;

            var studentBrowse = new StudentBrowseProcess();
            StudentSummarys resultStudents = studentBrowse.GetStudents(GetPupilFilter.ToString(), surname, forename, reg,
                year, house, "%", DateTime.Now, false, admisno);
            foreach (StudentSummary student in resultStudents)
            {
                return student.PersonID.ToString();
            }
            return personid;
        }

        public string GetPupilUDF(int personid)
        {
            string result = "";
            try
            {
                var studentsedt = new EditStudentInformationReadOnly();
                StudentEditResult status = studentsedt.Load(new Person(personid), DateTime.Now);

                foreach (UDFValue udfVal in studentsedt.Student.UDFValues)
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
                studentsedt.Dispose();
            }
            catch (Exception GetStudentUdfException)
            {
                logger.Log(LogLevel.Error, GetStudentUdfException);
            }
            return result;
        }

        public bool HasPupilUsername(int personid, string udf)
        {
            bool result = false;
            try
            {
                var studentsedt = new EditStudentInformationReadOnly();
                StudentEditResult status = studentsedt.Load(new Person(personid), DateTime.Now);

                foreach (UDFValue udfVal in studentsedt.Student.UDFValues)
                {
                    if (udfVal.FieldTypeCode == UDFFieldType.STRING1_CODE)
                    {
                        if (udfVal.TypedValueAttribute.Description == udf)
                            return !string.IsNullOrEmpty(((StringAttribute)udfVal.TypedValueAttribute).Value);
                    }
                }
                studentsedt.Dispose();
            }
            catch (Exception GetStudentUdfException)
            {
                logger.Log(LogLevel.Error, GetStudentUdfException);
            }
            return result;
        }

        public List<string> GetPupilUsernameUDFs
        {
            get
            {
                logger.Log(LogLevel.Debug, "GetPupilUsernameUDFs");

                List<string> usernameUDFs = new List<string>();
                try
                {
                    var studentsedt = new EditStudentInformationReadOnly();
                    StudentEditResult status = studentsedt.Load(new Person(GetDefaultStudentPersonId), DateTime.Now);
                    foreach (UDFValue udfVal in studentsedt.Student.UDFValues)
                    {
                        if (udfVal.FieldTypeCode == UDFFieldType.STRING1_CODE)
                            usernameUDFs.Add(udfVal.TypedValueAttribute.Description);
                    }
                    studentsedt.Dispose();
                }
                catch (Exception GetPupilUsernameUDFs_Exception)
                {
                    logger.Log(LogLevel.Error, "GetPupilUsernameUDFs_Exception:: " + GetPupilUsernameUDFs_Exception);
                }
                return usernameUDFs;
            }
        }

        private DataTable GetUsernameData
        {
            get
            {
                logger.Log(LogLevel.Trace, "GetUsernameData");
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("Forename", typeof (string)));
                dt.Columns.Add(new DataColumn("Surname", typeof (string)));
                dt.Columns.Add(new DataColumn("AdmissionNo", typeof (string)));
                dt.Columns.Add(new DataColumn("AdmissionYear", typeof (string)));
                dt.Columns.Add(new DataColumn("YearGroup", typeof (string)));
                dt.Columns.Add(new DataColumn("EntryYear", typeof(string)));
                dt.Columns.Add(new DataColumn("RegGroup", typeof (string)));
                dt.Columns.Add(new DataColumn("SystemId", typeof(string)));
                return dt;
            }
        }

        public DataTable GetPupilHierarchyStructure
        {
            get
            {
                logger.Log(LogLevel.Trace, "GetPupilHierarchyStructure");
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("PersonID", typeof(string)));
                dt.Columns.Add(new DataColumn("Year", typeof(string)));
                dt.Columns.Add(new DataColumn("House", typeof(string)));
                dt.Columns.Add(new DataColumn("IsSet", typeof(bool)));
                return dt;
            }
        }

        public DataTable GetPupilDefaultUsernameData
        {
            get
            {
                logger.Log(LogLevel.Trace, "GetPupilDefaultUsernameData");
                DataTable _dt = GetUsernameData;
                DataRow _dr = _dt.NewRow();
                int defaultStudentPersonId = GetDefaultStudentPersonId;
                _dr["Forename"] = GetPupilForename(defaultStudentPersonId);
                _dr["Surname"] = GetPupilSurname(defaultStudentPersonId);
                _dr["AdmissionNo"] = GetPupilAdmissionNumber(defaultStudentPersonId);
                _dr["AdmissionYear"] = GetPupilAdmissionDate(defaultStudentPersonId);
                _dr["YearGroup"] = GetPupilYear(defaultStudentPersonId);
                _dr["EntryYear"] = "";
                _dr["RegGroup"] = GetPupilRegistration(defaultStudentPersonId);
                _dr["SystemId"] = defaultStudentPersonId.ToString();
                _dt.Rows.Add(_dr);
                return _dt;
            }
        }

        private DataTable _pupilHierarchyData;

        public DataTable GetPupilHierarchyData
        {
            get
            {
                logger.Log(LogLevel.Trace, "GetPupilHierarchyData");
                if (_pupilHierarchyData == null)
                {
                    logger.Log(LogLevel.Trace, "GetPupilHierarchyData - build");
                    _pupilHierarchyData = GetPupilHierarchyStructure;

                    var studentBrowse = new StudentBrowseProcess();
                    StudentSummarys resultStudents = studentBrowse.GetStudents("Current", "%", "%", "%",
                        "%", "%", "%", DateTime.Now, false, "%");
                    foreach (StudentSummary student in resultStudents)
                    {
                        int personId = student.PersonID;
                        DataRow _dr = _pupilHierarchyData.NewRow();
                        _dr["PersonID"] = personId.ToString();
                        _dr["Year"] = student.YearGroup;
                        _dr["House"] = student.House;
                        _dr["IsSet"] = HasPupilUsername(personId, "Test");
                        _pupilHierarchyData.Rows.Add(_dr);
                    }
                }
                return _pupilHierarchyData;
            }
        }

        public int GetPupilHierarchyAllCount
        {
            get
            {
                logger.Log(LogLevel.Trace, "GetPupilHierarchyAllCount");
                return GetPupilHierarchyData.Rows.Count;
            }
        }

        public int GetPupilHierarchyAllCompletedCount
        {
            get
            {
                logger.Log(LogLevel.Trace, "GetPupilHierarchyAllCount");
                return GetPupilHierarchyData.Select("IsSet = true").Length;
            }
        }

        public int GetPupilHierarchyAllNotCompletedCount
        {
            get
            {
                logger.Log(LogLevel.Trace, "GetPupilHierarchyAllCount");
                return GetPupilHierarchyData.Select("IsSet = false").Length;
            }
        }

        public int GetPupilHierarchyItemCount(string type, string item)
        {
            logger.Log(LogLevel.Trace, "GetPupilHierarchyItemCount - type: " + type + " - item: " + item);
            if (type == "Year")
                return GetPupilHierarchyData.Select(type + " = '" + GetPupilYearNo(item) + "'").Length;
            return GetPupilHierarchyData.Select(type + " = '" + item + "'").Length;
        }

        public int GetPupilHierarchyItemCompletedCount(string type, string item)
        {
            logger.Log(LogLevel.Trace, "GetPupilHierarchyItemCount - type: " + type + " - item: " + item);
            if (type == "Year")
                return GetPupilHierarchyData.Select(type + " = '" + GetPupilYearNo(item) + "' AND IsSet = true").Length;
            return GetPupilHierarchyData.Select(type + " = '" + item + "' AND IsSet = true").Length;
        }

        public int GetPupilHierarchyItemNotCompletedCount(string type, string item)
        {
            logger.Log(LogLevel.Trace, "GetPupilHierarchyItemCount - type: " + type + " - item: " + item);
            if (type == "Year")
                return GetPupilHierarchyData.Select(type + " = '" + GetPupilYearNo(item) + "' AND IsSet = false").Length;
            return GetPupilHierarchyData.Select(type + " = '" + item + "' AND IsSet = false").Length;
        }

        public string GetPupilYearNo(string name)
        {
            name = name.ToLower();
            name = name.Replace(" ", "");
            name = name.Replace("year", "");
            return name;
        }
        #endregion

        #region SET
        public string SetPupilUserFilter
        {
            set
            {
                string userFilter = value;
                switch (userFilter)
                {
                    case "Current":
                        pupilUserFilter = StudentFilter.Current;
                        break;
                    case "Ever On Roll":
                        pupilUserFilter = StudentFilter.EverOnRoll;
                        break;
                    case "Guest":
                        pupilUserFilter = StudentFilter.Guest;
                        break;
                    case "Leavers":
                        pupilUserFilter = StudentFilter.Leavers;
                        break;
                    case "On Roll":
                        pupilUserFilter = StudentFilter.OnRoll;
                        break;
                    case "Future":
                        pupilUserFilter = StudentFilter.NewIntake;
                        break;
                    case "<Any>":
                        pupilUserFilter = StudentFilter.Any;
                        break;
                    default:
                        pupilUserFilter = StudentFilter.Current;
                        break;
                }
            }
        }

        public string SetPupilSIMSUDF
        {
            set { simsudf = value; }
        }

        /// <summary>
        /// Writes the Pupil UDF to SIMS .net (string)
        /// </summary>
        /// <param name="personid"></param>
        /// <param name="udfvalue"></param>
        /// <returns></returns>
        public bool SetPupilUDF(int personid, string udfvalue)
        {
            logger.Log(LogLevel.Trace, "SIMSAPI-SetPupilUDF - personid: " + personid + " - udfvalue: " + udfvalue);
            bool result = false;
            try
            {
                //logger.Log(NLog.LogLevel.Info, "PersonId: " + personid + " - UDF: " + udfvalue); 
                using(EditStudentInformation studentsedt = new EditStudentInformation())
                {
                    StudentEditResult status = studentsedt.Load(new Person(personid), DateTime.Now);
                    foreach (UDFValue udfVal in studentsedt.Student.UDFValues)
                    {
                        //logger.Log(NLog.LogLevel.Debug, "Checkpoint1"); 
                        if (simsudf == udfVal.TypedValueAttribute.Description)
                        {
                            if (Core.SetUdf(udfVal, udfvalue))
                            {
                                studentsedt.Save();
                                return true;
                            }
                            else
                            {
                                return false;

                            }
                        }
                    }
                }
                logger.Error("UDF {0} not found.", simsudf);
                return false;
            }
            catch (Exception SetPupilUDF_Exception)
            {
                logger.Log(LogLevel.Error, SetPupilUDF_Exception);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Writes a new Pupil email address into the SIMS database
        /// </summary>
        /// <param name="personid"></param>
        /// <param name="emailValue"></param>
        /// <param name="main"></param>
        /// <param name="primary"></param>
        /// <param name="notes"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool SetPupilEmail(Int32 personid, string emailValue, string main, string primary, string notes, string location)
        {
            try
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Classes.Pupils.SetPupilEmail(personid=" + personid + ", emailValue=" + emailValue + 
                    ", main=" + main + ", primary=" + primary + ", notes=" + notes + ", location=" + location);
                var studentsedt = new EditStudentInformation();
               
                // Load Pupil\Student record
                StudentEditResult status = studentsedt.Load(new Person(personid), DateTime.Now);

                // Create a new email record
                var email = new EMail();

                // Set the email address
                email.AddressAttribute.Value = emailValue;

                // Set Main
                switch (main)
                {
                    case "Yes":
                        email.MainAttribute.Value = (studentsedt.Student.Communication.Emails.Value.Count > 0)
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
                        email.MainAttribute.Value = (studentsedt.Student.Communication.Emails.Value.Count > 0)
                            ? EMailMainCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : EMailMainCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                }

                // Set Primary
                switch (primary)
                {
                    case "Yes":
                        email.PrimaryAttribute.Value = (studentsedt.Student.Communication.Emails.Value.Count > 0)
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
                        email.PrimaryAttribute.Value = (studentsedt.Student.Communication.Emails.Value.Count > 0)
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
                logger.Log(LogLevel.Trace, "Email Valid: " + email.Valid());

                // Save the new record to the database
                studentsedt.Student.Communication.Emails.Add(email);
                StudentEditResult result = studentsedt.Save();
                switch (result)
                {
                    case StudentEditResult.Success:
                        return true;
                    case StudentEditResult.DuplicateFound:
                        logger.Log(LogLevel.Error, "DuplicateFound");
                        return false;
                    case StudentEditResult.Failure:
                        logger.Log(LogLevel.Error, "Failure");
                        return false;
                    case StudentEditResult.Unknown:
                        logger.Log(LogLevel.Error, "Unknown");
                        return false;
                    default:
                        logger.Log(LogLevel.Error, "default");
                        return false;
                }
            }
            catch (Exception SetStudentEmailException)
            {
                logger.Log(LogLevel.Error, SetStudentEmailException);
                return false;
            }
        }

        public bool SetPupilTelephone(Int32 personid, string telephone, string main, string primary, string notes, string location, string device)
        {
            try
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Classes.Pupils.SetPupilTelephone(personid=" + personid + ", telephone=" + telephone +
                    ", main=" + main + ", primary=" + primary + ", notes=" + notes + ", location=" + location + ", device=" + device);
                var studentsedt = new EditStudentInformation();

                // Load Pupil\Student record
                StudentEditResult status = studentsedt.Load(new Person(personid), DateTime.Now);

                // Create a new telephone number
                var phone = new Telephone();

                // Set the telephone number
                phone.NumberAttribute.Value = telephone;

                // Set Main
                switch (main)
                {
                    case "Yes":
                        phone.MainAttribute.Value = (studentsedt.Student.Communication.Phones.Value.Count > 0)
                            ? TelephoneMainCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : TelephoneMainCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                    case "Yes (Overwrite)":
                        phone.MainAttribute.Value = TelephoneMainCollection.GetValues().Item(1);
                        break;
                    case "No":
                        phone.MainAttribute.Value = TelephoneMainCollection.GetValues().Item(0);
                        break;
                    default:
                        phone.MainAttribute.Value = (studentsedt.Student.Communication.Phones.Value.Count > 0)
                            ? TelephoneMainCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : TelephoneMainCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                }

                // Set primary
                switch (primary)
                {
                    case "Yes":
                        phone.PrimaryAttribute.Value = (studentsedt.Student.Communication.Phones.Value.Count > 0)
                            ? TelephoneMainCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : TelephoneMainCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                    case "Yes (Overwrite)":
                        phone.PrimaryAttribute.Value = TelephoneMainCollection.GetValues().Item(1);
                        break;
                    case "No":
                        phone.PrimaryAttribute.Value = TelephoneMainCollection.GetValues().Item(0);
                        break;
                    default:
                        phone.PrimaryAttribute.Value = (studentsedt.Student.Communication.Phones.Value.Count > 0)
                            ? TelephoneMainCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : TelephoneMainCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                }

                // Set the notes
                if (!string.IsNullOrWhiteSpace(notes))
                    phone.NotesAttribute.Value = notes;

                // Set the location
                phone.LocationAttribute.Value = PersonCache.TelephoneLocations.ItemByDescription(location);

                // Set the device (telephone\fax)
                phone.DeviceAttribute.Value = PersonCache.TelephoneDevices.ItemByDescription(device);

                // Run Validation against the new record
                phone.Validate();
                logger.Log(LogLevel.Trace, "Telephone Valid: " + phone.Valid());

                // Writes the new record to the database
                studentsedt.Student.Communication.Phones.Add(phone);
                StudentEditResult result = studentsedt.Save();
                switch (result)
                {
                        case StudentEditResult.Success:
                        return true;
                    case StudentEditResult.DuplicateFound:
                        logger.Log(LogLevel.Error, "DuplicateFound");
                        return false;
                    case StudentEditResult.Failure:
                        logger.Log(LogLevel.Error, "Failure");
                        return false;
                    case StudentEditResult.Unknown:
                        logger.Log(LogLevel.Error, "Unknown");
                        return false;
                    default:
                        logger.Log(LogLevel.Error, "default");
                        return false;
                }
            }
            catch (Exception SetPupilTelephone_Exception)
            {
                logger.Log(LogLevel.Error, "SetPupilTelephone " + SetPupilTelephone_Exception);
                return false;
            }
        }

        public bool SetPupilUsernameUDF(string udfName)
        {
            return false;
        }
        #endregion

    }
}
