/*
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
    public partial class Match
    {
        private DataTable dt;
        private DataSet ds;
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

        internal Match()
        {
            InitializeComponent();

            GetUserFilters();

            switch (Switcher.SimsApiClass.GetUserType)
            {
                case SIMSAPI.UserType.Staff:
                    logger.Log(NLog.LogLevel.Debug, "Loading UDFs - Staff");
                    string[] udfsStaff = Switcher.SimsApiClass.GetStaffUDFs;
                    if (udfsStaff != null)
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
                    string[] udfsStudents = Switcher.SimsApiClass.GetPupilUDFs;
                    if (udfsStudents != null)
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
                    string[] udfsContacts = Switcher.SimsApiClass.GetContactUDFs;
                    if (udfsContacts != null)
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
            string[] emailLocations = Switcher.SimsApiClass.GetEmailLocations;
            if (emailLocations.Length != 0)
            {
                foreach (string emailLocation in emailLocations)
                {
                    this.comboEmailLocation.Items.Add(emailLocation);
                }
                this.comboEmailLocation.Items.Add("");
            }

            ds = Switcher.ImportFileClass.GetDataSet;
            if (ds.Tables.Count > 1)
            {
                foreach (DataTable workBook in ds.Tables)
                {
                    this.comboWorkbook.Items.Add(workBook.TableName);
                }
                this.comboWorkbook.IsEnabled = true;
            }

            GetDataTable();
        }

        private void GetUserFilters()
        {
            switch (Switcher.SimsApiClass.GetUserType)
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

                    this.comboFilter.SelectedIndex = Switcher.SimsApiClass.GetUserFilter;
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
                    this.comboFilter.SelectedIndex = Switcher.SimsApiClass.GetUserFilter;
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

        private void GetDataTable()
        {
            if (!string.IsNullOrWhiteSpace(this.comboWorkbook.SelectedValue.ToString()))
            {
                dt = ds.Tables[this.comboWorkbook.SelectedValue.ToString()];
            }
            else
            {
                dt = ds.Tables[0];
            }

            //personid = null;
            firstname = null;
            surname = null;
            email = null;
            staffcode = null;
            title = null;
            gender = null;
            udf = null;
            //simsUdf = null;

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
                Switcher.SimsApiClass.SetMatchIgnoreFirstRow = (bool)comboIgnoreFirst.IsChecked;
                Switcher.SimsApiClass.SetImportDataset = Switcher.ImportFileClass.GetDataSet;

                Switcher.SimsApiClass.SetMatchFirstname = firstname;
                Switcher.SimsApiClass.SetMatchSurname = surname;
                Switcher.SimsApiClass.SetMatchEmail = email;
                Switcher.SimsApiClass.SetMatchStaffcode = staffcode;
                Switcher.SimsApiClass.SetMatchGender = gender;
                switch (Switcher.SimsApiClass.GetUserType)
                {
                    case SIMSAPI.UserType.Staff:
                        Switcher.SimsApiClass.SetMatchTitle = title;
                        break;
                    case SIMSAPI.UserType.Pupil:
                        Switcher.SimsApiClass.SetMatchYear = title;
                        break;
                    case SIMSAPI.UserType.Contact:
                        Switcher.SimsApiClass.SetMatchTown = title;
                        Switcher.SimsApiClass.SetMatchPostcode = staffcode;
                        break;
                }

                if (!string.IsNullOrEmpty(udf))
                {
                    Switcher.SimsApiClass.SetMatchUDF = udf;
                }
                if (!string.IsNullOrEmpty(simsUdf))
                {
                    Switcher.SimsApiClass.SetMatchSIMSUDF = simsUdf;
                }
                if (!string.IsNullOrEmpty(emaillocation))
                {
                    Switcher.SimsApiClass.SetMatchEmailLocation = emaillocation;
                }

                Switcher.SimsApiClass.SetMatchReg = reg;
                Switcher.Switch(new ImportWindow());
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
                switch (Switcher.SimsApiClass.GetUserType)
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
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            DataTable filtered = previewTable;
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


            bool ignoreFirstRow = (bool)comboIgnoreFirst.IsChecked;

            int rowCount = 5;
            if (dt.Rows.Count < rowCount)
                rowCount = dt.Rows.Count;

            for (int i = 1; i <= rowCount; i++)
            {
                if (ignoreFirstRow && i == 1)
                {
                    // Ignore it
                }
                else
                {
                    filtered.Rows.Add(previewRow(dt.Rows[i - 1], filtered));
                }
            }

            this.dataGrid.DataContext = filtered;
            this.dataGrid.Items.Refresh();
        }

        private DataRow previewRow(DataRow r, DataTable dt)
        {
            DataRow newrow = dt.NewRow();
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
            switch (Switcher.SimsApiClass.GetUserType)
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
            return newrow;
        }

        private void comboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Switcher.SimsApiClass.SetUserFilter = this.comboFilter.SelectedIndex;
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

        private void comboIgnoreFirst_Checked(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
        }
    }
}
