/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for Match.xaml
    /// </summary>
    public partial class Match
    {
        //private int ImportType;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly DataSet ds;
        private DataTable dt;

        private string personid;
        private string email;
        private string emailLocation;
        private string emailMain;
        private string emailPrimary;
        private string emailNotes;
        private string firstname;
        private string gender;
        private string reg;
        private string simsUdf;
        private string staffcode;
        private string surname;
        private string telephone;
        private string telephoneLocation;
        private string telephoneMain;
        private string telephonePrimary;
        private string telephoneNotes;
        private string title;
        private string udf;

        internal Match()
        {
            InitializeComponent();

            GetUserFilters();

            switch (Switcher.PreImportClass.GetUserType)
            {
                case Interfaces.UserType.Staff:
                    logger.Log(LogLevel.Debug, "Loading UDFs - Staff");
                    foreach (DataRow udf in Switcher.SimsApiClass.GetStaffUDFs.Rows)
                    {
                        string udfType = udf["Type"].ToString();
                        string udfValue = udf["Name"].ToString();
                        logger.Log(LogLevel.Trace, "UDFs:: " + udfType + " - " + udfValue);
                        if (udfType == "STRING1")
                            comboSIMSUDF.Items.Add(udfValue);
                    }
                    comboSIMSUDF.Items.Add("");
                    comboSIMSUDF.IsEnabled = true;
                    labelGender.IsEnabled = true;
                    comboGender.IsEnabled = true;
                    break;
                case Interfaces.UserType.Pupil:
                    logger.Log(LogLevel.Debug, "Loading UDFs - Students");
                    DataTable udfsStudents = Switcher.SimsApiClass.GetPupilUDFs;
                    foreach (DataRow udf in udfsStudents.Rows)
                    {
                        string udfType = udf["type"].ToString();
                        string udfValue = udf["Name"].ToString();
                        logger.Log(LogLevel.Trace, "UDFs:: " + udfValue);
                        comboSIMSUDF.Items.Add(udfValue);
                    }
                    comboSIMSUDF.Items.Add("");
                    comboSIMSUDF.IsEnabled = true;
                    labelReg.IsEnabled = true;
                    comboReg.IsEnabled = true;
                    labelCode.Content = "Admission number";
                    labelTitle.Content = "Year Group";
                    break;
                case Interfaces.UserType.Contact:
                    logger.Log(LogLevel.Debug, "Loading UDFs - Contacts");
                    DataTable udfsContacts = Switcher.SimsApiClass.GetContactUDFs;
                    foreach (DataRow udf in udfsContacts.Rows)
                    {
                        string udfType = udf["type"].ToString();
                        string udfValue = udf["Name"].ToString();
                        logger.Log(LogLevel.Trace, "UDFs:: " + udfValue);

                        comboSIMSUDF.Items.Add(udfValue);
                    }
                    comboSIMSUDF.Items.Add("");
                    comboSIMSUDF.IsEnabled = true;
                    labelCode.Content = "Postcode";
                    labelTitle.Content = "Town";

                    labelGender.Visibility = Visibility.Hidden;
                    comboGender.Visibility = Visibility.Hidden;

                    labelReg.Visibility = Visibility.Hidden;
                    comboReg.Visibility = Visibility.Hidden;
                    break;
                case Interfaces.UserType.Unknown:
                    logger.Log(LogLevel.Error, "Match: Unknown selected");
                    break;
            }

            // Get Email Locations
            string[] emailLocations = Switcher.SimsApiClass.GetEmailLocations;
            if (emailLocations.Length != 0)
            {
                foreach (string emailLocation in emailLocations)
                {
                    comboEmailLocation.Items.Add(emailLocation);
                }
                comboEmailLocation.Items.Add("");
            }

            // Get Telephone Locations
            string[] telephoneLocations = Switcher.SimsApiClass.GetTelephoneLocations;
            if (telephoneLocations.Length != 0)
            {
                foreach (string telephoneLocation in telephoneLocations)
                {
                    comboTelephoneLocation.Items.Add(telephoneLocation);
                }
                comboTelephoneLocation.Items.Add("");
            }

            ds = Switcher.ImportFileClass.GetDataSet;
            if (ds.Tables.Count > 1)
            {
                foreach (DataTable workBook in ds.Tables)
                {
                    comboWorkbook.Items.Add(workBook.TableName);
                }
                comboWorkbook.IsEnabled = true;
            }

            GetDataTable();
        }

        private DataTable previewTable
        {
            get
            {
                var tmpTable = new DataTable();
                tmpTable.Columns.Add(new DataColumn("PersonID", typeof(int)));
                tmpTable.Columns.Add(new DataColumn("Surname", typeof (string)));
                tmpTable.Columns.Add(new DataColumn("Firstname", typeof (string)));
                tmpTable.Columns.Add(new DataColumn("Email", typeof (string)));
                tmpTable.Columns.Add(new DataColumn("Email notes", typeof(string)));
                tmpTable.Columns.Add(new DataColumn("Telephone", typeof (string)));
                tmpTable.Columns.Add(new DataColumn("Telephone notes", typeof(string)));
                tmpTable.Columns.Add(new DataColumn("UDF", typeof (string)));
                switch (Switcher.PreImportClass.GetUserType)
                {
                    case Interfaces.UserType.Staff:
                        tmpTable.Columns.Add(new DataColumn("Title", typeof (string)));
                        tmpTable.Columns.Add(new DataColumn("StaffCode", typeof (string)));
                        tmpTable.Columns.Add(new DataColumn("Gender", typeof (string)));
                        break;
                    case Interfaces.UserType.Pupil:
                        tmpTable.Columns.Add(new DataColumn("Year", typeof (string)));
                        tmpTable.Columns.Add(new DataColumn("Reg", typeof (string)));
                        tmpTable.Columns.Add(new DataColumn("Admission", typeof (string)));
                        break;
                    case Interfaces.UserType.Contact:
                        tmpTable.Columns.Add(new DataColumn("Postcode", typeof (string)));
                        tmpTable.Columns.Add(new DataColumn("Town", typeof (string)));
                        break;
                }

                return tmpTable;
            }
        }

        private bool matchFillIn
        {
            get
            {
                if (!comboEmailLocation.IsEnabled && !comboUDF.IsEnabled && !comboTelephoneLocation.IsEnabled)
                {
                    MessageBox.Show("Please define the email, telephone or UDF");
                    return false;
                }

                return true;
            }
        }

        private void GetUserFilters()
        {
            switch (Switcher.PreImportClass.GetUserType)
            {
                case Interfaces.UserType.Staff:
                    comboFilter.Items.Add("Staff, all Current");
                    comboFilter.Items.Add("Teaching staff, all Current");
                    comboFilter.Items.Add("Support Staff, all Current");
                    comboFilter.Items.Add("Staff, all Future");
                    comboFilter.Items.Add("Teaching staff, all Leavers");
                    comboFilter.Items.Add("Support Staff, all Leavers");
                    comboFilter.Items.Add("Staff, all Leavers");
                    comboFilter.Items.Add("All");
                    comboFilter.SelectedIndex = 0;
                    comboFilter.IsEnabled = true;
                    break;
                case Interfaces.UserType.Pupil:

                    comboFilter.Items.Add("Current");
                    comboFilter.Items.Add("Ever On Roll");
                    comboFilter.Items.Add("Guest");
                    comboFilter.Items.Add("Leavers");
                    comboFilter.Items.Add("On Roll");
                    comboFilter.Items.Add("Future");
                    comboFilter.Items.Add("<Any>");
                    comboFilter.SelectedIndex = 0;
                    comboFilter.IsEnabled = true;
                    break;
                case Interfaces.UserType.Contact:
                    // Contacts has no filters
                    break;
                case Interfaces.UserType.Unknown:
                    logger.Log(LogLevel.Error, "GetUserFilters: Unknown selected");
                    break;
            }
        }

        private void GetDataTable()
        {
            bool dtSet = false;
            if (comboWorkbook.SelectedValue != null)
            {
                if (!string.IsNullOrWhiteSpace(comboWorkbook.SelectedValue.ToString()))
                {
                    dtSet = true;
                    dt = ds.Tables[comboWorkbook.SelectedValue.ToString()];
                }
            }
            if (!dtSet)
            {
                dt = ds.Tables[0];
            }

            personid = null;
            firstname = null;
            surname = null;
            email = null;
            staffcode = null;
            title = null;
            gender = null;
            udf = null;
            //simsUdf = null;
            telephone = null;

            comboUDF.Items.Clear();
            comboEmail.Items.Clear();
            comboFirst.Items.Clear();
            comboSurname.Items.Clear();
            comboCode.Items.Clear();
            comboGender.Items.Clear();
            comboTitle.Items.Clear();
            //comboSIMSUDF.Items.Clear();
            comboReg.Items.Clear();
            comboTelephone.Items.Clear();
            comboEmailNotes.Items.Clear();
            comboTelephoneNotes.Items.Clear();

            try
            {
                foreach (DataColumn column in dt.Columns)
                {
                    comboPersonID.Items.Add(column.ColumnName);
                    comboEmail.Items.Add(column.ColumnName);
                    comboFirst.Items.Add(column.ColumnName);
                    comboSurname.Items.Add(column.ColumnName);
                    comboCode.Items.Add(column.ColumnName);
                    comboGender.Items.Add(column.ColumnName);
                    comboTitle.Items.Add(column.ColumnName);
                    comboUDF.Items.Add(column.ColumnName);
                    comboReg.Items.Add(column.ColumnName);
                    comboTelephone.Items.Add(column.ColumnName);
                    comboEmailNotes.Items.Add(column.ColumnName);
                    comboTelephoneNotes.Items.Add(column.ColumnName);
                }
            }
            catch (NullReferenceException)
            {
            }

            var blank = new DataColumn("");
            comboPersonID.Items.Add(blank);
            comboEmail.Items.Add(blank);
            comboFirst.Items.Add(blank);
            comboSurname.Items.Add(blank);
            comboCode.Items.Add(blank);
            comboTitle.Items.Add(blank);
            comboGender.Items.Add(blank);
            comboUDF.Items.Add(blank);
            comboReg.Items.Add(blank);
            comboTelephone.Items.Add(blank);
            //comboSIMSUDF.Items.Add(blank);
            comboEmailNotes.Items.Add(blank);
            comboTelephoneNotes.Items.Add(blank);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (matchFillIn)
            {
                setPreImport();

                Switcher.PreImportClass.SetMatchFirstname = firstname;
                Switcher.PreImportClass.SetMatchSurname = surname;
                Switcher.PreImportClass.SetMatchEmail = email;
                Switcher.PreImportClass.SetMatchTelephone = telephone;
                Switcher.PreImportClass.SetMatchStaffcode = staffcode;
                Switcher.PreImportClass.SetMatchGender = gender;
                switch (Switcher.PreImportClass.GetUserType)
                {
                    case Interfaces.UserType.Staff:
                        Switcher.PreImportClass.SetMatchTitle = title;
                        break;
                    case Interfaces.UserType.Pupil:
                        Switcher.PreImportClass.SetMatchYear = title;
                        break;
                    case Interfaces.UserType.Contact:
                        Switcher.PreImportClass.SetMatchTown = title;
                        Switcher.PreImportClass.SetMatchPostcode = staffcode;
                        break;
                }

                if (!string.IsNullOrEmpty(udf))
                {
                    Switcher.PreImportClass.SetMatchUDF = udf;
                }
                if (!string.IsNullOrEmpty(simsUdf))
                {
                    Switcher.PreImportClass.SetMatchSIMSUDF = simsUdf;
                }
                if (!string.IsNullOrEmpty(emailLocation))
                {
                    Switcher.PreImportClass.SetMatchEmailLocation = emailLocation;
                }
                if (!string.IsNullOrEmpty(emailPrimary))
                {
                    Switcher.PreImportClass.SetMatchEmailPrimaryId = emailPrimary;
                }
                if (!string.IsNullOrEmpty(emailMain))
                {
                    Switcher.PreImportClass.SetMatchEmailMainId = emailMain;
                }
                if (!string.IsNullOrEmpty(emailNotes))
                {
                    Switcher.PreImportClass.SetMatchEmailNotes = emailNotes;
                }
                if (!string.IsNullOrEmpty(telephoneLocation))
                {
                    Switcher.PreImportClass.SetMatchTelephoneLocation = telephoneLocation;
                }
                if (!string.IsNullOrEmpty(telephonePrimary))
                {
                    Switcher.PreImportClass.SetMatchTelephonePrimaryId = telephonePrimary;
                }
                if (!string.IsNullOrEmpty(telephoneMain))
                {
                    Switcher.PreImportClass.SetMatchTelephoneMainId = telephoneMain;
                }
                if (!string.IsNullOrEmpty(telephoneNotes))
                {
                    Switcher.PreImportClass.SetMatchTelephoneNotes = telephoneNotes;
                }
                Switcher.PreImportClass.SetUserFilter = (string) comboFilter.SelectedValue;
                Switcher.PreImportClass.SetMatchReg = reg;
                Switcher.Switch(new ImportWindow());
            }
        }

        private void setPreImport()
        {
            Switcher.PreImportClass.SetMatchIgnoreFirstRow = (bool) comboIgnoreFirst.IsChecked;
            Switcher.PreImportClass.SetImportDataset = dt;
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

                    labelEmailMain.IsEnabled = false;
                    comboEmailMain.IsEnabled = false;
                    comboEmailMain.SelectedValue = null;

                    labelEmailPrimary.IsEnabled = false;
                    comboEmailPrimary.IsEnabled = false;
                    comboEmailPrimary.SelectedValue = null;

                    labelEmailNotes.IsEnabled = false;
                    comboEmailNotes.IsEnabled = false;
                    comboEmailNotes.SelectedValue = null;
                }
                else
                {
                    labelEmailLocation.IsEnabled = true;
                    comboEmailLocation.IsEnabled = true;
                    if (comboEmailLocation.SelectedIndex == -1)
                        comboEmailLocation.SelectedIndex = 1;

                    labelEmailMain.IsEnabled = true;
                    comboEmailMain.IsEnabled = true;
                    if (comboEmailMain.SelectedIndex == -1)
                        comboEmailMain.SelectedIndex = 1;

                    labelEmailPrimary.IsEnabled = true;
                    comboEmailPrimary.IsEnabled = true;
                    if (comboEmailPrimary.SelectedIndex == -1)
                        comboEmailPrimary.SelectedIndex = 1;

                    labelEmailNotes.IsEnabled = true;
                    comboEmailNotes.IsEnabled = true;
                }
            }
            if (comboTelephone.SelectedItem != null)
            {
                telephone = comboTelephone.SelectedItem.ToString();
                string temptelephone = comboTelephone.SelectedValue.ToString();
                if (string.IsNullOrEmpty(temptelephone))
                {
                    labelTelephoneLocation.IsEnabled = false;
                    comboTelephoneLocation.IsEnabled = false;
                    comboTelephoneLocation.SelectedValue = null;

                    labelTelephoneMain.IsEnabled = false;
                    comboTelephoneMain.IsEnabled = false;
                    comboTelephoneMain.SelectedValue = null;

                    labelTelephonePrimary.IsEnabled = false;
                    comboTelephonePrimary.IsEnabled = false;
                    comboTelephonePrimary.SelectedValue = null;

                    labelTelephoneNotes.IsEnabled = false;
                    comboTelephoneNotes.IsEnabled = false;
                    comboTelephoneNotes.SelectedValue = null;
                }
                else
                {
                    labelTelephoneLocation.IsEnabled = true;
                    comboTelephoneLocation.IsEnabled = true;
                    if (comboTelephoneLocation.SelectedIndex == -1)
                        comboTelephoneLocation.SelectedIndex = 1;

                    labelTelephoneMain.IsEnabled = true;
                    comboTelephoneMain.IsEnabled = true;
                    if (comboTelephoneMain.SelectedIndex == -1)
                        comboTelephoneMain.SelectedIndex = 1;

                    labelTelephonePrimary.IsEnabled = true;
                    comboTelephonePrimary.IsEnabled = true;
                    if (comboTelephonePrimary.SelectedIndex == -1)
                        comboTelephonePrimary.SelectedIndex = 1;

                    labelTelephoneNotes.IsEnabled = true;
                    comboTelephoneNotes.IsEnabled = true;
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
                emailLocation = comboEmailLocation.SelectedItem.ToString();
            }
            if (comboEmailPrimary.SelectedItem != null)
            {
                emailPrimary = comboEmailPrimary.SelectedValue.ToString();
            }
            if (comboEmailMain.SelectedItem != null)
            {
                emailMain = comboEmailMain.SelectedValue.ToString();
            }
            if (comboEmailNotes.SelectedItem != null)
            {
                emailNotes = comboEmailNotes.SelectedValue.ToString();
            }
            if (comboTelephone.SelectedItem != null)
            {
                telephone = comboTelephone.SelectedItem.ToString();
            }
            if (comboTelephoneLocation.SelectedItem != null)
            {
                telephoneLocation = comboTelephoneLocation.SelectedItem.ToString();
            }
            if (comboTelephonePrimary.SelectedItem != null)
            {
                telephonePrimary = comboTelephonePrimary.SelectedValue.ToString();
            }
            if (comboTelephoneMain.SelectedItem != null)
            {
                telephoneMain = comboTelephoneMain.SelectedValue.ToString();
            }
            if (comboTelephoneNotes.SelectedItem != null)
            {
                telephoneNotes = comboTelephoneNotes.SelectedValue.ToString();
            }

            var ignoreFirstRow = (bool) comboIgnoreFirst.IsChecked;

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

            dataGrid.DataContext = filtered;
            dataGrid.Items.Refresh();
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
            if (!string.IsNullOrWhiteSpace(telephone))
            {
                newrow["Telephone"] = r[telephone];
            }
            if (!string.IsNullOrWhiteSpace(emailNotes))
            {
                newrow["Email notes"] = r[emailNotes];
            }
            if (!string.IsNullOrWhiteSpace(telephoneNotes))
            {
                newrow["Telephone notes"] = r[telephoneNotes];
            }
            if (!string.IsNullOrWhiteSpace(udf))
            {
                newrow["UDF"] = r[udf];
            }
            switch (Switcher.PreImportClass.GetUserType)
            {
                case Interfaces.UserType.Staff:
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
                case Interfaces.UserType.Pupil:
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
                case Interfaces.UserType.Contact:
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
            // Switcher.PreImportClass.SetUserFilter = this.comboFilter.SelectedIndex;
        }

        private void comboWorkbook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dt = null;
            GetDataTable();
        }

        private void comboSIMSUDF_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tempudf = comboSIMSUDF.SelectedValue.ToString();
            labelUDF.Content = tempudf;
            if (string.IsNullOrEmpty(tempudf))
            {
                labelUDF.Content = "UDF";
                labelUDF.IsEnabled = false;
                comboUDF.IsEnabled = false;
                comboUDF.SelectedValue = null;
            }
            else
            {
                comboUDF.IsEnabled = true;
                labelUDF.IsEnabled = true;
            }
        }

        private void comboIgnoreFirst_Checked(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
        }
    }
}