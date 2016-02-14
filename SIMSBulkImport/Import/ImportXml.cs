using System.Data;
using System.IO;
using NLog;

namespace SIMSBulkImport
{
    internal class ImportXml
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private string _filePath;

        internal DataSet GetDataSet
        {
            get
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.ImportXml.GetDataSet(GET)");
                var result = new DataSet();
                if (!File.Exists(_filePath))
                {
                    logger.Log(LogLevel.Error, "ImportXml.GetDataSet - File not found: " + _filePath);
                    return null;
                }
                result.ReadXml(_filePath, XmlReadMode.Auto);
                return result;
            }
        }

        internal string SetFilePath
        {
            set
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.ImportXml.SetFilePath(SET: " + value + ")");
                _filePath = value;
            }
        }
    }
}