/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;
using System.IO;

namespace Matt40k.SIMSBulkImport
{
    internal class ImportXml
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private string _filePath;

        internal DataSet GetDataSet
        {
            get
            {
                DataSet result = new DataSet();
                if (!File.Exists(_filePath))
                {
                    logger.Log(NLog.LogLevel.Error, "ImportXml.GetDataSet - File not found: " + _filePath);
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
                _filePath = value;
            }
        }
    }
}
