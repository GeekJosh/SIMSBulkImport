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

        private string _importFilePath;
        private DataSet _importDataSet;

        private string _path;
        private string _fileName;
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
                    _importFilePath = value;
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

            _path = Path.GetDirectoryName(_importFilePath);
            _fileName = Path.GetFileName(_importFilePath);
            _importFileType = importFileType;

            logger.Log(LogLevel.Debug, _path);
            logger.Log(LogLevel.Debug, _fileName);


            Thread oThread = new Thread(new ThreadStart(spawnSeparateThread));

            // Start the thread
            oThread.Start();

            // Spin for a while waiting for the started thread to become
            // alive:
            while (!oThread.IsAlive) ;

            while (!_importComplete)
            {
                // Put the Main thread to sleep for 1 millisecond to allow oThread
                // to do some work:
                Thread.Sleep(1);
            }
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
                string ext = Path.GetExtension(_importFilePath);
                switch (ext)
                {
                    case ".csv":
                        return FileType.Csv;
                    case ".xml":
                        return FileType.Xml;
                    case ".xls":
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
                    ImportCsv _importCsv = new ImportCsv();
                    _importCsv.SetFileName = _fileName;
                    _importCsv.SetPath = _path;
                    _importDataSet = _importCsv.GetDataSet;
                    _importComplete = true;
                    break;
                case FileType.Xml:
                    ImportXml _importXml = new ImportXml();
                    _importXml.SetFileName = _fileName;
                    _importXml.SetPath = _path;
                    _importDataSet = _importXml.GetDataSet;
                    _importComplete = true;
                    break;
                case FileType.Xls:
                    ImportExcel _importExcel = new ImportExcel();
                    _importExcel.SetFileName = _fileName;
                    _importExcel.SetPath = _path;
                    _importDataSet = _importExcel.GetDataSet;
                    _importComplete = true;
                    break;
                case FileType.Unknown:
                    break;
            }
        }
    }
}
