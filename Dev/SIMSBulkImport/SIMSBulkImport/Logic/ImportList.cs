/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;

namespace Matt40k.SIMSBulkImport
{
    public class ImportList
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private DataTable _importTable;

        public ImportList()
        {
            Switcher.ResultsImportClass = new ResultsImport();
            _importTable = new DataTable();
            _importTable.Columns.Add(new DataColumn("Type", typeof(string)));
            _importTable.Columns.Add(new DataColumn("PersonID", typeof(Int32)));
            _importTable.Columns.Add(new DataColumn("Value", typeof(string)));
        }

        public int GetImportCount
        {
            get
            {
                return _importTable.Rows.Count;
            }
        }

        public DataRow GetListRow(int rowNo)
        {
            return _importTable.Rows[rowNo];
        }

        public void AddToList(DataRow row)
        {
            try
            {
                string status = (string)row["Status"];
                Int32 personID =  Convert.ToInt32((string)row["PersonID"]);

                if (status.StartsWith("Import"))
                {
                    if (status.Contains("Email"))
                    {
                        AddToImportTable("Email", personID, (string)row["Import email"]);
                    }
                    if (status.Contains("Telephone"))
                    {
                        AddToImportTable("Telephone", personID, (string)row["Import telephone"]);
                    }
                    if (status.Contains("UDF"))
                    {
                        AddToImportTable("UDF", personID, (string)row["Import UDF"]);
                    }
                }
                else
                {
                    // TODO - Add to Results table as failure
                }
            }
            catch (Exception AddToList_Exception)
            {
                logger.Log(NLog.LogLevel.Error, AddToList_Exception);
            }
        }

        private bool AddToImportTable(string Type, Int32 PersonID, string Value)
        {
            try
            {
                DataRow newrow = _importTable.NewRow();
                newrow["Type"] = Type;
                newrow["PersonID"] = PersonID;
                newrow["Value"] = Value;
                _importTable.Rows.Add(newrow);
            }
            catch (Exception AddToImportTable_Exception)
            {
                logger.Log(NLog.LogLevel.Error, AddToImportTable_Exception);
                return false;
            }
            return true;
        }
    }
}