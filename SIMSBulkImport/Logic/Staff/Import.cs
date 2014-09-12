/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using NLog;

namespace Matt40k.SIMSBulkImport.Staff
{
    public class Import
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public bool SetStaffEmail(Int32 personID, string value, string main, string primary, string notes, string location)
        {
            logger.Log(LogLevel.Trace, "Staff.Import.SetStaffEmail");
            return Switcher.SimsApiClass.SetStaffEmail(personID, value, main, primary, notes, location);
        }

        public bool SetStaffTelephone(Int32 personID, string value, string main, string primary, string notes, string location, string device)
        {
            logger.Log(LogLevel.Trace, "Staff.Import.SetStaffTelephone");
            return Switcher.SimsApiClass.SetStaffTelephone(personID, value, main, primary, notes, location, device);
        }

        public bool SetStaffUDF(Int32 personID, string value)
        {
            logger.Log(LogLevel.Trace, "Staff.Import.SetStaffUDF");
            return Switcher.SimsApiClass.SetStaffUDF(personID, value);
        }
    }
}