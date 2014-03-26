/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;
using NLog;

namespace Matt40k.SIMSBulkImport.Contact
{
    public class PreImport
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private DataTable contactTable;
        private DataTable importDataTable;

        public DataTable SetImportDataTable
        {
            set { importDataTable = value; }
        }

        public DataTable CreateDataTable
        {
            get
            {
                logger.Log(LogLevel.Debug, "Generating Contact Table");

                contactTable = new DataTable();
                contactTable.Columns.Add(new DataColumn("Status", typeof (string)));
                contactTable.Columns.Add(new DataColumn("Surname", typeof (string)));
                contactTable.Columns.Add(new DataColumn("Forename", typeof (string)));
                contactTable.Columns.Add(new DataColumn("Postcode", typeof (string)));
                contactTable.Columns.Add(new DataColumn("Town", typeof (string)));
                contactTable.Columns.Add(new DataColumn("Import email", typeof (string)));
                contactTable.Columns.Add(new DataColumn("Import telephone", typeof (string)));
                contactTable.Columns.Add(new DataColumn("Import UDF", typeof (string)));
                contactTable.Columns.Add(new DataColumn("SIMS email addresses", typeof (string)));
                contactTable.Columns.Add(new DataColumn("SIMS telephone", typeof (string)));
                contactTable.Columns.Add(new DataColumn("SIMS UDF", typeof (string)));
                contactTable.Columns.Add(new DataColumn("PersonID", typeof (string)));
                contactTable.Columns.Add(new DataColumn("Main", typeof(int)));
                contactTable.Columns.Add(new DataColumn("Primary", typeof(int)));
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
            string strTelephone = null;
            string strUdf = null;
            string emailsInSims = null;
            string telephonesInSims = null;
            string udfInSims = null;
            string status = "NOT FOUND";
            int pid = 0;

            try
            {
                string matchPersonID = Switcher.PreImportClass.GetMatchPersonID;
                string matchFirstname = Switcher.PreImportClass.GetMatchFirstname;
                string matchSurname = Switcher.PreImportClass.GetMatchSurname;
                string matchEmail = Switcher.PreImportClass.GetMatchEmail;
                string matchTelephone = Switcher.PreImportClass.GetMatchTelephone;
                string matchUDF = Switcher.PreImportClass.GetMatchUDF;
                string matchPostcode = Switcher.PreImportClass.GetMatchPostcode;
                string matchTown = Switcher.PreImportClass.GetMatchTown;

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
                    try
                    {
                        strEmail = r[matchEmail].ToString();
                    }
                    catch (ArgumentNullException AddToDataTable_matchEmail_ArgumentNullException)
                    {
                        logger.Log(LogLevel.Trace, AddToDataTable_matchEmail_ArgumentNullException);
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
                        logger.Log(LogLevel.Trace, AddToDataTable_matchTelephone_ArgumentNullException);
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
                if (!string.IsNullOrEmpty(matchPostcode))
                {
                    try
                    {
                        strPostcode = r[matchPostcode].ToString();
                    }
                    catch (ArgumentNullException AddToDataTable_matchPostcode_ArgumentNullException)
                    {
                        logger.Log(LogLevel.Trace, AddToDataTable_matchPostcode_ArgumentNullException);
                    }
                }
                if (!string.IsNullOrEmpty(matchTown))
                {
                    try
                    {
                        strTown = r[matchTown].ToString();
                    }
                    catch (ArgumentNullException AddToDataTable_matchTown_ArgumentNullException)
                    {
                        logger.Log(LogLevel.Trace, AddToDataTable_matchTown_ArgumentNullException);
                    }
                }

                if (string.IsNullOrWhiteSpace(strPersonid))
                    strPersonid = Switcher.SimsApiClass.GetContactPersonID(strSurname, strForename, "%", "%");

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
                    emailsInSims = Switcher.SimsApiClass.GetContactEmail(pid);
                    status = Switcher.PreImportClass.GetStatus(strPersonid, strEmail, emailsInSims, strUdf, udfInSims,
                        strTelephone, telephonesInSims);
                    telephonesInSims = Switcher.SimsApiClass.GetContactTelephone(pid);
                }

                // REMOVED - Add to failures table.

                // Now add to the DataTable
                DataRow newrow = contactTable.NewRow();
                newrow["Surname"] = strSurname;
                newrow["Forename"] = strForename;
                newrow["Postcode"] = strPostcode;
                newrow["Town"] = strTown;
                newrow["Import email"] = strEmail;
                newrow["Import telephone"] = strTelephone;
                newrow["Import UDF"] = strUdf;
                newrow["Status"] = status;
                newrow["SIMS email addresses"] = emailsInSims;
                newrow["SIMS telephone"] = telephonesInSims;
                newrow["SIMS UDF"] = udfInSims;
                newrow["PersonID"] = strPersonid;
                contactTable.Rows.Add(newrow);
            }
            catch (Exception AddToDataTable_Exception)
            {
                logger.Log(LogLevel.Debug, AddToDataTable_Exception);
            }

            return contactTable;
        }
    }
}