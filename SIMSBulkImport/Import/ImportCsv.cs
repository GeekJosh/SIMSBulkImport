﻿using System.Data;
using System.IO;
using NLog;

namespace SIMSBulkImport
{
    internal class ImportCsv
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private DataTable _dtCsv;
        private string _filePath;

        internal DataSet GetDataSet
        {
            get
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.ImportCsv.GetDataSet(GET)");
                var CsvDataSet = new DataSet();

                _dtCsv = new DataTable(_filePath);
                if (File.Exists(_filePath))
                {
                    using (StreamReader sr = File.OpenText(_filePath))
                    {
                        string s = "";
                        int rowCount = 0;
                        while ((s = sr.ReadLine()) != null)
                        {
                            string[] parts = s.Split(',');
                            if (rowCount == 0)
                                createColumns(parts);
                            addRow(parts);
                            rowCount = rowCount + 1;
                        }
                    }
                }
                else
                {
                    logger.Log(LogLevel.Error, "GetDataSet: File does not exist: " + _filePath);
                }
                CsvDataSet.Tables.Add(_dtCsv);
                return CsvDataSet;
            }
        }

        internal string SetFilePath
        {
            set
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.ImportCsv.SetFilePath(SET: " + value + ")");
                _filePath = value;
            }
        }

        private void createColumns(string[] columns)
        {
            logger.Log(LogLevel.Debug,
                "Trace:: SIMSBulkImport.ImportCsv.createColumns(columns: " + columns + ")");
            foreach (string column in columns)
            {
                _dtCsv.Columns.Add(new DataColumn(cleanInput(column), typeof (string)));
            }
        }

        private void addRow(string[] parts)
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.ImportCsv.addRow(parts: " + parts + ")");
            DataRow newrow = _dtCsv.NewRow();
            for (int i = 0; i < parts.Length; i++)
            {
                newrow[i] = cleanInput(parts[i]);
            }
            _dtCsv.Rows.Add(newrow);
        }

        private string cleanInput(string input)
        {
            logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.ImportCsv.cleanInput(input: " + input + ")");
            string output = input;
            if (output.Length > 1)
            {
                string firstChar = output.Substring(0, 1);
                if (firstChar == "\"")
                    output = output.Substring(1);
                int lastCharPosition = (output.Length - 1);
                string lastChar = output.Substring(lastCharPosition);
                if (lastChar == "\"")
                    output = output.Substring(0, lastCharPosition);
            }
            return output;
        }
    }
}