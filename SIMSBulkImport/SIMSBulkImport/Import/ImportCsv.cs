/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System.Data;
using System.IO;
using NLog;

namespace Matt40k.SIMSBulkImport
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
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportCsv.GetDataSet(GET)");
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
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportCsv.SetFilePath(SET: " + value + ")");
                _filePath = value;
            }
        }

        private void createColumns(string[] columns)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.ImportCsv.createColumns(columns: " + columns + ")");
            foreach (string column in columns)
            {
                _dtCsv.Columns.Add(new DataColumn(cleanInput(column), typeof (string)));
            }
        }

        private void addRow(string[] parts)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportCsv.addRow(parts: " + parts + ")");
            DataRow newrow = _dtCsv.NewRow();
            for (int i = 0; i < parts.Length; i++)
            {
                newrow[i] = cleanInput(parts[i]);
            }
            _dtCsv.Rows.Add(newrow);
        }

        private string cleanInput(string input)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportCsv.cleanInput(input: " + input + ")");
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