/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace UpdateConfig
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No parameters set");
                Environment.Exit(1);
            }

            if (string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("Argument 1 is null");
                Environment.Exit(2);
            }

            if (args.Length == 1)
            {
                string licenseKey = args[0];
                if (!string.IsNullOrEmpty(licenseKey))
                {
                    if (licenseKey.ToUpper() != "DEMO")
                    {
                        WriteLicense(licenseKey);
                    }
                    Environment.Exit(0);
                }
                Environment.Exit(6);
            }

            if (string.IsNullOrWhiteSpace(args[1]))
            {
                Console.WriteLine("Argument 2 is null");
                Environment.Exit(3);
            }

            try
            {
                //Console.WriteLine(args[0]);
                //Console.WriteLine(args[1]);
                File.Copy(args[0], args[1], true);
            }
            catch (Exception CopyException)
            {
                Console.WriteLine("Error copying file");
                Console.WriteLine(CopyException.ToString());
                Environment.Exit(4);
            }
            
            try
            {
                File.Delete(args[0]);
            }
            catch (Exception)
            {
                Console.WriteLine("Error delete temp file");
            }
            Environment.Exit(0);
            //Console.ReadKey();
        }

        private static bool writeConfig(string value, string field)
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();

            fileMap.ExeConfigFilename = Path.Combine(FilePath, "SIMSBulkImport.exe.config");
            System.Configuration.Configuration config =
                ConfigurationManager.OpenMappedExeConfiguration(fileMap,
                ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(field);
            config.AppSettings.Settings.Add(field, value);
            try
            {
                // Save the configuration file.
                config.Save(ConfigurationSaveMode.Modified);
            }
            catch (Exception)
            {
                return true;
            }
            return false;
        }

        private static void writeUpdateURL(string value)
        {
            writeConfig(value, "UpdateURL");
        }

        private static void writeAutoUpdate(string value)
        {
            writeConfig(value, "AutoUpdate");
        }

        private static void WriteLicense(string value)
        {
            writeConfig(value, "LicenseKey");
            //Environment.Exit(5);
        }

        private static string FilePath
        {
            get
            {
                return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Substring(6);
            }
        }
    }
}
