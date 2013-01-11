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

        private string _path;
        private string _fileName;

        internal DataSet GetDataSet
        {
            get
            {
                DataSet result = new DataSet();
                string path = System.IO.Path.Combine(_path, _fileName);
                if (!File.Exists(path))
                {
                    logger.Log(NLog.LogLevel.Error, "ImportXml.GetDataSet - File not found: " + path);
                    return null;
                }
                result.ReadXml(path, XmlReadMode.Auto);
                return result;
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
    }
}
