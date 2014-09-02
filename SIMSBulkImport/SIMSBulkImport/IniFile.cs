/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System.Runtime.InteropServices;
using System.Text;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    public class IniFile
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly string path;

        /// <summary>
        ///     INIFile Constructor.
        /// </summary>
        /// <PARAM name="INIPath"></PARAM>
        public IniFile(string INIPath)
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.IniFile.IniFile(INIPath: " + INIPath + ")");
            path = INIPath;
        }

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
            string key, string def, StringBuilder retVal,
            int size, string filePath);

        /// <summary>
        ///     Read Data Value From the Ini File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// <PARAM name="Key"></PARAM>
        /// <PARAM name="Path"></PARAM>
        /// <returns></returns>
        public string Read(string Section, string Key)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.IniFile.Read(Section: " + Section + ", Key: " + Key + ")");
            var temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp,
                255, path);
            return temp.ToString();
        }
    }
}