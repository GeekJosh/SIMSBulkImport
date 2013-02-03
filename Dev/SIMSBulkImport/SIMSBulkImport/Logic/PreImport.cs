﻿/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;

namespace Matt40k.SIMSBulkImport
{
    public class PreImport
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private bool ignoreFirstRow;
        private DataTable importDataTable;

        public int GetImportFileRecordCount
        {
            get
            {
                int importFileRecordCount = importDataTable.Rows.Count;
                logger.Log(NLog.LogLevel.Debug, "RecordCount: " + importFileRecordCount);
                return importFileRecordCount;
            }
        }

        public bool SetMatchIgnoreFirstRow
        {
            set
            {
                logger.Log(NLog.LogLevel.Debug, "ignoreFirstRow: " + value);
                ignoreFirstRow = value;
            }
        }

        public DataTable SetImportDataset
        {
            set
            {
                // Remove First row (header) if set
                if (ignoreFirstRow)
                    value.Rows[0].Delete();

                // Remove duplicate entries
                value = Validation.DeDuplicatation(value);

                importDataTable = value;
            }
        }

        public DataTable GetImportDataTable
        {
            get
            {
                return importDataTable;
            }
        }
    }
}
