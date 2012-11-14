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

namespace GingerBeard
{
    /// <summary>
    /// Interaction logic for Match.xaml
    /// </summary>
    public partial class Match : Window
    {
        SIMSAPI simsApi;
        private DataTable dt;

        //private string personid;
        private string firstname;
        private string surname;
        private string email;
        private string staffcode;
        private string gender;
        private string title;
        private string udf;
        private string simsUdf;

        private void GetUserFilters(int importType)
        {
            switch (importType)
            {
                case 1:
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
                case 2:
                    break;
                case 3:
                    // Contacts has no filters
                    break;
            }
        }

        public Match(SIMSAPI simsapi, int importType)
        {
            simsApi = simsapi;

            InitializeComponent();

            this.Title = "Match :: " + GetName.Title;
            
            GetUserFilters(importType);

            string[] udfs = simsApi.GetPersonUDFs;
            if (udfs.Length != 0)
            {
                foreach (string udf in udfs)
                {
                    this.comboSIMSUDF.Items.Add(udf);
                }
                this.comboSIMSUDF.Items.Add("");
                this.comboSIMSUDF.IsEnabled = true;
            }

            if (simsApi.GetIsExcel)
            {
                foreach (string workBook in simsApi.GetSheets)
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
                DataSet dt1 = simsApi.GetImportDataSet(this.comboWorkbook.SelectedIndex);
                if (dt1.Tables.Count > 0)
                {
                    dt = simsApi.GetImportDataSet(this.comboWorkbook.SelectedIndex).Tables[0];
                }
            }
            else
            {
                dt = simsApi.GetImportDataSet().Tables[0];
            }

            comboUDF.Items.Clear();
            comboEmail.Items.Clear();
            comboFirst.Items.Clear();
            comboSurname.Items.Clear();
            comboCode.Items.Clear();
            comboGender.Items.Clear();
            comboTitle.Items.Clear();
            //comboSIMSUDF.Items.Clear();

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
            //comboSIMSUDF.Items.Add(blank);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (comboEmail.SelectedItem == null && comboUDF.SelectedItem == null)
            {
                MessageBox.Show("Please define the email or UDF");
            }
            else
            {
                simsApi.SetMatchFirstname = firstname;
                simsApi.SetMatchSurname = surname;
                simsApi.SetMatchEmail = email;
                simsApi.SetMatchStaffcode = staffcode;
                simsApi.SetMatchGender = gender;
                simsApi.SetMatchTitle = title;
                simsApi.SetMatchUDF = udf;
                simsApi.SetMatchSIMSUDF = simsUdf;
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
                tmpTable.Columns.Add(new DataColumn("Gender", typeof(string)));
                tmpTable.Columns.Add(new DataColumn("Title", typeof(string)));
                tmpTable.Columns.Add(new DataColumn("StaffCode", typeof(string)));
                tmpTable.Columns.Add(new DataColumn("Email", typeof(string)));
                tmpTable.Columns.Add(new DataColumn("UDF", typeof(string)));
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
            }
            if (comboGender.SelectedItem != null)
            {
                gender = comboGender.SelectedItem.ToString();
            }
            if (comboTitle.SelectedItem != null)
            {
                title = comboTitle.SelectedItem.ToString();
            }
            if (comboUDF.SelectedItem != null)
            {
                udf = comboUDF.SelectedItem.ToString();
            }
            if (comboSIMSUDF.SelectedItem != null)
            {
                simsUdf = comboSIMSUDF.SelectedItem.ToString();
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
                    if (!string.IsNullOrWhiteSpace(staffcode))
                    {
                        newrow["StaffCode"] = r[staffcode];
                    }
                    if (!string.IsNullOrWhiteSpace(gender))
                    {
                        newrow["Gender"] = r[gender];
                    }
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        newrow["Title"] = r[title];
                    }
                    if (!string.IsNullOrWhiteSpace(udf))
                    {
                        newrow["UDF"] = r[udf];
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
            }
            else
            {
                this.comboUDF.IsEnabled = true;
                this.labelUDF.IsEnabled = true;
            }
        }
    }
}
