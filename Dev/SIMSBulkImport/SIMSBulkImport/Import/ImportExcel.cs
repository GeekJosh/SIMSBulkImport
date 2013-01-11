/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;
using System.Data.OleDb;
//using System.IO;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    internal class ImportExcel
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        
        private string _path;
        private string _fileName;

        internal DataSet GetDataSet
        {
            get
            {
                DataSet ExcelDataSet = new DataSet();
                try
                {
                    logger.Log(LogLevel.Debug, "ExcelDataSet");

                    string tmpSelect = "SELECT * FROM [" + _fileName + "]";
                    string tmpConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _path + ";Extended Properties=Text;";

                    OleDbConnection ExcelConnection = new OleDbConnection(tmpConn);
                    OleDbCommand ExcelCommand = new OleDbCommand(tmpSelect, ExcelConnection);
                    OleDbDataAdapter ExcelAdapter = new OleDbDataAdapter(ExcelCommand);
                    ExcelConnection.Open();
                    //ExcelAdapter.ContinueUpdateOnError = true;
                    ExcelAdapter.Fill(ExcelDataSet);
                    ExcelConnection.Close();
                }
                catch (Exception GetCSVFileException)
                {
                    logger.Log(NLog.LogLevel.Error, GetCSVFileException);
                }
                return ExcelDataSet;
            }
        }

        internal string SetPath
        {
            set
            {
                _path = value;
            }
        }

        internal string SetFileName
        {
            set
            {
                _fileName = value;
            }
        }

        internal string[] GetExcelWorkSheetsList
        {
            get
            {
                DataTable ExcelSheets;
                OleDbConnection ExcelConnection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _path + @"\" + _fileName + ";Extended Properties=Excel 8.0;");
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
        }
    }
}
