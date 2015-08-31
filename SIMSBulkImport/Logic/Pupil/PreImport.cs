using System;
using System.Data;
using NLog;

namespace Matt40k.SIMSBulkImport.Pupil
{
    public class PreImport
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private DataTable importDataTable;
        private DataTable pupilTable;

        public DataTable SetImportDataTable
        {
            set { importDataTable = value; }
        }

        /// <summary>
        /// Create a new import 'To-Do' DataTable - Pupil
        /// </summary>
        public DataTable CreateDataTable
        {
            get
            {
                logger.Log(LogLevel.Info, "Generating Pupil Table");

                pupilTable = new DataTable();
                pupilTable.Columns.Add(new DataColumn("Status", typeof (string)));
                pupilTable.Columns.Add(new DataColumn("Surname", typeof (string)));
                pupilTable.Columns.Add(new DataColumn("Forename", typeof (string)));
                pupilTable.Columns.Add(new DataColumn("Gender", typeof (string)));
                pupilTable.Columns.Add(new DataColumn("Admission Number", typeof (string)));
                pupilTable.Columns.Add(new DataColumn("Date of Birth", typeof (string)));
                pupilTable.Columns.Add(new DataColumn("Year", typeof (string)));
                pupilTable.Columns.Add(new DataColumn("Registration", typeof (string)));
                pupilTable.Columns.Add(new DataColumn("House", typeof (string)));
                pupilTable.Columns.Add(new DataColumn("Import email", typeof (string)));
                pupilTable.Columns.Add(new DataColumn("Import telephone", typeof (string)));
                pupilTable.Columns.Add(new DataColumn("Import UDF", typeof (string)));
                pupilTable.Columns.Add(new DataColumn("SIMS email addresses", typeof (string)));
                pupilTable.Columns.Add(new DataColumn("SIMS telephone", typeof (string)));
                pupilTable.Columns.Add(new DataColumn("SIMS UDF", typeof (string)));
                pupilTable.Columns.Add(new DataColumn("PersonID", typeof (string)));
                pupilTable.Columns.Add(new DataColumn("Main", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Primary", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Notes", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Location", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Device", typeof(string)));
                return pupilTable;
            }
        }

