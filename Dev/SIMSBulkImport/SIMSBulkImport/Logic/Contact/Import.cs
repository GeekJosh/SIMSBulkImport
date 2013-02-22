/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;

namespace Matt40k.SIMSBulkImport.Contact
{
    public class Import
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public bool SetContactEmail(Int32 personID, string value)
        {
            return Switcher.SimsApiClass.SetContactEmail(personID, value);
        }

        public bool SetContactTelephone(Int32 personID, string value)
        {
            return Switcher.SimsApiClass.SetContactTelephone(personID, value);
        }

        public bool SetContactUDF(Int32 personID, string value)
        {
            return Switcher.SimsApiClass.SetContactUDF(personID, value);
        }
    }
}