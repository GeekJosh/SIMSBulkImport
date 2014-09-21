/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    public class ImportList
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly DataTable _importTable;

        /// <summary>
        /// 
        /// 
        /// Main:
        ///   1=Set as main (overwrite)
        ///   2=Don't set as main
        ///   3=Set as main if no other main exists
        ///   
        /// Primary:
        ///   1=Set as primary (overwrite)
        ///   2=Don't set as primary
        ///   3=Set as primary if no other primary exists

        /// </summary>
        public ImportList()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportList()");
            Switcher.ResultsImportClass = new ResultsImport();
            _importTable = new DataTable();
            _importTable.Columns.Add(new DataColumn("Type", typeof (string)));
            _importTable.Columns.Add(new DataColumn("PersonID", typeof (int)));
            _importTable.Columns.Add(new DataColumn("Value", typeof (string)));
            _importTable.Columns.Add(new DataColumn("Surname", typeof (string)));
            _importTable.Columns.Add(new DataColumn("Forename", typeof (string)));
            _importTable.Columns.Add(new DataColumn("Postcode", typeof (string)));
            _importTable.Columns.Add(new DataColumn("Town", typeof (string)));
            _importTable.Columns.Add(new DataColumn("Admission Number", typeof (string)));
            _importTable.Columns.Add(new DataColumn("Year", typeof (string)));
            _importTable.Columns.Add(new DataColumn("Registration", typeof (string)));
            _importTable.Columns.Add(new DataColumn("House", typeof (string)));
            _importTable.Columns.Add(new DataColumn("Title", typeof (string)));
            _importTable.Columns.Add(new DataColumn("Gender", typeof (string)));
            _importTable.Columns.Add(new DataColumn("Staff Code", typeof (string)));
            _importTable.Columns.Add(new DataColumn("Date of Birth", typeof (string)));
            _importTable.Columns.Add(new DataColumn("Main", typeof(string)));
            _importTable.Columns.Add(new DataColumn("Primary", typeof(string)));
            _importTable.Columns.Add(new DataColumn("Notes", typeof(string)));
            _importTable.Columns.Add(new DataColumn("Location", typeof(string)));
            _importTable.Columns.Add(new DataColumn("Device", typeof(string)));
        }

        /// <summary>
        ///     Returns the row count of the ImportList table
        /// </summary>
        public int GetImportCount
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportList.GetImportCount(GET)");
                return _importTable.Rows.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowNo"></param>
        /// <returns></returns>
        public DataRow GetListRow(int rowNo)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportList.GetListRow(rowNo: " + rowNo + ")");
            return _importTable.Rows[rowNo];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        public void AddToList(DataRow row)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportList.AddToList(row: " + row + ")");
            try
            {
                string status = row["Status"].ToString();
                Int32 personID = Convert.ToInt32((string) row["PersonID"]);
                string surname = row["Surname"].ToString();
                string forename = row["Forename"].ToString();
                string title = null;
                string gender = null;
                string staffCode = null;
                string dob = null;
                string email = null;
                string telephone = null;
                string udf = null;
                string admissionNumber = null;
                string year = null;
                string registration = null;
                string house = null;
                string postCode = null;
                string town = null;
                string primary = null;
                string main = null;
                string notes = null;
                string location = null;
                string device = null;

                logger.Log(LogLevel.Trace, "AddToList: primary: " + primary);
                logger.Log(LogLevel.Trace, "AddToList: main: " + main);

                switch (Switcher.PreImportClass.GetUserType)
                {
                    case Interfaces.UserType.Contact:
                        town = row["Town"].ToString();
                        postCode = row["PostCode"].ToString();
                        break;
                    case Interfaces.UserType.Pupil:
                        admissionNumber = row["Admission Number"].ToString();
                        year = row["Year"].ToString();
                        registration = row["Registration"].ToString();
                        house = row["House"].ToString();
                        gender = row["Gender"].ToString();
                        dob = row["Date of Birth"].ToString();
                        break;
                    case Interfaces.UserType.Staff:
                        title = row["Title"].ToString();
                        gender = row["Gender"].ToString();
                        staffCode = row["Staff Code"].ToString();
                        dob = row["Date of Birth"].ToString();
                        break;
                }

                if (status.StartsWith("Import"))
                {
                    if (status.Contains("Email"))
                    {
                        logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportList.AddToList-Email");

                        email = (string) row["Import email"];
                        primary = (string)row["Primary"];
                        main = (string)row["Main"];
                        notes = null;
                        if (!row.IsNull("Notes"))
                        {
                            notes = (string)row["Notes"];
                        }
                        location = (string)row["Location"];
                        
                        bool isValidEmail = Validation.IsValidEmail(email);
                        if (isValidEmail)
                            AddToImportTable("Email", personID, email, surname, forename, title, gender, staffCode, dob,
                                admissionNumber, year, registration, house, postCode, town, main, primary, 
                                notes, location, null);
                        else
                        {
                            logger.Log(LogLevel.Info, "Invalid email: " + email);
                            Switcher.ResultsImportClass.AddToResultsTable(personID.ToString(), "Not Imported", "Email",
                                email, "Invalid email address", surname, forename, title, gender, staffCode, dob,
                                admissionNumber, year, registration, house, postCode, town, main, primary,
                                location, device);
                        }
                    }
                    if (status.Contains("Telephone"))
                    {
                        logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportList.AddToList-Telephone");

                        telephone = row["Import telephone"].ToString();
                        primary = row["Primary"].ToString();
                        main = row["Main"].ToString();
                        notes = null;
                        if (!row.IsNull("Notes"))
                        {
                            notes = (string)row["Notes"];
                        }
                        location = row["Location"].ToString();
                        device = row["Device"].ToString();

                        bool isValidTelephone = Validation.IsValidTelephone(telephone);
                        if (isValidTelephone)
                            AddToImportTable("Telephone", personID, telephone, surname, forename, title, gender,
                                staffCode, dob, admissionNumber, year, registration, house, postCode, town, main, 
                                primary, notes, location, device);
                        else
                        {
                            logger.Log(LogLevel.Info, "Invalid email: " + email);
                            Switcher.ResultsImportClass.AddToResultsTable(personID.ToString(), "Not Imported",
                                "Telephone", telephone, "Invalid telephone number", surname, forename, title, gender,
                                staffCode, dob, admissionNumber, year, registration, house, postCode, town, 
                                main, primary, location, device);
                        }
                    }
                    if (status.Contains("UDF"))
                    {
                        logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportList.AddToList-UDF");

                        udf = (string) row["Import UDF"];
                        AddToImportTable("UDF", personID, udf, surname, forename, title, gender, staffCode, dob,
                            admissionNumber, year, registration, house, postCode, town, null, null, null, null, null);
                    }
                }
                else
                {
                    Switcher.ResultsImportClass.AddToResultsTable(personID.ToString(), "Not Imported", null, null,
                        status, surname, forename, title, gender, staffCode, dob, admissionNumber, year, registration,
                        house, postCode, town, main, primary, location, device);
                }
            }
            catch (Exception AddToList_Exception)
            {
                logger.Log(LogLevel.Error, AddToList_Exception);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="personID"></param>
        /// <param name="value"></param>
        /// <param name="surname"></param>
        /// <param name="forename"></param>
        /// <param name="title"></param>
        /// <param name="gender"></param>
        /// <param name="staffCode"></param>
        /// <param name="dob"></param>
        /// <param name="admissionNumber"></param>
        /// <param name="year"></param>
        /// <param name="registration"></param>
        /// <param name="house"></param>
        /// <param name="postCode"></param>
        /// <param name="town"></param>
        /// <param name="primary"></param>
        /// <param name="main"></param>
        /// <param name="notes"></param>
        /// <param name="location"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        private bool AddToImportTable(string type, Int32 personID, string value, string surname, string forename,
            string title, string gender, string staffCode, string dob, string admissionNumber, string year,
            string registration, string house, string postCode, string town, string main, string primary,
            string notes, string location, string device)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.ImportList.AddToImportTable(type: " + type + ", personID: " + personID +
                ", value: " + value + ", surname: " + surname + ", forename: " + forename + ", title: " + title +
                ", gender: " + gender + ", staffCode: " + staffCode + ", dob: " + dob + ", admissionNumber: " +
                admissionNumber + ", year: " + year + ", registration: " + registration + ", house: " + house +
                ", postCode: " + postCode + ", town: " + town + ", main: " + main + ", primary: " + primary + 
                ", notes: " + notes + ", location: " + location + ", device: " + device +")");
            try
            {
                DataRow newrow = _importTable.NewRow();
                newrow["Type"] = type;
                newrow["PersonID"] = personID;
                newrow["Value"] = value;
                newrow["Surname"] = surname;
                newrow["Forename"] = forename;
                newrow["Title"] = title;
                newrow["Gender"] = gender;
                newrow["Staff Code"] = staffCode;
                newrow["Date of Birth"] = dob;
                newrow["Admission Number"] = admissionNumber;
                newrow["Year"] = year;
                newrow["Registration"] = registration;
                newrow["House"] = house;
                newrow["Postcode"] = postCode;
                newrow["Town"] = town;
                newrow["Main"] = main;
                newrow["Primary"] = primary;
                newrow["Notes"] = notes;
                newrow["Location"] = location;
                newrow["Device"] = device;
                _importTable.Rows.Add(newrow);
            }
            catch (Exception AddToImportTable_Exception)
            {
                logger.Log(LogLevel.Error, AddToImportTable_Exception);
                return false;
            }
            return true;
        }
    }
}