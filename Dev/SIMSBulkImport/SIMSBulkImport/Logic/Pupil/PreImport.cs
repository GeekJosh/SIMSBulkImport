/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;

namespace Matt40k.SIMSBulkImport.Pupil
{
    public class PreImport
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private DataTable importDataTable;
        private DataTable pupilTable;

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
                logger.Log(NLog.LogLevel.Info, "Generating Pupil Table");

                pupilTable = new DataTable();
                pupilTable.Columns.Add(new DataColumn("Status", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Surname", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Forename", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Gender", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Admission Number", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Date of Birth", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Year", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Registration", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("House", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Import email", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("Import UDF", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("SIMS email addresses", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("SIMS UDF", typeof(string)));
                pupilTable.Columns.Add(new DataColumn("PersonID", typeof(string)));
                return pupilTable;
            }
        }

        public DataTable AddToDataTable(int recordid)
        {
            string strPersonid = null;
            string strForename = null;
            string strSurname = null;
            string strEmail = null;
            string strUdf = null;
            string strAdmis = null;
            string strGender = null;
            string strDob = null;
            string strYear = null;
            string strReg = null;
            string strHouse = null;
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
                try { strAdmis = r[Switcher.PreImportClass.GetMatchStaffcode].ToString(); }
                catch (ArgumentNullException) { }
                try { strGender = r[Switcher.PreImportClass.GetMatchGender].ToString(); }
                catch (ArgumentNullException) { }
                try { strUdf = r[Switcher.PreImportClass.GetMatchUDF].ToString(); }
                catch (ArgumentNullException) { }

                try { strYear = r[Switcher.PreImportClass.GetMatchYear].ToString(); }
                catch (ArgumentNullException) { }
                try { strReg = r[Switcher.PreImportClass.GetMatchReg].ToString(); }
                catch (ArgumentNullException) { }
                try { strHouse = r[Switcher.PreImportClass.GetMatchHouse].ToString(); }
                catch (ArgumentNullException) { }

                strPersonid = Switcher.SimsApiClass.GetStudentPersonId(strForename, strSurname, strReg, strYear, strHouse, strAdmis).ToString();
                
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
                    if (string.IsNullOrWhiteSpace(strForename)) { strForename = Switcher.SimsApiClass.GetPupilForename(pid); }
                    if (string.IsNullOrWhiteSpace(strSurname)) { strSurname = Switcher.SimsApiClass.GetPupilSurname(pid); }

                    emailsInSims = Switcher.SimsApiClass.GetPupilEmail(pid);
                    udfInSims = Switcher.SimsApiClass.GetPupilUdf(pid);

                    if (string.IsNullOrWhiteSpace(strDob)) { strDob = Switcher.SimsApiClass.GetPupilDOB(pid); }
                    if (string.IsNullOrWhiteSpace(strAdmis)) { strAdmis = Switcher.SimsApiClass.GetPupilAdmis(pid); }
                    if (string.IsNullOrWhiteSpace(strGender)) { strGender = Switcher.SimsApiClass.GetPupilGender(pid); }
                    if (string.IsNullOrWhiteSpace(strYear)) { strYear = Switcher.SimsApiClass.GetPupilYear(pid); }
                    if (string.IsNullOrWhiteSpace(strReg)) { strReg = Switcher.SimsApiClass.GetPupilReg(pid); }
                    if (string.IsNullOrWhiteSpace(strHouse)) { strHouse = Switcher.SimsApiClass.GetPupilHouse(pid); }

                    status = Switcher.PreImportClass.GetStatus(strPersonid, strEmail, emailsInSims, strUdf, udfInSims);
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
                newrow["Import UDF"] = strUdf;
                newrow["Status"] = status;
                newrow["SIMS email addresses"] = emailsInSims;
                newrow["SIMS UDF"] = udfInSims;
                newrow["PersonID"] = strPersonid;
                pupilTable.Rows.Add(newrow);
            }
            catch (Exception AddToDataTable_Exception)
            {
                logger.Log(NLog.LogLevel.Debug, AddToDataTable_Exception);
            }
            return pupilTable;
        }
    }
}
