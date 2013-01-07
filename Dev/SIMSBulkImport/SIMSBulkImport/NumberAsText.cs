/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.IO;

namespace Matt40k.SIMSBulkImport
{
    class NumberAsText
    {
        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static string Fixed(string file)
        {
            string newFile = null;
            string finalFile = null;
            newFile = TempFile.GetNewTempFile;
            finalFile = TempFile.GetNewTempFile;

            logger.Log(NLog.LogLevel.Debug, newFile);
            logger.Log(NLog.LogLevel.Debug, finalFile);
          
            File.Copy(file, newFile, true);

            StreamWriter finFile;
            StreamReader tmpFile;
            string line = null;
            try
            {
                finFile = new StreamWriter(finalFile);
                tmpFile = new StreamReader(file);
                               
                int i = 0;
                while ((line = tmpFile.ReadLine()) != null)
                {
                    string fixedline;
                    if (i < 3)
                    {
                        fixedline = fix(line);
                    }
                    else
                    {
                        fixedline = line;
                    }
                    logger.Log(NLog.LogLevel.Debug, fixedline);
                    finFile.WriteLine(fix(line));
                    i++;
                }
                finFile.Close();
            }
            catch (Exception readFileException)
            {
                logger.Log(NLog.LogLevel.Error, readFileException);
                File.Delete(newFile);
                File.Delete(finalFile);
                return null;
            }
            try
            {
                File.Delete(newFile);
            }
            catch (Exception readFile_DeleteTmpFile_Exception)
            {
                logger.Log(NLog.LogLevel.Error, readFile_DeleteTmpFile_Exception);
            }
            return finalFile;
        }

        static string fix(string toFix)
        {
            string fixedText = null;
            if (!string.IsNullOrEmpty(toFix))
            {
                string[] parts = toFix.Split(',');
                try
                {
                    foreach (string part in parts)
                    {
                        if (isNo(part.Substring(0, 1)))
                        {
                            fixedText = fixedText + "\"" + part + "\"";
                        }
                        else
                        {
                            fixedText = fixedText + part;
                        }
                        fixedText = fixedText + ",";
                    }
                    fixedText = fixedText.Substring(0, (fixedText.Length - 1));
                }
                catch (Exception fix_Exception)
                {
                    logger.Log(NLog.LogLevel.Error, fix_Exception);
                } 
            }
            return fixedText;
        }

        // Not the greatest - but I didnt want to do a Convert.Int32 as I don't want extra Exception 
        static bool isNo(string item)
        {
            switch (item)
            {
                case "0":
                    return true;
                case "1":
                    return true;
                case "2":
                    return true;
                case "3":
                    return true;
                case "4":
                    return true;
                case "5":
                    return true;
                case "6":
                    return true;
                case "7":
                    return true;
                case "8":
                    return true;
                case "9":
                    return true;
                default:
                    return false;
            }
        }
    }
}
