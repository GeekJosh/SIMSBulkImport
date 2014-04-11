/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    /// 
    /// </summary>
    public class Import
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly Contact.Import _importContact;
        private readonly Pupil.Import _importPupil;
        private readonly Staff.Import _importStaff;

        /// <summary>
        /// 
        /// </summary>
        public Import()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Import()");
            switch (Switcher.PreImportClass.GetUserType)
            {
                case Interfaces.UserType.Contact:
                    _importContact = new Contact.Import();
                    break;
                case Interfaces.UserType.Pupil:
                    _importPupil = new Pupil.Import();
                    break;
                case Interfaces.UserType.Staff:
                    _importStaff = new Staff.Import();
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="personID"></param>
        /// <param name="value"></param>
        /// <param name="main"></param>
        /// <param name="primary"></param>
        /// <returns></returns>
        public bool SetEmail(Int32 personID, string value, string main, string primary, string notes, string location)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.Import.SetEmail(personID: " + personID + ", value: " + value + ")");
            switch (Switcher.PreImportClass.GetUserType)
            {
                case Interfaces.UserType.Contact:
                    return _importContact.SetContactEmail(personID, value, main, primary, notes, location);
                case Interfaces.UserType.Pupil:
                    return _importPupil.SetPupilEmail(personID, value, main, primary, notes, location);
                case Interfaces.UserType.Staff:
                    return _importStaff.SetStaffEmail(personID, value, main, primary, notes, location);
                default:
                    logger.Log(LogLevel.Error, "Import.SetEmail - UserType not set");
                    return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="personID">PersonID</param>
        /// <param name="value">Telephone number to be set</param>
        /// <param name="main"></param>
        /// <param name="primary"></param>
        /// <returns>true - success</returns>
        public bool SetTelephone(Int32 personID, string value, string main, string primary, string notes, string location, string device)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.Import.SetTelephone(personID: " + personID + ", value: " + value + ")");
            switch (Switcher.PreImportClass.GetUserType)
            {
                case Interfaces.UserType.Contact:
                    logger.Log(LogLevel.Trace, "SetTelephone_Contact");
                    return _importContact.SetContactTelephone(personID, value, main, primary, notes, location, device);
                case Interfaces.UserType.Pupil:
                    logger.Log(LogLevel.Trace, "SetTelephone_Pupil");
                    return _importPupil.SetPupilTelephone(personID, value, main, primary, notes, location, device);
                case Interfaces.UserType.Staff:
                    logger.Log(LogLevel.Trace, "SetTelephone_Staff");
                    return _importStaff.SetStaffTelephone(personID, value, main, primary, notes, location, device);
                default:
                    logger.Log(LogLevel.Error, "Import.SetTelephone - UserType not set");
                    return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="personID">PersonID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetUDF(Int32 personID, string value)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.Import.SetUDF(personID: " + personID + ", value: " + value + ")");
            switch (Switcher.PreImportClass.GetUserType)
            {
                case Interfaces.UserType.Contact:
                    return _importContact.SetContactUDF(personID, value);
                case Interfaces.UserType.Pupil:
                    return _importPupil.SetPupilUDF(personID, value);
                case Interfaces.UserType.Staff:
                    return _importStaff.SetStaffUDF(personID, value);
                default:
                    logger.Log(LogLevel.Error, "Import.SetUDF - UserType not set");
                    return false;
            }
        }
    }
}