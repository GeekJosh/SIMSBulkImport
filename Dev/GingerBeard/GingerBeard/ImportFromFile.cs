using System;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace GingerBeard
{
    public class ImportFromFile
    {
        string pathName = null;
        string fileName = null;
        string extName = null;
        int workSheetNo = 0;
        bool isExcel = false;

        public bool GetIsExcel
        {
            get { return isExcel; }
        }

        public string SetFile {
            set {
                pathName = Path.GetDirectoryName(value);
                fileName = Path.GetFileName(value);
                extName = Path.GetExtension(value);
                if (extName == ".xls") { isExcel = true; }
            }
        }

        public string[] GetSheets
        {
            get
            {
                switch (extName)
                {
                    case ".xls":
                        return GetExcelWorkSheetsList(pathName, fileName);
                    default:
                        return null;
                }
            }
        }

        public DataSet GetDataSetFromFile
        {
            get
            {
                DataSet ds = null;

                //System.Windows.MessageBox.Show(pathName);
                switch (extName)
                {
                    case ".csv":
                        return GetCVSFile(pathName, fileName);
                    case ".xml":
                        return GetXmlFile(pathName, fileName);
                    case ".xls":
                        return GetExcelWorkSheet(pathName, fileName, workSheetNo);
                    case ".txt":
                        return GetCVSFile(pathName, fileName);
                    default:
                        break;
                }
                return ds;
            }
        }

        public int SetWorkBook
        {
            set { workSheetNo = value; }
        }

        private string[] GetExcelWorkSheetsList(string pathName, string fileName)
        {
            DataTable ExcelSheets;
            OleDbConnection ExcelConnection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + @"\" + fileName + ";Extended Properties=Excel 8.0;");
            OleDbCommand ExcelCommand = new OleDbCommand();
            ExcelCommand.Connection = ExcelConnection;
            OleDbDataAdapter ExcelAdapter = new OleDbDataAdapter(ExcelCommand);

            ExcelConnection.Open();
            ExcelSheets = ExcelConnection.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            ExcelConnection.Close();

            string sheets = null;
            foreach (DataRow sheet in ExcelSheets.Rows)
            {
                    sheets = sheets + sheet["TABLE_NAME"].ToString() + ",";
            }
            sheets = sheets.Substring(0, sheets.Length - 1);

            return sheets.Split(',');
        }

        private DataSet GetExcelWorkSheet(string pathName, string fileName, int workSheetNumber)
        {
            OleDbConnection ExcelConnection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + @"\" + fileName + ";Extended Properties=Excel 8.0;");
            OleDbCommand ExcelCommand = new OleDbCommand();
            ExcelCommand.Connection = ExcelConnection;
            OleDbDataAdapter ExcelAdapter = new OleDbDataAdapter(ExcelCommand);
            DataSet ExcelDataSet = new DataSet();
            try
            {
                ExcelConnection.Open();
                DataTable ExcelSheets = ExcelConnection.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                string SpreadSheetName = "[" + ExcelSheets.Rows[workSheetNumber]["TABLE_NAME"].ToString() + "]";
                ExcelCommand.CommandText = @"SELECT * FROM " + SpreadSheetName;
                ExcelAdapter.Fill(ExcelDataSet);
            }
            catch (Exception)
            {

            }
            ExcelConnection.Close();
            return ExcelDataSet;
        }

        private DataSet GetCVSFile(string pathName, string fileName)
        {
            OleDbConnection ExcelConnection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Extended Properties=Text;");
            OleDbCommand ExcelCommand = new OleDbCommand(@"SELECT * FROM " + fileName, ExcelConnection);
            OleDbDataAdapter ExcelAdapter = new OleDbDataAdapter(ExcelCommand);
            ExcelConnection.Open();
            DataSet ExcelDataSet = new DataSet();
            ExcelAdapter.Fill(ExcelDataSet);
            ExcelConnection.Close();
            return ExcelDataSet;
        }

        private DataSet GetXmlFile(string pathName, string fileName)
        {
            string path = System.IO.Path.Combine(pathName, fileName);
            DataSet result = new DataSet();
            result.ReadXml(path, XmlReadMode.Auto);
            return result;
        }
    }
}
