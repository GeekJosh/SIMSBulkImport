/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    internal class ImportCsv
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private string _path;
        private string _fileName;

        internal DataSet GetDataSet
        {
            get
            {
                DataSet CsvDataSet = new DataSet();
                try
                {
                    string oldFile = Path.Combine(_path, _fileName);
                    string newFile = NumberAsText.Fixed(oldFile);
                    string pathName = Path.GetDirectoryName(newFile);
                    string fileName = Path.GetFileName(newFile);

                    string tmpSelect = "SELECT * FROM [" + fileName + "]";
                    string tmpConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Extended Properties=Text;";

                    OleDbConnection ExcelConnection = new OleDbConnection(tmpConn);
                    OleDbCommand ExcelCommand = new OleDbCommand(tmpSelect, ExcelConnection);
                    OleDbDataAdapter ExcelAdapter = new OleDbDataAdapter(ExcelCommand);
                    ExcelConnection.Open();
                    //ExcelAdapter.ContinueUpdateOnError = true;
                    ExcelAdapter.Fill(CsvDataSet);
                    ExcelConnection.Close();
                }
                catch (Exception GetCSVFileException)
                {
                    logger.Log(NLog.LogLevel.Error, GetCSVFileException);
                }
                return CsvDataSet;
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
    }
}
