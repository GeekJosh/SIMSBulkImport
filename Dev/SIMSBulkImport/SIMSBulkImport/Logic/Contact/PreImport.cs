/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;

namespace Matt40k.SIMSBulkImport.Contact
{
    public class PreImport
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private DataTable importDataTable;
        private DataTable contactTable;

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
                logger.Log(NLog.LogLevel.Debug, "Generating Contact Table");

                contactTable = new DataTable();
                contactTable.Columns.Add(new DataColumn("Status", typeof(string)));
                contactTable.Columns.Add(new DataColumn("Surname", typeof(string)));
                contactTable.Columns.Add(new DataColumn("Forename", typeof(string)));
                contactTable.Columns.Add(new DataColumn("Postcode", typeof(string)));
                contactTable.Columns.Add(new DataColumn("Town", typeof(string)));
                contactTable.Columns.Add(new DataColumn("Import email", typeof(string)));
                contactTable.Columns.Add(new DataColumn("Import UDF", typeof(string)));
                contactTable.Columns.Add(new DataColumn("SIMS email addresses", typeof(string)));
                contactTable.Columns.Add(new DataColumn("SIMS UDF", typeof(string)));
                contactTable.Columns.Add(new DataColumn("PersonID", typeof(string)));
                return contactTable;
            }
        }

        public DataTable AddToDataTable(int recordid)
        {
            string strPersonid = null;
            string strForename = null;
            string strSurname = null;
            string strPostcode = null;
            string strTown = null;
            string strEmail = null;
            string strUdf = null;
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
                try { strUdf = r[Switcher.PreImportClass.GetMatchUDF].ToString(); }
                catch (ArgumentNullException) { }
                try { strPostcode = r[Switcher.PreImportClass.GetMatchPostcode].ToString(); }
                catch (ArgumentNullException) { }
                try { strTown = r[Switcher.PreImportClass.GetMatchTown].ToString(); }
                catch (ArgumentNullException) { }

                strPersonid = Switcher.SimsApiClass.GetContactPersonId(strSurname, strForename, "%", "%");

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
                    status = Switcher.SimsApiClass.GetContactStatus(strPersonid, strEmail, emailsInSims);
                }

                // REMOVED - Add to failures table.

                // Now add to the DataTable
                DataRow newrow = contactTable.NewRow();
                newrow["Surname"] = strSurname;
                newrow["Forename"] = strForename;
                newrow["Postcode"] = strPostcode;
                newrow["Town"] = strTown;
                newrow["Import email"] = strEmail;
                newrow["Import UDF"] = strUdf;
                newrow["Status"] = status;
                newrow["SIMS email addresses"] = emailsInSims;
                newrow["SIMS UDF"] = udfInSims;
                newrow["PersonID"] = strPersonid;
                contactTable.Rows.Add(newrow);
            }
            catch (Exception AddToDataTable_Exception)
            {
                logger.Log(NLog.LogLevel.Debug, AddToDataTable_Exception);
            }

            return contactTable;
        }
    }
}
