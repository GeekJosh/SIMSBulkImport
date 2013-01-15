/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;
using System.IO;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    internal class ImportCsv
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private string _path;
        private string _fileName;
        private DataTable _dtCsv;

        internal DataSet GetDataSet
        {
            get
            {
                DataSet CsvDataSet = new DataSet();

                _dtCsv = new DataTable(_fileName);
                if (File.Exists(_fileName))
                {
                    using (StreamReader sr = File.OpenText(_fileName))
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
                CsvDataSet.Tables.Add(_dtCsv);
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

        private void createColumns(string[] columns)
        {
            foreach (string column in columns)
            {
                _dtCsv.Columns.Add(new DataColumn(cleanInput(column), typeof(string)));
            }
        }

        private void addRow(string[] parts)
        {
            DataRow newrow = _dtCsv.NewRow();
            for (int i = 0; i < parts.Length; i++)
            {
                newrow[i] = cleanInput(parts[i]);
            }
            _dtCsv.Rows.Add(newrow);
        }

        private string cleanInput(string input)
        {
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
