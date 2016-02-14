using System;
using NLog;

namespace SIMSBulkImport.Pupil
{
    public class Import
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public bool SetPupilEmail(Int32 personID, string value, string main, string primary, string notes, string location)
        {
            return Switcher.SimsApiClass.SetPupilEmail(personID, value, main, primary, notes, location);
        }

        public bool SetPupilTelephone(Int32 personID, string value, string main, string primary, string notes, string location, string device)
        {
            return Switcher.SimsApiClass.SetPupilTelephone(personID, value, main, primary, notes, location, device);
        }

        public bool SetPupilUDF(Int32 personID, string value)
        {
            return Switcher.SimsApiClass.SetPupilUDF(personID, value);
        }
    }
}