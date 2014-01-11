/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using NLog;

namespace Matt40k.SIMSBulkImport.Pupil
{
    public class Import
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public bool SetPupilEmail(Int32 personID, string value)
        {
            return Switcher.SimsApiClass.SetPupilEmail(personID, value);
        }

        public bool SetPupilTelephone(Int32 personID, string value)
        {
            return Switcher.SimsApiClass.SetPupilTelephone(personID, value);
        }

        public bool SetPupilUDF(Int32 personID, string value)
        {
            return Switcher.SimsApiClass.SetPupilUDF(personID, value);
        }
    }
}