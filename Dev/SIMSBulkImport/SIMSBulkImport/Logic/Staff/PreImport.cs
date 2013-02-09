/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;

namespace Matt40k.SIMSBulkImport.Staff
{
    public class PreImport
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private DataTable importDataTable;
        private DataTable staffTable;

        public DataTable SetImportDataTable
        {
            set
            {
                importDataTable = value;
            }
        }

        public DataTable CreateDataTable
        {
            get
            {
                logger.Log(NLog.LogLevel.Info, "Generating Staff Table");

                staffTable = new DataTable();
                staffTable.Columns.Add(new DataColumn("Status", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Surname", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Forename", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Title", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Gender", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Staff Code", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Date of Birth", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Import email", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Import telephone", typeof(string)));
                staffTable.Columns.Add(new DataColumn("Import UDF", typeof(string)));
                staffTable.Columns.Add(new DataColumn("SIMS email addresses", typeof(string)));
                staffTable.Columns.Add(new DataColumn("SIMS telephone", typeof(string)));
                staffTable.Columns.Add(new DataColumn("SIMS UDF", typeof(string)));
                staffTable.Columns.Add(new DataColumn("PersonID", typeof(string)));
                return staffTable;
            }
        }

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

            try
            {
                string matchFirstname = Switcher.PreImportClass.GetMatchFirstname;
                string matchSurname = Switcher.PreImportClass.GetMatchSurname;
                string matchEmail = Switcher.PreImportClass.GetMatchEmail;
                string matchTelephone = Switcher.PreImportClass.GetMatchTelephone;
                string matchUDF = Switcher.PreImportClass.GetMatchUDF;
                string matchGender = Switcher.PreImportClass.GetMatchGender;
                string matchStaffcode = Switcher.PreImportClass.GetMatchStaffcode;
                string matchTitle = Switcher.PreImportClass.GetMatchTitle;

                DataRow r = importDataTable.Rows[recordid];
                if (!string.IsNullOrEmpty(matchFirstname))
                {
                    try
                    {
                        strForename = r[matchFirstname].ToString();
                    }
                    catch (ArgumentNullException AddToDataTable_matchFirstname_ArgumentNullException)
                    {
                        logger.Log(NLog.LogLevel.Trace, AddToDataTable_matchFirstname_ArgumentNullException);
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
                        logger.Log(NLog.LogLevel.Trace, AddToDataTable_matchSurname_ArgumentNullException);
                    }
                }
                if (!string.IsNullOrEmpty(matchEmail))
                {
                    try
                    { 
                        strEmail = r[matchEmail].ToString();
                    }
                    catch (ArgumentNullException AddToDataTable_matchEmail_ArgumentNullException)
                    {
                        logger.Log(NLog.LogLevel.Trace, AddToDataTable_matchEmail_ArgumentNullException);
                    }
                }
                if (!string.IsNullOrEmpty(matchTelephone))
                {
                    try
                    {
                        strTelephone = r[matchTelephone].ToString();
                    }
                    catch (ArgumentNullException AddToDataTable_matchTelephone_ArgumentNullException) 
                    {
                        logger.Log(NLog.LogLevel.Trace, AddToDataTable_matchTelephone_ArgumentNullException);
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
                        logger.Log(NLog.LogLevel.Trace, AddToDataTable_matchUDF_ArgumentNullException);
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
                        logger.Log(NLog.LogLevel.Trace, AddToDataTable_matchGender_ArgumentNullException);
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
                        logger.Log(NLog.LogLevel.Trace, AddToDataTable_matchStaffcode_ArgumentNullException);
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
                        logger.Log(NLog.LogLevel.Trace, AddToDataTable_matchTitle_ArgumentNullException);
                    }
                }

                strPersonid = Switcher.SimsApiClass.GetStaffPersonID(strSurname, strForename, strTitle, strGender, strStaff);

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

                    if (string.IsNullOrWhiteSpace(strSurname)) { strSurname = Switcher.SimsApiClass.GetStaffSurname(pid); }
                    if (string.IsNullOrWhiteSpace(strForename)) { strForename = Switcher.SimsApiClass.GetStaffForename(pid); }
                    if (string.IsNullOrWhiteSpace(strTitle)) { strTitle = Switcher.SimsApiClass.GetStaffTitle(pid); }
                    if (string.IsNullOrWhiteSpace(strGender)) { strGender = Switcher.SimsApiClass.GetStaffGender(pid); }
                    if (string.IsNullOrWhiteSpace(strStaff)) { strStaff = Switcher.SimsApiClass.GetStaffCode(pid); }
                    if (string.IsNullOrWhiteSpace(strDob)) { strDob = Switcher.SimsApiClass.GetStaffDOB(pid); }

                    status = Switcher.PreImportClass.GetStatus(strPersonid, strEmail, emailsInSims, strUdf, udfInSims);
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
                staffTable.Rows.Add(newrow);
            }
            catch (Exception AddToDataTable_Exception)
            {
                logger.Log(NLog.LogLevel.Debug, AddToDataTable_Exception);
            }
            return staffTable;
        }
    }
}
