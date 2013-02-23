/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;

namespace Matt40k.SIMSBulkImport.Staff
{
    public class Import
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public bool SetStaffEmail(Int32 personID, string value)
        {
            // REDUCE TO TRACE
            logger.Log(NLog.LogLevel.Debug, "Staff.Import.SetStaffEmail");
            return Switcher.SimsApiClass.SetStaffEmail(personID, value);
        }

        public bool SetStaffTelephone(Int32 personID, string value)
        {
            // REDUCE TO TRACE
            logger.Log(NLog.LogLevel.Debug, "Staff.Import.SetStaffTelephone");
            return Switcher.SimsApiClass.SetStaffTelephone(personID, value);
        }

        public bool SetStaffUDF(Int32 personID, string value)
        {
            // REDUCE TO TRACE
            logger.Log(NLog.LogLevel.Debug, "Staff.Import.SetStaffUDF");
            return Switcher.SimsApiClass.SetStaffUDF(personID, value);
        }
    }
}