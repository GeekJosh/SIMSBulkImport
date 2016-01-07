using System.IO;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    internal class TempFile
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Creates a (hopefully) unique pretty* temporary file
        /// </summary>
        public static string GetNewTempFile
        {
            get
            {
                logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.TempFile.GetNewTempFile(GET");
                string _filename = Prefix.GetPrefix + Path.GetRandomFileName();
                _filename = Path.ChangeExtension(_filename, ".tmp");
                _filename = Path.Combine(Path.GetTempPath(), _filename);
                using (File.Create(_filename))
                {
                }
                ;
                return _filename;
            }
        }
    }
}