/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;

namespace Matt40k.SIMSBulkImport.Pupil
{
    public class Import
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public bool SetEmail(Int32 personID, string value)
        {
            return false;
        }

        public bool SetTelephone(Int32 personID, string value)
        {
            return false;
        }

        public bool SetUDF(Int32 personID, string value)
        {
            return false;
        }
    }
}