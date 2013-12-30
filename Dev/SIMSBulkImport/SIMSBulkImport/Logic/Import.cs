/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;

namespace Matt40k.SIMSBulkImport
{
    public class Import
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private Contact.Import _importContact;
        private Pupil.Import _importPupil;
        private Staff.Import _importStaff;

        public Import()
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Import()");
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

        public bool SetEmail(Int32 personID, string value)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Import.SetEmail(personID: " + personID + ", value: " + value + ")");
            switch (Switcher.PreImportClass.GetUserType)
            {
                case Interfaces.UserType.Contact:
                    return _importContact.SetContactEmail(personID, value);
                case Interfaces.UserType.Pupil:
                    return _importPupil.SetPupilEmail(personID, value);
                case Interfaces.UserType.Staff:
                    return _importStaff.SetStaffEmail(personID, value);
                default:
                    logger.Log(NLog.LogLevel.Error, "Import.SetEmail - UserType not set");
                    return false;
            }
        }

        public bool SetTelephone(Int32 personID, string value)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Import.SetTelephone(personID: " + personID + ", value: " + value + ")");
            switch (Switcher.PreImportClass.GetUserType)
            {
                case Interfaces.UserType.Contact:
                    logger.Log(NLog.LogLevel.Trace, "SetTelephone_Contact");
                    return _importContact.SetContactTelephone(personID, value);
                case Interfaces.UserType.Pupil:
                    logger.Log(NLog.LogLevel.Trace, "SetTelephone_Pupil");
                    return _importPupil.SetPupilTelephone(personID, value);
                case Interfaces.UserType.Staff:
                    logger.Log(NLog.LogLevel.Trace, "SetTelephone_Staff");
                    return _importStaff.SetStaffTelephone(personID, value);
                default:
                    logger.Log(NLog.LogLevel.Error, "Import.SetTelephone - UserType not set");
                    return false;
            }
        }

        public bool SetUDF(Int32 personID, string value)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Import.SetUDF(personID: " + personID + ", value: " + value + ")");
            switch (Switcher.PreImportClass.GetUserType)
            {
                case Interfaces.UserType.Contact:
                    return _importContact.SetContactUDF(personID, value);
                case Interfaces.UserType.Pupil:
                    return _importPupil.SetPupilUDF(personID, value);
                case Interfaces.UserType.Staff:
                    return _importStaff.SetStaffUDF(personID, value);
                default:
                    logger.Log(NLog.LogLevel.Error, "Import.SetUDF - UserType not set");
                    return false;
            }
        }
    }
}
