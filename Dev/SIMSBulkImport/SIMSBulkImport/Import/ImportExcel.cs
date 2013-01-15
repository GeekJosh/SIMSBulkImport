/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;
using System.IO;
using Microsoft.Office.Interop.Excel;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    internal class ImportExcel
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private string _filePath;

        internal string SetFilePath
        {
            set
            {
                if (File.Exists(value))
                    _filePath = value;
            }
        }

        // Reference: http://www.dotneter.com/reading-excel-and-binding-to-datagridview-using-microsoft-office-interop-excel
        internal DataSet GetDataSet
        {
            get
            {
                logger.Log(NLog.LogLevel.Debug, "GetExcel");
                DataSet _dataSet = new DataSet();
                try
                {
                    Workbook workbook;
                    Application excelApp = new Application();

                    workbook = excelApp.Workbooks.Open(_filePath, 0, true, 5, "", "", true, XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    logger.Log(NLog.LogLevel.Debug, "Worksheets: " + workbook.Sheets.Count);

                    foreach (Worksheet ws in workbook.Sheets)
                    {
                        System.Data.DataTable _dataTable = new System.Data.DataTable(ws.Name);
                        Range range = ws.UsedRange;

                        int column = 0;
                        int row = 0;

                        if (range.Columns.Count > 1 && range.Rows.Count > 1)
                        {
                            for (column = 1; column <= range.Columns.Count; column++)
                            {
                                string _columnName = (range.Cells[1, column] as Range).Value2.ToString();
                                _dataTable.Columns.Add(_columnName);
                            }

                            for (row = 2; row <= range.Rows.Count; row++)
                            {
                                DataRow dr = _dataTable.NewRow();
                                for (column = 1; column <= range.Columns.Count; column++)
                                {
                                    dr[column - 1] = (range.Cells[row, column] as Range).Value2.ToString();
                                }
                                _dataTable.Rows.Add(dr);
                            }
                        }
                        _dataSet.Tables.Add(_dataTable);
                    }
                    workbook.Close(true, null, null);
                    excelApp.Quit();
                }
                catch (Exception GetDataSet_Exception)
                {
                    logger.Log(NLog.LogLevel.Error, GetDataSet_Exception);
                }
                return _dataSet;
            }
        }
    }
}
