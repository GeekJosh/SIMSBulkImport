﻿/*
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

        public ImportList()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportList()");
            Switcher.ResultsImportClass = new ResultsImport();
            _importTable = new DataTable();
            _importTable.Columns.Add(new DataColumn("Type", typeof (string)));
            _importTable.Columns.Add(new DataColumn("PersonID", typeof (Int32)));
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

        public DataRow GetListRow(int rowNo)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportList.GetListRow(rowNo: " + rowNo + ")");
            return _importTable.Rows[rowNo];
        }

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
                        bool isValidEmail = Validation.IsValidEmail(email);
                        if (isValidEmail)
                            AddToImportTable("Email", personID, email, surname, forename, title, gender, staffCode, dob,
                                admissionNumber, year, registration, house, postCode, town);
                        else
                        {
                            logger.Log(LogLevel.Info, "Invalid email: " + email);
                            Switcher.ResultsImportClass.AddToResultsTable(personID.ToString(), "Not Imported", "Email",
                                email, "Invalid email address", surname, forename, title, gender, staffCode, dob,
                                admissionNumber, year, registration, house, postCode, town);
                        }
                    }
                    if (status.Contains("Telephone"))
                    {
                        logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportList.AddToList-Telephone");

                        telephone = (string) row["Import telephone"];
                        bool isValidTelephone = Validation.IsValidTelephone(telephone);
                        if (isValidTelephone)
                            AddToImportTable("Telephone", personID, telephone, surname, forename, title, gender,
                                staffCode, dob, admissionNumber, year, registration, house, postCode, town);
                        else
                        {
                            logger.Log(LogLevel.Info, "Invalid email: " + email);
                            Switcher.ResultsImportClass.AddToResultsTable(personID.ToString(), "Not Imported",
                                "Telephone", telephone, "Invalid telephone number", surname, forename, title, gender,
                                staffCode, dob, admissionNumber, year, registration, house, postCode, town);
                        }
                    }
                    if (status.Contains("UDF"))
                    {
                        logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportList.AddToList-UDF");

                        udf = (string) row["Import UDF"];
                        AddToImportTable("UDF", personID, udf, surname, forename, title, gender, staffCode, dob,
                            admissionNumber, year, registration, house, postCode, town);
                    }
                }
                else
                {
                    Switcher.ResultsImportClass.AddToResultsTable(personID.ToString(), "Not Imported", null, null,
                        status, surname, forename, title, gender, staffCode, dob, admissionNumber, year, registration,
                        house, postCode, town);
                }
            }
            catch (Exception AddToList_Exception)
            {
                logger.Log(LogLevel.Error, AddToList_Exception);
            }
        }

        private bool AddToImportTable(string type, Int32 personID, string value, string surname, string forename,
            string title, string gender, string staffCode, string dob, string admissionNumber, string year,
            string registration,
            string house, string postCode, string town)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.ImportList.AddToImportTable(type: " + type + ", personID: " + personID +
                ", value: " + value + ", surname: " + surname + ", forename: " + forename + ", title: " + title +
                ", gender: " + gender + ", staffCode: " + staffCode + ", dob: " + dob + ", admissionNumber: " +
                admissionNumber + ", year: " + year + ", registration: " + registration + ", house: " + house +
                ", postCode: " + postCode + ", town: " + town + ")");
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