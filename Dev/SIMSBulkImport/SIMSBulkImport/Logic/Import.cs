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
            switch (Switcher.PreImportClass.GetUserType)
            {
                case Interfaces.UserType.Contact:
                    return _importContact.SetEmail(personID, value);
                case Interfaces.UserType.Pupil:
                    return _importPupil.SetEmail(personID, value);
                case Interfaces.UserType.Staff:
                    return _importStaff.SetEmail(personID, value);
                default:
                    logger.Log(NLog.LogLevel.Error, "Import.SetEmail - UserType not set");
                    return false;
            }
        }

        public bool SetTelephone(Int32 personID, string value)
        {
            switch (Switcher.PreImportClass.GetUserType)
            {
                case Interfaces.UserType.Contact:
                    return _importContact.SetTelephone(personID, value);
                case Interfaces.UserType.Pupil:
                    return _importPupil.SetTelephone(personID, value);
                case Interfaces.UserType.Staff:
                    return _importStaff.SetTelephone(personID, value);
                default:
                    logger.Log(NLog.LogLevel.Error, "Import.SetEmail - UserType not set");
                    return false;
            }
        }

        public bool SetUDF(Int32 personID, string value)
        {
            switch (Switcher.PreImportClass.GetUserType)
            {
                case Interfaces.UserType.Contact:
                    return _importContact.SetUDF(personID, value);
                case Interfaces.UserType.Pupil:
                    return _importPupil.SetUDF(personID, value);
                case Interfaces.UserType.Staff:
                    return _importStaff.SetUDF(personID, value);
                default:
                    logger.Log(NLog.LogLevel.Error, "Import.SetEmail - UserType not set");
                    return false;
            }
        }
    }
}
