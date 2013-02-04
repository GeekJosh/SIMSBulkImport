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
                staffTable.Columns.Add(new DataColumn("Import UDF", typeof(string)));
                staffTable.Columns.Add(new DataColumn("SIMS email addresses", typeof(string)));
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
            string strUdf = null;
            string strStaff = null;
            string strTitle = null;
            string strGender = null;
            string strDob = null;
            string emailsInSims = null;
            string udfInSims = null;
            string status = "NOT FOUND";
            bool importEmail = false;
            bool importUdf = false;
            int pid = 0;

            try
            {
                DataRow r = importDataTable.Rows[recordid];

                try { strForename = r[Switcher.PreImportClass.GetMatchFirstname].ToString(); }
                catch (ArgumentNullException) { }
                try { strSurname = r[Switcher.PreImportClass.GetMatchSurname].ToString(); }
                catch (ArgumentNullException) { }
                try { strEmail = r[Switcher.PreImportClass.GetMatchEmail].ToString(); }
                catch (ArgumentNullException) { }
                try { strStaff = r[Switcher.PreImportClass.GetMatchStaffcode].ToString(); }
                catch (ArgumentNullException) { }
                try { strTitle = r[Switcher.PreImportClass.GetMatchTitle].ToString(); }
                catch (ArgumentNullException) { }
                try { strGender = r[Switcher.PreImportClass.GetMatchGender].ToString(); }
                catch (ArgumentNullException) { }
                try { strUdf = r[Switcher.PreImportClass.GetMatchUDF].ToString(); }
                catch (ArgumentNullException) { }

                strPersonid = Switcher.SimsApiClass.GetStaffPersonid(strSurname, strForename, strTitle, strGender, strStaff);

                if (!string.IsNullOrEmpty(strEmail))
                {
                    importEmail = true;
                    strEmail = strEmail.ToLower();
                }
                if (!string.IsNullOrEmpty(strUdf))
                {
                    importUdf = true;
                }
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
                    emailsInSims = Switcher.SimsApiClass.GetPersonEmail(pid);
                    udfInSims = Switcher.SimsApiClass.GetStaffUdf(pid);

                    if (string.IsNullOrWhiteSpace(strSurname)) { strSurname = Switcher.SimsApiClass.GetPersonSurname(pid); }
                    if (string.IsNullOrWhiteSpace(strForename)) { strForename = Switcher.SimsApiClass.GetPersonForename(pid); }
                    if (string.IsNullOrWhiteSpace(strTitle)) { strTitle = Switcher.SimsApiClass.GetPersonTitle(pid); }
                    if (string.IsNullOrWhiteSpace(strGender)) { strGender = Switcher.SimsApiClass.GetPersonGender(pid); }
                    if (string.IsNullOrWhiteSpace(strStaff)) { strStaff = Switcher.SimsApiClass.GetPersonCode(pid); }
                    if (string.IsNullOrWhiteSpace(strDob)) { strDob = Switcher.SimsApiClass.GetPersonDob(pid); }

                    status = Switcher.SimsApiClass.GetStatus(strPersonid, strEmail, emailsInSims, strUdf, udfInSims);
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
                newrow["Import UDF"] = strUdf;
                newrow["Status"] = status;
                newrow["SIMS email addresses"] = emailsInSims;
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
