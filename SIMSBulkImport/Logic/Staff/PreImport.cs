/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;
using NLog;

namespace Matt40k.SIMSBulkImport.Staff
{
    public class PreImport
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private DataTable importDataTable;
        private DataTable staffTable;

        public DataTable SetImportDataTable
        {
            set { importDataTable = value; }
        }

        /// <summary>
        /// Create a new import 'To-Do' DataTable - Staff
        /// </summary>
        public DataTable CreateDataTable
        {
            get
            {
                logger.Log(LogLevel.Info, "Generating Staff Table");

                staffTable = new DataTable();
                staffTable.Columns.Add(new DataColumn("Status", typeof (string)));
                staffTable.Columns.Add(new DataColumn("Surname", typeof (string)));
                staffTable.Columns.Add(new DataColumn("Forename", typeof (string)));
                staffTable.Columns.Add(new DataColumn("Title", typeof (string)));
                staffTable.Columns.Add(new DataColumn("Gender", typeof (string)));
                staffTable.Columns.Add(new DataColumn("Staff Code", typeof (string)));
                staffTable.Columns.Add(new DataColumn("Date of Birth", typeof (string)));
                staffTable.Columns.Add(new DataColumn("Import email", typeof (string)));
                staffTable.Columns.Add(new DataColumn("Import telephone", typeof (string)));
                staffTable.Columns.Add(new DataColumn("Import UDF", typeof (string)));
                staffTable.Columns.Add(new DataColumn("SIMS email addresses", typeof (string)));
                staffTable.Columns.Add(new DataColumn("SIMS telephone", typeof (string)));
                staffTable.Columns.Add(new DataColumn("SIMS UDF", typeof (string)));
                staffTable.Columns.Add(new DataColumn("PersonID", typeof (string)));
                staffTable.Columns.Add(new DataColumn("Main", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Primary", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Notes", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Location", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Device", typeof(string)));
                return staffTable;
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
            string strStaff = null;
            string strTitle = null;
            string strGender = null;
            string strDob = null;
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
                string matchUDF = Switcher.PreImportClass.GetMatchUDF;
                string matchGender = Switcher.PreImportClass.GetMatchGender;
                string matchStaffcode = Switcher.PreImportClass.GetMatchStaffcode;
                string matchTitle = Switcher.PreImportClass.GetMatchTitle;


                

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
                    if (IsDefault(matchNotes))
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
                        strStaff = r[matchStaffcode].ToString();
                    }
                    catch (ArgumentNullException AddToDataTable_matchStaffcode_ArgumentNullException)
                    {
                        logger.Log(LogLevel.Trace, AddToDataTable_matchStaffcode_ArgumentNullException);
                    }
                }
                if (!string.IsNullOrEmpty(matchTitle))
                {
                    try
                    {
                        strTitle = r[matchTitle].ToString();
                    }
                    catch (ArgumentNullException AddToDataTable_matchTitle_ArgumentNullException)
                    {
                        logger.Log(LogLevel.Trace, AddToDataTable_matchTitle_ArgumentNullException);
                    }
                }

                if (string.IsNullOrWhiteSpace(strPersonid))
                    strPersonid = Switcher.SimsApiClass.GetStaffPersonID(strSurname, strForename, strTitle, strGender,
                        strStaff);

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
                    emailsInSims = Switcher.SimsApiClass.GetStaffEmail(pid);
                    udfInSims = Switcher.SimsApiClass.GetStaffUDF(pid);
                    telephonesInSims = Switcher.SimsApiClass.GetStaffTelephone(pid);

                    if (string.IsNullOrWhiteSpace(strSurname))
                    {
                        strSurname = Switcher.SimsApiClass.GetStaffSurname(pid);
                    }
                    if (string.IsNullOrWhiteSpace(strForename))
                    {
                        strForename = Switcher.SimsApiClass.GetStaffForename(pid);
                    }
                    if (string.IsNullOrWhiteSpace(strTitle))
                    {
                        strTitle = Switcher.SimsApiClass.GetStaffTitle(pid);
                    }
                    if (string.IsNullOrWhiteSpace(strGender))
                    {
                        strGender = Switcher.SimsApiClass.GetStaffGender(pid);
                    }
                    if (string.IsNullOrWhiteSpace(strStaff))
                    {
                        strStaff = Switcher.SimsApiClass.GetStaffCode(pid);
                    }
                    if (string.IsNullOrWhiteSpace(strDob))
                    {
                        strDob = Switcher.SimsApiClass.GetStaffDOB(pid);
                    }

                    status = Switcher.PreImportClass.GetStatus(strPersonid, strEmail, emailsInSims, strUdf, udfInSims,
                        strTelephone, telephonesInSims);
                }

                // REMOVED - Add to failures table.

                // Now add to the DataTable
                DataRow newrow = staffTable.NewRow();
                newrow["Surname"] = strSurname;
                newrow["Forename"] = strForename;
                newrow["Title"] = strTitle;
                newrow["Gender"] = strGender;
                newrow["Staff Code"] = strStaff;
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
                staffTable.Rows.Add(newrow);
            }
            catch (Exception AddToDataTable_Exception)
            {
                logger.Log(LogLevel.Debug, AddToDataTable_Exception);
            }
            return staffTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsDefault(string value)
        {
            logger.Log(LogLevel.Debug, "Matt40k.SIMSBulkImport.Staff.PreImport.IsDefault(value: " + value + ")");
            if (value == "<Default>")
                return true;
            else
                return false;
        }
    }
}