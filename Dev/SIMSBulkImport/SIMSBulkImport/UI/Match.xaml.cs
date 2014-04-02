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

        private bool emailSelected;
        private bool telephoneSelected;

        /// <summary>
        /// Variables of column matches to SIMS fields
        /// </summary>
        #region Variables
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
        #endregion

        /// <summary>
        /// 
        /// </summary>
        internal Match()
        {
            InitializeComponent();

            // Set the subtitle
            SetSubTitle();

            // Get User filters
            GetUserFilters();

            // Get UDFs
            //GetUdfs();

            // Enable area (staff\student\contact) specific options
            switch (Switcher.PreImportClass.GetUserType)
            {
                case Interfaces.UserType.Staff:
                    labelGender.IsEnabled = true;
                    comboGender.IsEnabled = true;
                    break;
                case Interfaces.UserType.Pupil:
                    labelReg.IsEnabled = true;
                    comboReg.IsEnabled = true;
                    labelCode.Content = "Admission number";
                    labelTitle.Content = "Year Group";
                    break;
                case Interfaces.UserType.Contact:
                    labelCode.Content = "Postcode";
                    labelTitle.Content = "Town";
                    labelGender.Visibility = Visibility.Hidden;
                    comboGender.Visibility = Visibility.Hidden;
                    labelReg.Visibility = Visibility.Hidden;
                    comboReg.Visibility = Visibility.Hidden;
                    break;
                case Interfaces.UserType.Unknown:
                    break;
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

        /// <summary>
        /// Creates the preview DataTable
        /// </summary>
        private DataTable previewTable
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Match.previewTable(GET)");
                logger.Log(LogLevel.Trace, emailSelected);
                logger.Log(LogLevel.Trace, telephoneSelected);
                var tmpTable = new DataTable();
                tmpTable.Columns.Add(new DataColumn("PersonID", typeof(string)));
                tmpTable.Columns.Add(new DataColumn("Surname", typeof (string)));
                tmpTable.Columns.Add(new DataColumn("Firstname", typeof (string)));

                if (emailSelected)
                {
                    // Email preview columns
                    tmpTable.Columns.Add(new DataColumn("Email", typeof(string)));
                    tmpTable.Columns.Add(new DataColumn("Email main", typeof(string)));
                    tmpTable.Columns.Add(new DataColumn("Email primary", typeof(string)));
                    tmpTable.Columns.Add(new DataColumn("Email notes", typeof(string)));
                }

                if (telephoneSelected)
                {
                    // Telephone preview columns
                    tmpTable.Columns.Add(new DataColumn("Telephone", typeof(string)));
                    tmpTable.Columns.Add(new DataColumn("Telephone main", typeof(string)));
                    tmpTable.Columns.Add(new DataColumn("Telephone primary", typeof(string)));
                    tmpTable.Columns.Add(new DataColumn("Telephone notes", typeof(string)));
                }

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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        private void GetUserFilters()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Match.GetUserFilters()");
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

        /// <summary>
        /// 
        /// </summary>
        private void GetDataTable()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Match.GetDataTable()");
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

            // Clear ComboBoxes
            comboUDF.Items.Clear();
            comboFirst.Items.Clear();
            comboSurname.Items.Clear();
            comboCode.Items.Clear();
            comboGender.Items.Clear();
            comboTitle.Items.Clear();
            //comboSIMSUDF.Items.Clear();
            comboReg.Items.Clear();
            comboEmail.Items.Clear();
            comboEmailMain.Items.Clear();
            comboEmailPrimary.Items.Clear();
            comboEmailNotes.Items.Clear();
            comboEmailLocation.Items.Clear();
            comboTelephone.Items.Clear();
            comboTelephoneMain.Items.Clear();
            comboTelephonePrimary.Items.Clear();
            comboTelephoneNotes.Items.Clear();
            comboTelephoneLocation.Items.Clear();
            comboTelephoneDevice.Items.Clear();

            // Add default 
            comboEmailMain.Items.Add("<Default>");
            comboEmailPrimary.Items.Add("<Default>");
            comboEmailLocation.Items.Add("<Default>");
            comboTelephoneMain.Items.Add("<Default>");
            comboTelephonePrimary.Items.Add("<Default>");
            comboTelephoneLocation.Items.Add("<Default>");
            comboTelephoneDevice.Items.Add("<Default>");

            try
            {
                foreach (DataColumn column in dt.Columns)
                {
                    comboPersonID.Items.Add(column.ColumnName);
                    comboFirst.Items.Add(column.ColumnName);
                    comboSurname.Items.Add(column.ColumnName);
                    comboCode.Items.Add(column.ColumnName);
                    comboGender.Items.Add(column.ColumnName);
                    comboTitle.Items.Add(column.ColumnName);
                    comboUDF.Items.Add(column.ColumnName);
                    comboReg.Items.Add(column.ColumnName);

                    comboEmail.Items.Add(column.ColumnName);
                    comboEmailMain.Items.Add(column.ColumnName);
                    comboEmailPrimary.Items.Add(column.ColumnName);
                    comboEmailNotes.Items.Add(column.ColumnName);
                    comboEmailLocation.Items.Add(column.ColumnName);

                    comboTelephone.Items.Add(column.ColumnName);
                    comboTelephoneMain.Items.Add(column.ColumnName);
                    comboTelephonePrimary.Items.Add(column.ColumnName);
                    comboTelephoneNotes.Items.Add(column.ColumnName);
                    comboTelephoneLocation.Items.Add(column.ColumnName);
                    comboTelephoneDevice.Items.Add(column.ColumnName);
                }
            }
            catch (NullReferenceException)
            {
            }

            var blank = new DataColumn("");
            comboPersonID.Items.Add(blank);
            comboFirst.Items.Add(blank);
            comboSurname.Items.Add(blank);
            comboCode.Items.Add(blank);
            comboTitle.Items.Add(blank);
            comboGender.Items.Add(blank);
            comboUDF.Items.Add(blank);
            comboReg.Items.Add(blank);
            //comboSIMSUDF.Items.Add(blank);
            comboEmail.Items.Add(blank);
            comboEmailNotes.Items.Add(blank);
            comboTelephone.Items.Add(blank);
            comboTelephoneNotes.Items.Add(blank);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, RoutedEventArgs e)
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

                if (!string.IsNullOrEmpty(personid))
                {
                    Switcher.PreImportClass.SetMatchPersonID = personid;
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
                    Switcher.PreImportClass.SetMatchEmailPrimary = emailPrimary;
                }
                if (!string.IsNullOrEmpty(emailMain))
                {
                    Switcher.PreImportClass.SetMatchEmailMain = emailMain;
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
                    Switcher.PreImportClass.SetMatchTelephonePrimary = telephonePrimary;
                }
                if (!string.IsNullOrEmpty(telephoneMain))
                {
                    Switcher.PreImportClass.SetMatchTelephoneMain = telephoneMain;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDefault_Click(object sender, RoutedEventArgs e)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Match.buttonDefault_Click()");
            
            // Switch to Default UI
            Switcher.Switch(new Default());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Match.buttonBack_Click()");

            // Switch to Open UI
            Switcher.Switch(new Open());
        }

        /// <summary>
        /// 
        /// </summary>
        private void setPreImport()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Match.setPreImport()");
            Switcher.PreImportClass.SetMatchIgnoreFirstRow = (bool) comboIgnoreFirst.IsChecked;
            Switcher.PreImportClass.SetImportDataset = dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Match.combo_SelectionChanged()");
            UpdatePreview();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdatePreview()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Match.UpdatePreview()");

            DataTable filtered = previewTable;

            // Read the ComboBoxs and set local variables
            SetMatchBinding();

            var ignoreFirstRow = (bool)comboIgnoreFirst.IsChecked;

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
                    filtered.Rows.Add((previewRow(dt.Rows[i - 1], filtered)));
                }
            }

            dataGrid.DataContext = filtered;
            dataGrid.Items.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetMatchBinding()
        {
            if (comboPersonID.SelectedItem != null)
            {
                personid = comboPersonID.SelectedItem.ToString();
            }
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
                emailSelected = !string.IsNullOrEmpty(tempemail);
                if (emailSelected)
                {
                    // Email column is set, enable other options

                    // Enable Email location combobox and set the default value
                    labelEmailLocation.IsEnabled = true;
                    comboEmailLocation.IsEnabled = true;
                    if (comboEmailLocation.SelectedIndex == -1)
                        comboEmailLocation.SelectedIndex = 1;

                    // Enable Email main combobox and set the default value
                    labelEmailMain.IsEnabled = true;
                    comboEmailMain.IsEnabled = true;
                    if (comboEmailMain.SelectedIndex == -1)
                        comboEmailMain.SelectedIndex = 0;

                    // Enable Email primary combobox and set the default value
                    labelEmailPrimary.IsEnabled = true;
                    comboEmailPrimary.IsEnabled = true;
                    if (comboEmailPrimary.SelectedIndex == -1)
                        comboEmailPrimary.SelectedIndex = 0;

                    // Enable Email notes combobox and set the default value
                    labelEmailNotes.IsEnabled = true;
                    comboEmailNotes.IsEnabled = true;
                }
                else
                {
                    // Email column NOT set, disable other options

                    // Disable Email location combobox and reset value
                    labelEmailLocation.IsEnabled = false;
                    comboEmailLocation.IsEnabled = false;
                    comboEmailLocation.SelectedValue = null;

                    // Disable Email Main combobox and reset value
                    labelEmailMain.IsEnabled = false;
                    comboEmailMain.IsEnabled = false;
                    comboEmailMain.SelectedValue = null;

                    // Disable Email Primary combobox and reset value
                    labelEmailPrimary.IsEnabled = false;
                    comboEmailPrimary.IsEnabled = false;
                    comboEmailPrimary.SelectedValue = null;

                    // Disable Email Notes combobox and reset value
                    labelEmailNotes.IsEnabled = false;
                    comboEmailNotes.IsEnabled = false;
                    comboEmailNotes.SelectedValue = null;
                }
            }
            if (comboTelephone.SelectedItem != null)
            {
                telephone = comboTelephone.SelectedItem.ToString();
                string temptelephone = comboTelephone.SelectedValue.ToString();
                telephoneSelected = !string.IsNullOrEmpty(temptelephone);
                if (telephoneSelected)
                {
                    // Telephone column is set, enable other options

                    // Enable Telephone location combobox and set the default value
                    labelTelephoneLocation.IsEnabled = true;
                    comboTelephoneLocation.IsEnabled = true;
                    if (comboTelephoneLocation.SelectedIndex == -1)
                        comboTelephoneLocation.SelectedIndex = 1;

                    // Enable Telephone main combobox and set the default value
                    labelTelephoneMain.IsEnabled = true;
                    comboTelephoneMain.IsEnabled = true;
                    if (comboTelephoneMain.SelectedIndex == -1)
                        comboTelephoneMain.SelectedIndex = 0;

                    // Enable Telephone primary combobox and set the default value
                    labelTelephonePrimary.IsEnabled = true;
                    comboTelephonePrimary.IsEnabled = true;
                    if (comboTelephonePrimary.SelectedIndex == -1)
                        comboTelephonePrimary.SelectedIndex = 0;

                    // Enable Telephone notes combobox and set the default value
                    labelTelephoneNotes.IsEnabled = true;
                    comboTelephoneNotes.IsEnabled = true;
                }
                else
                {
                    // Telephone column NOT set, disable other options

                    // Disable Telephone location combobox and reset value
                    labelTelephoneLocation.IsEnabled = false;
                    comboTelephoneLocation.IsEnabled = false;
                    comboTelephoneLocation.SelectedValue = null;

                    // Disable Telephone main combobox and reset value
                    labelTelephoneMain.IsEnabled = false;
                    comboTelephoneMain.IsEnabled = false;
                    comboTelephoneMain.SelectedValue = null;

                    // Disable Telephone primary combobox and reset value
                    labelTelephonePrimary.IsEnabled = false;
                    comboTelephonePrimary.IsEnabled = false;
                    comboTelephonePrimary.SelectedValue = null;

                    // Disable Telephone notes combobox and reset value
                    labelTelephoneNotes.IsEnabled = false;
                    comboTelephoneNotes.IsEnabled = false;
                    comboTelephoneNotes.SelectedValue = null;
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataRow previewRow(DataRow r, DataTable dt)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Match.previewRow()");
            DataRow newrow = dt.NewRow();
            if (!string.IsNullOrWhiteSpace(personid))
            {
                newrow["PersonID"] = r[personid];
            }
            if (!string.IsNullOrWhiteSpace(firstname))
            {
                newrow["Firstname"] = r[firstname];
            }
            if (!string.IsNullOrWhiteSpace(surname))
            {
                newrow["Surname"] = r[surname];
            }
            // If email selected then add them to the preview DataTable
            if (emailSelected)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        newrow["Email"] = r[email];
                    }
                    if (!string.IsNullOrWhiteSpace(emailMain))
                    {
                        if ((emailMain) != "<Default>")
                            newrow["Email main"] = r[emailMain];
                    }
                    if (!string.IsNullOrWhiteSpace(emailPrimary))
                    {
                        if ((emailPrimary) != "<Default>")
                            newrow["Email primary"] = r[emailPrimary];
                    }
                    if (!string.IsNullOrWhiteSpace(emailNotes))
                    {
                        newrow["Email notes"] = r[emailNotes];
                    }
                }
                catch (Exception previewRow_Email_Exception)
                {
                    logger.Log(LogLevel.Error, "ERROR:: Matt40k.SIMSBulkImport.Match.previewRow_Email_Exception()");
                    logger.Log(LogLevel.Error, previewRow_Email_Exception);
                }
            }
            // If telephone selected then add them to the preview DataTable
            if (telephoneSelected)
            {
                try
                { 
                if (!string.IsNullOrWhiteSpace(telephone))
                {
                    newrow["Telephone"] = r[telephone];
                }
                if (!string.IsNullOrWhiteSpace(telephoneMain))
                {
                    if ((telephoneMain) != "<Default>")
                        newrow["Telephone main"] = r[telephoneMain];
                }
                if (!string.IsNullOrWhiteSpace(telephonePrimary))
                {
                    if ((telephonePrimary) != "<Default>")
                        newrow["Telephone primary"] = r[telephonePrimary];
                }
                if (!string.IsNullOrWhiteSpace(telephoneNotes))
                {
                    newrow["Telephone notes"] = r[telephoneNotes];
                }
                }
                catch (Exception previewRow_Telephone_Exception)
                {
                    logger.Log(LogLevel.Error, "ERROR:: Matt40k.SIMSBulkImport.Match.previewRow_Telephone_Exception()");
                    logger.Log(LogLevel.Error, previewRow_Telephone_Exception);
                }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Switcher.PreImportClass.SetUserFilter = this.comboFilter.SelectedIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboWorkbook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dt = null;
            GetDataTable();
        }

        /// <summary>
        /// Sets the user-defined SIMS UDF Type 
        /// Default: Single line of text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboUDFType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Match.comboUDFType_SelectionChanged()");
            string udfType = comboSIMSUDFType.SelectionBoxItem.ToString();
            if (!string.IsNullOrWhiteSpace(udfType))
            {
                Switcher.PreImportClass.SetUDFType = udfType;
                GetUdfs();
            }
        }

        /// <summary>
        /// Sets the label for the user-defined UDF to the name of the UDF defined
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboIgnoreFirst_Checked(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetUdfs()
        {
            switch (Switcher.PreImportClass.GetUserType)
            {
                // Load Staff UDFs
                case Interfaces.UserType.Staff:
                    logger.Log(LogLevel.Debug, "Loading UDFs - Staff");
                    
                    // Clear SIMS UDFs
                    comboSIMSUDF.Items.Clear();
                    
                    foreach (DataRow udf in Switcher.SimsApiClass.GetStaffUDFs.Rows)
                    {
                        string udfType = udf["Type"].ToString();
                        string udfValue = udf["Name"].ToString();
                        logger.Log(LogLevel.Trace, "UDFs:: " + udfType + " - " + udfValue);
                        if (udfType == Switcher.PreImportClass.GetUDFType)
                            comboSIMSUDF.Items.Add(udfValue);
                    }
                    
                    comboSIMSUDF.Items.Add("");
                    comboSIMSUDF.IsEnabled = true;
                    break;
                // Load Students UDFs
                case Interfaces.UserType.Pupil:
                    //logger.Log(LogLevel.Debug, "Loading UDFs - Students");

                    // Clear SIMS UDFs
                    comboSIMSUDF.Items.Clear();

                    DataTable udfsStudents = Switcher.SimsApiClass.GetPupilUDFs;
                    foreach (DataRow udf in udfsStudents.Rows)
                    {
                        string udfType = udf["type"].ToString();
                        string udfValue = udf["Name"].ToString();
                        logger.Log(LogLevel.Trace, "UDFs:: " + udfValue);
                        if (udfType == Switcher.PreImportClass.GetUDFType)
                            comboSIMSUDF.Items.Add(udfValue);
                    }
                    comboSIMSUDF.Items.Add("");
                    comboSIMSUDF.IsEnabled = true;
                    break;
                // Load Contact UDFs
                case Interfaces.UserType.Contact:
                    logger.Log(LogLevel.Debug, "Loading UDFs - Contacts");

                    // Clear SIMS UDFs
                    comboSIMSUDF.Items.Clear();

                    DataTable udfsContacts = Switcher.SimsApiClass.GetContactUDFs;
                    foreach (DataRow udf in udfsContacts.Rows)
                    {
                        string udfType = udf["type"].ToString();
                        string udfValue = udf["Name"].ToString();
                        logger.Log(LogLevel.Trace, "UDFs:: " + udfValue);
                        if (udfType == Switcher.PreImportClass.GetUDFType)
                            comboSIMSUDF.Items.Add(udfValue);
                    }
                    comboSIMSUDF.Items.Add("");
                    comboSIMSUDF.IsEnabled = true;
                    break;
                case Interfaces.UserType.Unknown:
                    logger.Log(LogLevel.Error, "Match: Unknown selected");
                    break;
            }
        }

        /// <summary>
        /// Set the UI subtitle
        /// </summary>
        private void SetSubTitle()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Options.SetTitle()");
            switch (Switcher.PreImportClass.GetUserType)
            {
                case Interfaces.UserType.Staff:
                    labelSubTitle.Content = "Staff";
                    break;
                case Interfaces.UserType.Pupil:
                    labelSubTitle.Content = "Pupil";
                    break;
                case Interfaces.UserType.Contact:
                    labelSubTitle.Content = "Contact";
                    break;
            }
        }
    }
}