        /// <summary>
        /// Adds a item to the 'To-Do' DataTable
        /// </summary>
        /// <param name="recordid"></param>
        /// <returns></returns>
        public DataTable AddToDataTable(int recordid)
        {
            string strPersonid = null;
            string strForename = null;
            string strSurname = null;
            string strEmail = null;
            string strTelephone = null;
            string strUdf = null;
            string strAdmis = null;
            string strGender = null;
            string strDob = null;
            string strYear = null;
            string strReg = null;
            string strHouse = null;
            string emailsInSims = null;
            string telephonesInSims = null;
            string udfInSims = null;
            string status = "NOT FOUND";
            int pid = 0;

            string strMain = null;
            string strPrimary = null;
            string strLocation = null;
            string strNotes = null;
            string strDevice = null;

            try
            {
                string matchPersonID = Switcher.PreImportClass.GetMatchPersonID;
                string matchFirstname = Switcher.PreImportClass.GetMatchFirstname;
                string matchSurname = Switcher.PreImportClass.GetMatchSurname;
                string matchEmail = Switcher.PreImportClass.GetMatchEmail;
                string matchTelephone = Switcher.PreImportClass.GetMatchTelephone;
                string matchStaffcode = Switcher.PreImportClass.GetMatchStaffcode;
                string matchUDF = Switcher.PreImportClass.GetMatchUDF;
                string matchGender = Switcher.PreImportClass.GetMatchGender;
                string matchYear = Switcher.PreImportClass.GetMatchYear;
                string matchReg = Switcher.PreImportClass.GetMatchReg;
                string matchHouse = Switcher.PreImportClass.GetMatchHouse;

                DataRow r = importDataTable.Rows[recordid];
                if (!string.IsNullOrEmpty(matchPersonID))
                {
                    try
                    {
                        strPersonid = r[matchPersonID].ToString();
                    }
                    catch (ArgumentNullException AddToDataTable_matchPersonID_ArgumentNullException)
                    {
                        logger.Log(LogLevel.Trace, AddToDataTable_matchPersonID_ArgumentNullException);
                    }
                }
                if (!string.IsNullOrEmpty(matchFirstname))
                {
                    try
                    {
                        strForename = r[matchFirstname].ToString();
                    }
                    catch (ArgumentNullException AddToDataTable_matchFirstname_ArgumentNullException)
                    {
                        logger.Log(LogLevel.Trace, AddToDataTable_matchFirstname_ArgumentNullException);
                    }
                }
                if (!string.IsNullOrEmpty(matchSurname))
                {
                    try
                    {
                        strSurname = r[matchSurname].ToString();
                    }
                    catch (ArgumentNullException AddToDataTable_matchSurname_ArgumentNullException)
                    {
                        logger.Log(LogLevel.Trace, AddToDataTable_matchSurname_ArgumentNullException);
                    }
                }
                if (!string.IsNullOrEmpty(matchEmail))
                {
                    string matchMain = Switcher.PreImportClass.GetMatchEmailMain;
                    string matchPrimary = Switcher.PreImportClass.GetMatchEmailPrimary;
                    string matchLocation = Switcher.PreImportClass.GetMatchEmailLocation;
                    string matchNotes = Switcher.PreImportClass.GetMatchEmailNotes;

                    try
                    {
                        strEmail = r[matchEmail].ToString();
                    }
                    catch (ArgumentNullException AddToDataTable_matchEmail_ArgumentNullException)
                    {
                        logger.Log(LogLevel.Trace, AddToDataTable_matchEmail_ArgumentNullException);
                    }

                    // Email - Main
                    if (IsDefault(matchMain))
                        strMain = Switcher.ConfigManClass.GetDefaultEmailMain;
                    else
                    {
                        try
                        {
                            strMain = r[matchMain].ToString();
                        }
                        catch (ArgumentNullException AddToDataTable_matchEmail_Main_ArgumentNullException)
                        {
                            logger.Log(LogLevel.Trace, AddToDataTable_matchEmail_Main_ArgumentNullException);
                        }
                    }

                    // Email - Primary
                    if (IsDefault(matchPrimary))
                        strPrimary = Switcher.ConfigManClass.GetDefaultEmailPrimary;
                    else
                    {
                        try
                        {
                            strPrimary = r[matchPrimary].ToString();
                        }
                        catch (ArgumentNullException AddToDataTable_matchEmail_Primary_ArgumentNullException)
                        {
                            logger.Log(LogLevel.Trace, AddToDataTable_matchEmail_Primary_ArgumentNullException);
                        }
                    }

                    // Email - Location
                    if (IsDefault(matchLocation))
                        strLocation = Switcher.ConfigManClass.GetDefaultEmailLocation;
                    else
                    {
                        try
                        {
                            strLocation = r[matchLocation].ToString();
                        }
                        catch (ArgumentNullException AddToDataTable_matchEmail_Location_ArgumentNullException)
                        {
                            logger.Log(LogLevel.Trace, AddToDataTable_matchEmail_Location_ArgumentNullException);
                        }
                    }

                    // Email - Notes
                    if (string.IsNullOrEmpty(matchNotes) || IsDefault(matchNotes))
                        strNotes = Switcher.ConfigManClass.GetDefaultEmailNotes;
                    else
                    {
                        try
                        {
                            strNotes = r[matchNotes].ToString();
                        }
                        catch (ArgumentNullException AddToDataTable_matchEmail_Notes_ArgumentNullException)
                        {
                            logger.Log(LogLevel.Trace, AddToDataTable_matchEmail_Notes_ArgumentNullException);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(matchTelephone))
                {
                    string matchMain = Switcher.PreImportClass.GetMatchTelephoneMain;
                    string matchPrimary = Switcher.PreImportClass.GetMatchTelephonePrimary;
                    string matchLocation = Switcher.PreImportClass.GetMatchTelephoneLocation;
                    string matchNotes = Switcher.PreImportClass.GetMatchTelephoneNotes;
                    string matchDevice = Switcher.PreImportClass.GetMatchTelephoneDevice;

                    try
                    {
                        strTelephone = r[matchTelephone].ToString();
                    }
                    catch (ArgumentNullException AddToDataTable_matchTelephone_ArgumentNullException)
                    {
                        logger.Log(LogLevel.Trace, AddToDataTable_matchTelephone_ArgumentNullException);
                    }

                    // Telephone - Main
                    if (IsDefault(matchMain))
                        strMain = Switcher.ConfigManClass.GetDefaultTelephoneMain;
                    else
                    {
                        try
                        {
                            strMain = r[matchMain].ToString();
                        }
                        catch (ArgumentNullException AddToDataTable_matchTelephone_Main_ArgumentNullException)
                        {
                            logger.Log(LogLevel.Trace, AddToDataTable_matchTelephone_Main_ArgumentNullException);
                        }
                    }

                    // Telephone - Primary
                    if (IsDefault(matchPrimary))
                        strPrimary = Switcher.ConfigManClass.GetDefaultTelephonePrimary;
                    else
                    {
                        try
                        {
                            strPrimary = r[matchPrimary].ToString();
                        }
                        catch (ArgumentNullException AddToDataTable_matchTelephone_Primary_ArgumentNullException)
                        {
                            logger.Log(LogLevel.Trace, AddToDataTable_matchTelephone_Primary_ArgumentNullException);
                        }
                    }

                    // Telephone - Location
                    if (IsDefault(matchLocation))
                        strLocation = Switcher.ConfigManClass.GetDefaultTelephoneLocation;
                    else
                    {
                        try
                        {
                            strLocation = r[matchLocation].ToString();
                        }
                        catch (ArgumentNullException AddToDataTable_matchTelephone_Location_ArgumentNullException)
                        {
                            logger.Log(LogLevel.Trace, AddToDataTable_matchTelephone_Location_ArgumentNullException);
                        }
                    }

                    // Telephone - Notes
                    if (IsDefault(matchNotes))
                        strNotes = Switcher.ConfigManClass.GetDefaultTelephoneNotes;
                    else
                    {
                        try
                        {
                            strNotes = r[matchNotes].ToString();
                        }
                        catch (ArgumentNullException AddToDataTable_matchTelephone_Notes_ArgumentNullException)
                        {
                            logger.Log(LogLevel.Trace, AddToDataTable_matchTelephone_Notes_ArgumentNullException);
                        }
                    }

                    // Telephone - Device
                    if (IsDefault(matchDevice))
                        strDevice = Switcher.ConfigManClass.GetDefaultTelephoneDevice;
                    else
                    {
                        try
                        {
                            strDevice = r[matchDevice].ToString();
                        }
                        catch (ArgumentNullException AddToDataTable_matchTelephone_Notes_ArgumentNullException)
                        {
                            logger.Log(LogLevel.Trace, AddToDataTable_matchTelephone_Notes_ArgumentNullException);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(matchUDF))
                {
                    try
                    {
                        strUdf = r[matchUDF].ToString();
                    }
                    catch (ArgumentNullException AddToDataTable_matchUDF_ArgumentNullException)
                    {
                        logger.Log(LogLevel.Trace, AddToDataTable_matchUDF_ArgumentNullException);
                    }
                }
                if (!string.IsNullOrEmpty(matchGender))
                {
                    try
                    {
                        strGender = r[matchGender].ToString();
                    }
                    catch (ArgumentNullException AddToDataTable_matchGender_ArgumentNullException)
                    {
                        logger.Log(LogLevel.Trace, AddToDataTable_matchGender_ArgumentNullException);
                    }
                }
                if (!string.IsNullOrEmpty(matchStaffcode))
                {
                    try
                    {
                        strAdmis = r[matchStaffcode].ToString();
                    }
                    catch (ArgumentNullException AddToDataTable_matchStaffcode_ArgumentNullException)
                    {
                        logger.Log(LogLevel.Trace, AddToDataTable_matchStaffcode_ArgumentNullException);
                    }
                }

                if (!string.IsNullOrEmpty(matchYear))
                {
                    try
                    {
                        strYear = r[matchYear].ToString();
                    }
                    catch (ArgumentNullException AddToDataTable_matchYear_ArgumentNullException)
                    {
                        logger.Log(LogLevel.Trace, AddToDataTable_matchYear_ArgumentNullException);
                    }
                }
                if (!string.IsNullOrEmpty(matchReg))
                {
                    try
                    {
                        strReg = r[matchReg].ToString();
                    }
                    catch (ArgumentNullException AddToDataTable_matchReg_ArgumentNullException)
                    {
                        logger.Log(LogLevel.Trace, AddToDataTable_matchReg_ArgumentNullException);
                    }
                }
                if (!string.IsNullOrEmpty(matchHouse))
                {
                    try
                    {
                        strHouse = r[matchHouse].ToString();
                    }
                    catch (ArgumentNullException AddToDataTable_matchHouse_ArgumentNullException)
                    {
                        logger.Log(LogLevel.Trace, AddToDataTable_matchHouse_ArgumentNullException);
                    }
                }

                if (string.IsNullOrWhiteSpace(strPersonid))
                    strPersonid = Switcher.SimsApiClass.GetPupilPersonID(strForename, strSurname, strReg, strYear, strHouse,
                        strAdmis);

                try
                {
                    pid = Convert.ToInt32(strPersonid);
                }
                catch
                {
                    logger.Error("Unable to convert '" + strPersonid + "' to int32");
                }

                if (pid > 0)
                {
                    // Person has been found - so we pull in missing data fields :)
                    if (string.IsNullOrWhiteSpace(strForename))
                    {
                        strForename = Switcher.SimsApiClass.GetPupilForename(pid);
                    }
                    if (string.IsNullOrWhiteSpace(strSurname))
                    {
                        strSurname = Switcher.SimsApiClass.GetPupilSurname(pid);
                    }

                    emailsInSims = Switcher.SimsApiClass.GetPupilEmail(pid);
                    udfInSims = Switcher.SimsApiClass.GetPupilUDF(pid);
                    telephonesInSims = Switcher.SimsApiClass.GetPupilTelephone(pid);

                    if (string.IsNullOrWhiteSpace(strDob))
                    {
                        strDob = Switcher.SimsApiClass.GetPupilDOB(pid);
                    }
                    if (string.IsNullOrWhiteSpace(strAdmis))
                    {
                        strAdmis = Switcher.SimsApiClass.GetPupilAdmissionNumber(pid);
                    }
                    if (string.IsNullOrWhiteSpace(strGender))
                    {
                        strGender = Switcher.SimsApiClass.GetPupilGender(pid);
                    }
                    if (string.IsNullOrWhiteSpace(strYear))
                    {
                        strYear = Switcher.SimsApiClass.GetPupilYear(pid);
                    }
                    if (string.IsNullOrWhiteSpace(strReg))
                    {
                        strReg = Switcher.SimsApiClass.GetPupilRegistration(pid);
                    }
                    if (string.IsNullOrWhiteSpace(strHouse))
                    {
                        strHouse = Switcher.SimsApiClass.GetPupilHouse(pid);
                    }

                    status = Switcher.PreImportClass.GetStatus(strPersonid, strEmail, emailsInSims, strUdf, udfInSims,
                        strTelephone, telephonesInSims);
                }

                // REMOVED - Add to failures table.

                // Now add to the DataTable
                DataRow newrow = pupilTable.NewRow();
                newrow["Surname"] = strSurname;
                newrow["Forename"] = strForename;
                newrow["Gender"] = strGender;
                newrow["Admission Number"] = strAdmis;
                newrow["Year"] = strYear;
                newrow["Registration"] = strReg;
                newrow["House"] = strHouse;
                newrow["Date of Birth"] = strDob;
                newrow["Import email"] = strEmail;
                newrow["Import telephone"] = strTelephone;
                newrow["Import UDF"] = strUdf;
                newrow["Status"] = status;
                newrow["SIMS email addresses"] = emailsInSims;
                newrow["SIMS telephone"] = telephonesInSims;
                newrow["SIMS UDF"] = udfInSims;
                newrow["PersonID"] = strPersonid;
                newrow["Main"] = strMain;
                newrow["Primary"] = strPrimary;
                newrow["Notes"] = strNotes;
                newrow["Location"] = strLocation;
                newrow["Device"] = strDevice;
                pupilTable.Rows.Add(newrow);
            }
            catch (Exception AddToDataTable_Exception)
            {
                logger.Log(LogLevel.Debug, AddToDataTable_Exception);
            }
            return pupilTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsDefault(string value)
        {
            logger.Log(LogLevel.Debug, "Matt40k.SIMSBulkImport.Pupil.PreImport.IsDefault(value: " + value + ")");
            if (value == "<Default>")
                return true;
            else
                return false;
        }
    }
}