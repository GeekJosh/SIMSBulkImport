/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Threading;
using NLog;

using System.Data.OleDb;

namespace Matt40k.SIMSBulkImport
{
    public class ImportFile
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private string _filePath;
        private DataSet _importDataSet;
        private FileType _importFileType;

        internal string SetImportFilePath
        {
            set
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportFile.SetImportFilePath(SET: " + value + ")");
                if (File.Exists(value))
                {
                    logger.Log(LogLevel.Debug, value);
                    _filePath = value;
                }
                else
                {
                    logger.Log(LogLevel.Error, "File does not exist:" + value);
                }
            }
        }

        internal DataSet GetDataSet
        {
            get
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportFile.GetDataSet(GET)");
                return _importDataSet;
            }
        }

        internal void GetImportDataSet()
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportFile.GetImportDataSet()");

            _importFileType = importFileType;

            switch (_importFileType)
            {
                case FileType.Csv:
                    ImportCsv _importCsv = new ImportCsv();
                    _importCsv.SetFilePath = _filePath;
                    _importDataSet = _importCsv.GetDataSet;
                    break;
                case FileType.Xml:
                    ImportXml _importXml = new ImportXml();
                    _importXml.SetFilePath = _filePath;
                    _importDataSet = _importXml.GetDataSet;
                    break;
                case FileType.Xls:
                    ImportExcel _importExcel = new ImportExcel();
                    _importExcel.SetFilePath = _filePath;
                    _importDataSet = _importExcel.GetDataSet;
                    break;
                case FileType.Unknown:
                    break;
            }

        }

        private enum FileType
        {
            Xml,
            Csv,
            Xls,
            Unknown
        };

        internal bool GetIsExcel
        {
            get
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportFile.GetIsExcel(GET)");
                return (importFileType == FileType.Xls);
            }
        }

        private FileType importFileType
        {
            get
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportFile.importFileType(GET)");
                string ext = Path.GetExtension(_filePath);
                logger.Log(NLog.LogLevel.Debug, "importFileType: " + ext);
                switch (ext)
                {
                    case ".csv":
                        return FileType.Csv;
                    case ".xml":
                        return FileType.Xml;
                    case ".xls":
                        return FileType.Xls;
                    case ".xlsx":
                        return FileType.Xls;
                    case ".txt":
                        return FileType.Csv;
                    default:
                        return FileType.Unknown;
                }
            }
        }

        public void Reset()
        {
            // TODO
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.ImportFile.Reset()");
        }
    }
}
