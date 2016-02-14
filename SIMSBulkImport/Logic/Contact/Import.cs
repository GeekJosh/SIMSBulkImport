using System;
using NLog;

namespace SIMSBulkImport.Contact
{
    public class Import
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public bool SetContactEmail(Int32 personID, string value, string main, string primary, string notes, string location)
        {
            return Switcher.SimsApiClass.SetContactEmail(personID, value, main, primary, notes, location);
        }

        public bool SetContactTelephone(Int32 personID, string value, string main, string primary, string notes, string location, string device)
        {
            return Switcher.SimsApiClass.SetContactTelephone(personID, value, main, primary, notes, location, device);
        }

        public bool SetContactUDF(Int32 personID, string value)
        {
            return Switcher.SimsApiClass.SetContactUDF(personID, value);
        }
    }
}