/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Configuration;

using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Matt40k.SIMSBulkImport
{
    public class License
    {
        private string licensee_la;
        private string licensee_sch;
        private string licenseeName;
        /*
         * 0 - Demo (Green Abbey)
         * 1 - Invalid license
         * 2 - School license
         * 3 - LA license
         */
        int license_type = 0;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public string GetDfE
        {
            get { return licensee_la + licensee_sch; }
        }

        public string GetName
        {
            get { return licenseeName; }
        }

        public string GetLicenseType
        {
            get
            {
                if (license_type == 3) { return "Local Authority"; }
                if (license_type == 2) { return "School"; }
                if (license_type == 0) { return "Demo"; }
                return "Invalid";
            }
        }

        public string GetKey
        {
            get
            {
                return ConfigurationManager.AppSettings["LicenseKey"];
            }
        }

        public bool SetKey(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                logger.Log(NLog.LogLevel.Debug, "License key: " + value);
                if (!validLicense(value))
                {
                    logger.Log(NLog.LogLevel.Error, "Invalid License key entered!");
                    return false;
                }
            }
            else
            {
                logger.Log(NLog.LogLevel.Info, "No license key set");
                return false;
            }

            if (!ConfigMan.SetConfig("LicenseKey", value))
            {
                System.Windows.Forms.MessageBox.Show("Error: Unable to write config", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            return true;
        }

        public License()
        {
            licensee_la = "823";
            licensee_sch = "4321";
            licenseeName = "Green Abbey School";
            license_type = 1;

            /*
            if (!string.IsNullOrEmpty(GetKey))
            {
                logger.Log(NLog.LogLevel.Debug, "License key: " + GetKey);
                if (!validLicense(GetKey))
                {
                    logger.Log(NLog.LogLevel.Error, "Invalid License key!");
                    license_type = 1;
                }
                else
                {

                }
            }
            else
            {
                logger.Log(NLog.LogLevel.Info, "No license key set");
            }*/
        }

        private bool validLicense(string licenseKey)
        {
            // Key format:
            // LEA,SCHCODE,SCHOOL NAME,LA\SCHOOL
            string t1 = Decrypt(licenseKey);
            if (string.IsNullOrEmpty(t1))
            {
                return false;
            }
            string[] t2 = t1.Split(',');
            if (t2.Length != 4)
            {
                return false;
            }
            if (t2[0].Length != 3)
            {
                return false;
            }
            if (t2[3] == "2" || t2[3] == "3")
            {
                licensee_la = t2[0];
                licensee_sch = t2[1];
                licenseeName = t2[2];
                try
                {
                    license_type = Convert.ToInt32(t2[3]);
                }
                catch
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool IsLicensed(string leacode, string schcode)
        {
            return true;
            /*
            switch (license_type)
            {
                case 3:
                    if (leacode == licensee_la) { return true; }
                    break;
                case 2:
                    if (leacode == licensee_la) { if (schcode == licensee_sch) { return true; } }
                    break;
                default:
                    return false;
            }         
            return false;
             */
        }

        public string DisplayLicense
        {
            get
            {
                switch (license_type)
                {
                    case 1:
                        return "Unlicensed! - Invalid license key entered!";
                    case 2:
                        return "Licensed to: " + licenseeName + " (" + licensee_la + licensee_sch + ") only";
                    case 3:
                        return "Licensed to: "+ licenseeName + " (" + licensee_la + ") Local Authority only";
                    default:
                        return "Demo License: Green Abbey School (8234321)";
                }
            }
        }

        public string Decrypt(string license)
        {
            var iv = "45287112549354892144548565456541";
            var key = "anjueolkdiwpoida";

            byte[] decode = Decode(license);

            if (decode != null)
            {
                return DecryptRJ256(decode, key, iv);
            }
            return null;
        }

        public byte[] Decode(string str)
        {
            try
            {
                var decbuff = Convert.FromBase64String(str);
                return decbuff;
            }
            catch (System.Exception DecodeException)
            {
                logger.Log(NLog.LogLevel.Error, "Invalid key!");
                logger.Log(NLog.LogLevel.Error, DecodeException);
            }
            return null;
        }

        static public String DecryptRJ256(byte[] cypher, string KeyString, string IVString)
        {
            var sRet = "";

            try
            {
                var encoding = new UTF8Encoding();
                var Key = encoding.GetBytes(KeyString);
                var IV = encoding.GetBytes(IVString);

                using (var rj = new RijndaelManaged())
                {
                    try
                    {
                        rj.Padding = PaddingMode.PKCS7;
                        rj.Mode = CipherMode.CBC;
                        rj.KeySize = 256;
                        rj.BlockSize = 256;
                        rj.Key = Key;
                        rj.IV = IV;
                        var ms = new MemoryStream(cypher);

                        using (var cs = new CryptoStream(ms, rj.CreateDecryptor(Key, IV), CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                sRet = sr.ReadLine();
                            }
                        }
                    }
                    finally
                    {
                        rj.Clear();
                    }
                }
            }
            catch (CryptographicException decryptexception)
            {
                logger.Log(NLog.LogLevel.Error, "Invalid key!");
                logger.Log(NLog.LogLevel.Error, decryptexception);
            }
            return sRet;
        }


    }
}


