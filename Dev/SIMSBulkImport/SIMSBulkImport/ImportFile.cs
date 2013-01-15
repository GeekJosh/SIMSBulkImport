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
    internal class ImportFile
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private string _filePath;
        private DataSet _importDataSet;

        private FileType _importFileType;
        private bool _importComplete;

        internal string SetImportFilePath
        {
            set
            {
                logger.Log(LogLevel.Debug, "SetImportFilePath");
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
                return _importDataSet;
            }
        }

        internal void GetImportDataSet()
        {
            logger.Log(LogLevel.Debug, "GetImportDataSet start");
            logger.Log(LogLevel.Debug, _filePath);

            _importFileType = importFileType;
            spawnSeparateThread();

            logger.Log(NLog.LogLevel.Error, "GetImportDataSet end");
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
                return (importFileType == FileType.Xls);
            }
        }

        internal string[] GetSheets
        {
            get
            {
                // TODO
                return null;
            }
        }

        private FileType importFileType
        {
            get
            {
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

        private void spawnSeparateThread()
        {
            switch (_importFileType)
            {
                case FileType.Csv:
                    logger.Log(NLog.LogLevel.Debug, "spawnSeparateThread: Csv");
                    ImportCsv _importCsv = new ImportCsv();
                    _importCsv.SetFilePath = _filePath;
                    _importDataSet = _importCsv.GetDataSet;
                    _importComplete = true;
                    break;
                case FileType.Xml:
                    logger.Log(NLog.LogLevel.Debug, "spawnSeparateThread: Xml");
                    ImportXml _importXml = new ImportXml();
                    _importXml.SetFilePath = _filePath;
                    _importDataSet = _importXml.GetDataSet;
                    _importComplete = true;
                    break;
                case FileType.Xls:
                    logger.Log(NLog.LogLevel.Debug, "spawnSeparateThread: Xls");
                    ImportExcel _importExcel = new ImportExcel();
                    _importExcel.SetFilePath = _filePath;
                    _importDataSet = _importExcel.GetDataSet;
                    _importComplete = true;
                    break;
                case FileType.Unknown:
                    break;
            }
        }

        public void Reset()
        {
            // TODO
        }

        public bool IsImportFileSet
        {
            get
            {
                return !string.IsNullOrEmpty(_filePath);
            }
        }
    }
}
