﻿/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    /// Interaction logic for Match.xaml
    /// </summary>
    public partial class Match : Window
    {
        private SIMSAPI simsApi;
        //private ImportFromFile importFromFile;

        private DataTable dt;
        //private int ImportType;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        //private string personid;
        private string firstname;
        private string surname;
        private string email;
        private string staffcode;
        private string gender;
        private string title;
        private string udf;
        private string simsUdf;
        private string emaillocation;
        private string reg;

        private void GetUserFilters()
        {
            switch (simsApi.GetUserType)
            {
                case SIMSAPI.UserType.Staff:
                    this.comboFilter.Items.Add("Staff, all Current");
                    this.comboFilter.Items.Add("Teaching staff, all Current");
                    this.comboFilter.Items.Add("Support Staff, all Current");
                    this.comboFilter.Items.Add("Staff, all Future");
                    this.comboFilter.Items.Add("Teaching staff, all Leavers");
                    this.comboFilter.Items.Add("Support Staff, all Leavers");
                    this.comboFilter.Items.Add("Staff, all Leavers");
                    this.comboFilter.Items.Add("All");

                    this.comboFilter.SelectedIndex = simsApi.GetUserFilter;
                    this.comboFilter.IsEnabled = true;
                    break;
                case SIMSAPI.UserType.Pupil:
                    this.comboFilter.Items.Add("<Any>");
                    this.comboFilter.Items.Add("Current");
                    this.comboFilter.Items.Add("Ever On Roll");
                    this.comboFilter.Items.Add("Guest");
                    this.comboFilter.Items.Add("Leavers");
                    this.comboFilter.Items.Add("On Roll");
                    this.comboFilter.Items.Add("Future");
                    this.comboFilter.SelectedIndex = simsApi.GetUserFilter;
                    this.comboFilter.IsEnabled = true;
                    break;
                case SIMSAPI.UserType.Contact:
                    // Contacts has no filters
                    break;
                case SIMSAPI.UserType.Unknown:
                    logger.Log(NLog.LogLevel.Error, "GetUserFilters: Unknown selected");
                    break;
            }
        }

        public Match(SIMSAPI simsapi)
        {
            simsApi = simsapi;
            //importFromFile = importFile;

            InitializeComponent();

            this.Title = "Match - " + GetExe.Title;
            
            GetUserFilters();

            switch (simsApi.GetUserType)
            {
                case SIMSAPI.UserType.Staff:
                    logger.Log(NLog.LogLevel.Debug, "Loading UDFs - Staff");
                    string[] udfsStaff = simsApi.GetStaffUDFs;
                    if (udfsStaff.Length != 0)
                    {
                        foreach (string udf in udfsStaff)
                        {
                            this.comboSIMSUDF.Items.Add(udf);
                        }
                        this.comboSIMSUDF.Items.Add("");
                        this.comboSIMSUDF.IsEnabled = true;
                    }
                    this.labelGender.IsEnabled = true;
                    this.comboGender.IsEnabled = true;
                    break;
                case SIMSAPI.UserType.Pupil:
                    logger.Log(NLog.LogLevel.Debug, "Loading UDFs - Students");
                    string[] udfsStudents = simsApi.GetStudentUDFs;
                    if (udfsStudents.Length != 0)
                    {
                        foreach (string udf in udfsStudents)
                        {
                            this.comboSIMSUDF.Items.Add(udf);
                        }
                        this.comboSIMSUDF.Items.Add("");
                        this.comboSIMSUDF.IsEnabled = true;
                    }
                    this.labelReg.IsEnabled = true;
                    this.comboReg.IsEnabled = true;
                    this.labelCode.Content = "Admission number";
                    this.labelTitle.Content = "Year Group";
                    break;
                case SIMSAPI.UserType.Contact:
                    logger.Log(NLog.LogLevel.Debug, "Loading UDFs - Contacts");
                    string[] udfsContacts = simsApi.GetContactUDFs;
                    if (udfsContacts.Length != 0)
                    {
                        foreach (string udf in udfsContacts)
                        {
                            this.comboSIMSUDF.Items.Add(udf);
                        }
                        this.comboSIMSUDF.Items.Add("");
                        this.comboSIMSUDF.IsEnabled = true;
                    }
                    this.labelCode.Content = "Postcode";
                    this.labelTitle.Content = "Town";

                    this.labelGender.Visibility = Visibility.Hidden;
                    this.comboGender.Visibility = Visibility.Hidden;

                    this.labelReg.Visibility = Visibility.Hidden;
                    this.comboReg.Visibility = Visibility.Hidden;
                    break;
                    case SIMSAPI.UserType.Unknown:
                    logger.Log(NLog.LogLevel.Error, "Match: Unknown selected");
                    break;
            }

            // Get Email Locations
            string[] emailLocations = simsApi.GetEmailLocations;
            if (emailLocations.Length != 0)
            {
                foreach (string emailLocation in emailLocations)
                {
                    this.comboEmailLocation.Items.Add(emailLocation);
                }
                this.comboEmailLocation.Items.Add("");
            }

            if (importFromFile.GetIsExcel)
            {
                foreach (string workBook in importFromFile.GetSheets)
                {
                    this.comboWorkbook.Items.Add(workBook);
                }
                this.comboWorkbook.IsEnabled = true;
            }
            else
            {
                GetDataTable();
            }
        }

        private void GetDataTable()
        {
            //personid = null;
            firstname = null;
            surname = null;
            email = null;
            staffcode = null;
            title = null;
            gender = null;
            udf = null;
            //simsUdf = null;

            if (this.comboWorkbook.IsEnabled)
            {
                DataSet dt1 = importFromFile.GetImportDataSet(this.comboWorkbook.SelectedIndex);
                if (dt1.Tables.Count > 0)
                {
                    dt = importFromFile.GetImportDataSet(this.comboWorkbook.SelectedIndex).Tables[0];
                }
            }
            else
            {
                try
                {
                    dt = importFromFile.GetImportDataSet().Tables[0];
                }
                catch (IndexOutOfRangeException importDSexception)
                {
                    logger.Log(NLog.LogLevel.Error, importDSexception);
                    //System.Windows.Forms.MessageBox.Show("File is open by another process or corrupt");
                    // TODO ABORT
                }
            }

            comboUDF.Items.Clear();
            comboEmail.Items.Clear();
            comboFirst.Items.Clear();
            comboSurname.Items.Clear();
            comboCode.Items.Clear();
            comboGender.Items.Clear();
            comboTitle.Items.Clear();
            //comboSIMSUDF.Items.Clear();
            comboReg.Items.Clear();

            try
            {
                foreach (DataColumn column in dt.Columns)
                {
                    //comboPerson.Items.Add(column.ColumnName);
                    comboEmail.Items.Add(column.ColumnName);
                    comboFirst.Items.Add(column.ColumnName);
                    comboSurname.Items.Add(column.ColumnName);
                    comboCode.Items.Add(column.ColumnName);
                    comboGender.Items.Add(column.ColumnName);
                    comboTitle.Items.Add(column.ColumnName);
                    comboUDF.Items.Add(column.ColumnName);
                    comboReg.Items.Add(column.ColumnName);
                }
            }
            catch (NullReferenceException)
            {
                
            }

            DataColumn blank = new DataColumn("");
            comboEmail.Items.Add(blank);
            comboFirst.Items.Add(blank);
            comboSurname.Items.Add(blank);
            comboCode.Items.Add(blank);
            comboTitle.Items.Add(blank);
            comboGender.Items.Add(blank);
            comboUDF.Items.Add(blank);
            comboReg.Items.Add(blank);
            //comboSIMSUDF.Items.Add(blank);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            
            if (matchFillIn)
            {
                simsApi.SetImportDataset = importFromFile.GetImportDataSet();

                simsApi.SetMatchFirstname = firstname;
                simsApi.SetMatchSurname = surname;
                simsApi.SetMatchEmail = email;
                simsApi.SetMatchStaffcode = staffcode;
                simsApi.SetMatchGender = gender;
                switch (simsApi.GetUserType)
                {
                    case SIMSAPI.UserType.Staff:
                        simsApi.SetMatchTitle = title;
                        break;
                    case SIMSAPI.UserType.Pupil:
                        simsApi.SetMatchYear = title;
                        break;
                    case SIMSAPI.UserType.Contact:
                        simsApi.SetMatchTown = title;
                        simsApi.SetMatchPostcode = staffcode;
                        break;
                }

                if (!string.IsNullOrEmpty(udf))
                {
                    simsApi.SetMatchUDF = udf;
                }
                if (!string.IsNullOrEmpty(simsUdf))
                {
                    simsApi.SetMatchSIMSUDF = simsUdf;
                }
                if (!string.IsNullOrEmpty(emaillocation))
                {
                    simsApi.SetMatchEmailLocation = emaillocation;
                }
                
                simsApi.SetMatchReg = reg;
                this.Close();
            }
        }

        private DataTable previewTable
        {
            get
            {
                DataTable tmpTable = new DataTable();
                //tmpTable.Columns.Add(new DataColumn("PersonID", typeof(string)));
                tmpTable.Columns.Add(new DataColumn("Surname", typeof(string)));
                tmpTable.Columns.Add(new DataColumn("Firstname", typeof(string)));
                tmpTable.Columns.Add(new DataColumn("Email", typeof(string)));
                tmpTable.Columns.Add(new DataColumn("UDF", typeof(string)));
                switch (simsApi.GetUserType)
                {
                    case SIMSAPI.UserType.Staff:
                        tmpTable.Columns.Add(new DataColumn("Title", typeof(string)));
                        tmpTable.Columns.Add(new DataColumn("StaffCode", typeof(string)));
                        tmpTable.Columns.Add(new DataColumn("Gender", typeof(string)));
                        break;
                    case SIMSAPI.UserType.Pupil:
                        tmpTable.Columns.Add(new DataColumn("Year", typeof(string)));
                        tmpTable.Columns.Add(new DataColumn("Reg", typeof(string)));
                        tmpTable.Columns.Add(new DataColumn("Admission", typeof(string)));
                        break;
                    case SIMSAPI.UserType.Contact:
                        tmpTable.Columns.Add(new DataColumn("Postcode", typeof(string)));
                        tmpTable.Columns.Add(new DataColumn("Town", typeof(string)));
                        break;
                }

                return tmpTable;
            }
        }

        private void combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataTable filtered  = previewTable;
            //if (comboPerson.SelectedItem != null)
            //{
            //    personid = comboPerson.SelectedItem.ToString();
            //}
            if (comboCode.SelectedItem != null)
            {
                staffcode = comboCode.SelectedItem.ToString();
            }
            if (comboFirst.SelectedItem != null)
            {
                firstname = comboFirst.SelectedItem.ToString();
            }
            if (comboSurname.SelectedItem != null)
            {
                surname = comboSurname.SelectedItem.ToString();
            }
            if (comboEmail.SelectedItem != null)
            {
                email = comboEmail.SelectedItem.ToString();
                string tempemail = comboEmail.SelectedValue.ToString();
                if (string.IsNullOrEmpty(tempemail))
                {
                    labelEmailLocation.IsEnabled = false;
                    comboEmailLocation.IsEnabled = false;
                    comboEmailLocation.SelectedValue = null;
                }
                else
                {
                    labelEmailLocation.IsEnabled = true;
                    comboEmailLocation.IsEnabled = true;
                }
            }

            if (comboGender.SelectedItem != null)
            {
                gender = comboGender.SelectedItem.ToString();
            }
            if (comboTitle.SelectedItem != null)
            {
                title = comboTitle.SelectedItem.ToString();
            }
            if (comboReg.SelectedItem != null)
            {
                reg = comboReg.SelectedItem.ToString();
            }
            if (comboUDF.SelectedItem != null)
            {
                udf = comboUDF.SelectedItem.ToString();
            }
            if (comboSIMSUDF.SelectedItem != null)
            {
                simsUdf = comboSIMSUDF.SelectedItem.ToString();
            }
            if (comboEmailLocation.SelectedItem != null)
            {
                emaillocation = comboEmailLocation.SelectedItem.ToString();
            }

            int count = 1;

            foreach (DataRow r in dt.Rows)
            {

                if (count < 6)
                {
                    DataRow newrow = filtered.NewRow();
                    //if (!string.IsNullOrWhiteSpace(personid))
                    //{
                    //    newrow["PersonID"] = r[personid];
                    //}
                    if (!string.IsNullOrWhiteSpace(firstname))
                    {
                        newrow["Firstname"] = r[firstname];
                    }
                    if (!string.IsNullOrWhiteSpace(surname))
                    {
                        newrow["Surname"] = r[surname];
                    }
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        newrow["Email"] = r[email];
                    }
                    if (!string.IsNullOrWhiteSpace(udf))
                    {
                        newrow["UDF"] = r[udf];
                    }
                    switch (simsApi.GetUserType)
                    {
                        case SIMSAPI.UserType.Staff:
                            if (!string.IsNullOrWhiteSpace(gender))
                            {
                                newrow["Gender"] = r[gender];
                            }
                            if (!string.IsNullOrWhiteSpace(staffcode))
                            {
                                newrow["StaffCode"] = r[staffcode];
                            }
                            if (!string.IsNullOrWhiteSpace(title))
                            {
                                newrow["Title"] = r[title];
                            }
                            break;
                        case SIMSAPI.UserType.Pupil:
                            if (!string.IsNullOrWhiteSpace(title))
                            {
                                newrow["Year"] = r[title];
                            }
                            if (!string.IsNullOrWhiteSpace(reg))
                            {
                                newrow["Reg"] = r[reg];
                            }
                            if (!string.IsNullOrWhiteSpace(staffcode))
                            {
                                newrow["Admission"] = r[staffcode];
                            }
                            break;
                        case SIMSAPI.UserType.Contact:
                            if (!string.IsNullOrWhiteSpace(title))
                            {
                                newrow["Town"] = r[title];
                            }
                            if (!string.IsNullOrWhiteSpace(staffcode))
                            {
                                newrow["Postcode"] = r[staffcode];
                            }
                            break;
                    }
                    filtered.Rows.Add(newrow);
                    count = count + 1;
                }
            }

            this.dataGrid.DataContext = filtered;
            this.dataGrid.Items.Refresh();
        }

        private void comboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            simsApi.SetUserFilter = this.comboFilter.SelectedIndex;
        }

        private void comboWorkbook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dt = null;
            GetDataTable();
        }

        private void comboSIMSUDF_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tempudf = comboSIMSUDF.SelectedValue.ToString();
            this.labelUDF.Content = tempudf;
            if (string.IsNullOrEmpty(tempudf))
            {
                this.labelUDF.Content = "UDF";
                this.labelUDF.IsEnabled = false;
                this.comboUDF.IsEnabled = false;
                this.comboUDF.SelectedValue = null;
            }
            else
            {
                this.comboUDF.IsEnabled = true;
                this.labelUDF.IsEnabled = true;
            }
        }

        private bool matchFillIn
        {
            get
            {
                if (comboEmail.SelectedItem == null && comboUDF.SelectedItem == null)
                {
                    MessageBox.Show("Please define the email or UDF");
                    return false;
                }
                if (comboEmail.SelectedItem != null && comboEmailLocation.SelectedItem == null)
                {
                    MessageBox.Show("Please define the email location");
                    return false;
                }
                return true;
            }
        }
    }
}
