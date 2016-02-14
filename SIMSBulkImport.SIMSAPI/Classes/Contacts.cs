using System;
using System.Data;
using NLog;
using SIMS.Entities;
using SIMS.Processes;
using Exception = System.Exception;
using PersonCache = SIMS.Entities.PersonCache;

namespace SIMSBulkImport.Classes
{
    /// <summary>
    ///     Contacts (parents)
    /// </summary>
    public class Contacts
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private int DefaultContactPersonId;
        private DataTable contactUdfs;
        private string simsudf;

        #region GET
        private int GetDefaultContactPersonId
        {
            get
            {
                if (DefaultContactPersonId == 0)
                {
                    var contacts = new BrowseContacts();
                    contacts.Search("%", "%", "%", "%");

                    foreach (ContactSummary contact in contacts.ContactSummarys)
                    {
                        DefaultContactPersonId = contact.PersonID;
                        //logger.Log(NLog.LogLevel.Debug, "Default ContactID: " + DefaultContactPersonId);
                        return DefaultContactPersonId;
                    }
                }
                return DefaultContactPersonId;
            }
        }

        /// <summary>
        ///     GetContactFilter
        ///     Place holder - SIMS doesn't have a filtering option of contacts :(
        /// </summary>
        public string GetContactFilter
        {
            get { return null; }
        }

        public DataTable GetContactUDFs
        {
            get
            {
                if (contactUdfs == null)
                {
                    contactUdfs = createUdfTable;
                    try
                    {
                        var contactprocess = new EditContact();
                        contactprocess.ReadOnly = true;
                        contactprocess.Populate(new PersonID(GetDefaultContactPersonId));

                        foreach (UDFValue udfVal in contactprocess.Contact.UDFValues)
                        {
                            if (udfVal.FieldTypeCode == UDFFieldType.STRING1_CODE ||
                                udfVal.FieldTypeCode == UDFFieldType.STRINGM_CODE ||
                                udfVal.FieldTypeCode == UDFFieldType.DATE_CODE ||
                                udfVal.FieldTypeCode == UDFFieldType.INTEGER_CODE ||
                                udfVal.FieldTypeCode == UDFFieldType.DOUBLE_CODE ||
                                udfVal.FieldTypeCode == UDFFieldType.DOUBLE2_CODE ||
                                udfVal.FieldTypeCode == UDFFieldType.BOOLEAN_CODE ||
                                udfVal.FieldTypeCode == UDFFieldType.LOOKUP1_CODE
                                )
                            {
                                string type = SIMSAPI.GetFriendlyNameFromUDFFieldType(udfVal.FieldTypeCode);
                                string name = udfVal.TypedValueAttribute.Description;
                                int min = udfVal.TypedValueAttribute.MinLength;
                                int max = udfVal.TypedValueAttribute.MaxLength;
                                logger.Log(LogLevel.Debug, "GetContactUDFs:: " + name + " (" + type + ")");
                                DataRow udfRow = contactUdfs.NewRow();
                                udfRow["Type"] = type;
                                udfRow["Name"] = name;
                                udfRow["Min"] = min;
                                udfRow["Max"] = max;
                                contactUdfs.Rows.Add(udfRow);
                            }
                        }

                        contactprocess.Dispose();
                    }

                    catch (Exception GetContactUDFs_Exception)
                    {
                        logger.Log(LogLevel.Error, GetContactUDFs_Exception);
                    }
                }
                return contactUdfs;
            }
        }

        private DataTable createUdfTable
        {
            get
            {
                var udfTable = new DataTable("SIMS UDFs");
                udfTable.Columns.Add("Type");
                udfTable.Columns.Add("Name");
                udfTable.Columns.Add("Min");
                udfTable.Columns.Add("Max");
                return udfTable;
            }
        }

        public string GetContactPersonID(string surname, string forename, string town, string postCode)
        {
            string personid = "";
            var contacts = new BrowseContacts();
            contacts.Search(surname, forename, town, postCode);
            foreach (ContactSummary contact in contacts.ContactSummarys)
            {
                if (!string.IsNullOrEmpty(personid))
                {
                    personid = personid + ",";
                }
                personid = personid + contact.PersonID;
            }
            return personid;
        }

        public string GetContactEmail(int pid)
        {
            string result = "";
            try
            {
                var contactprocess = new EditContact();
                contactprocess.Populate(new PersonID(pid));

                //logger.Info(pid);

                if (contactprocess.Contact.Emails.Value.Count > 0)
                {
                    int workcount = 1;
                    string emailaddresses = null;

                    //logger.Info("Email count: " + contactprocess.Contact.Emails.Value.Count);

                    foreach (EMail item in contactprocess.Contact.Emails.Value)
                    {
                        //logger.Info(pid + " - " + item.Address.ToLower());
                        if (workcount == 1)
                        {
                            emailaddresses = item.Address.ToLower();
                        }
                        else
                        {
                            emailaddresses = emailaddresses + "," + item.Address.ToLower();
                        }
                        workcount = workcount + 1;
                    }
                    if (string.IsNullOrEmpty(emailaddresses))
                    {
                        return "<NO TYPE EMAIL>";
                    }
                    return emailaddresses;
                }
                return "<BLANK>";
            }
            catch (Exception GetContactEmailException)
            {
                logger.Log(LogLevel.Error, GetContactEmailException);
            }
            return result;
        }

        public string GetContactTelephone(int pid)
        {
            string result = "";
            try
            {
                var contactprocess = new EditContact();
                contactprocess.Populate(new PersonID(pid));

                //logger.Info(pid);

                if (contactprocess.Contact.Phones.Value.Count > 0)
                {
                    int workcount = 1;
                    string telephone = null;

                    //logger.Info("Email count: " + contactprocess.Contact.Emails.Value.Count);

                    foreach (Telephone item in contactprocess.Contact.Phones.Value)
                    {
                        //logger.Info(pid + " - " + item.Address.ToLower());
                        if (workcount == 1)
                        {
                            telephone = item.Number.ToLower();
                        }
                        else
                        {
                            telephone = telephone + "," + item.Number.ToLower();
                        }
                        workcount = workcount + 1;
                    }
                    if (string.IsNullOrEmpty(telephone))
                    {
                        return "<NO TYPE EMAIL>";
                    }
                    return telephone;
                }
                return "<BLANK>";
            }
            catch (Exception GetContactEmailException)
            {
                logger.Log(LogLevel.Error, GetContactEmailException);
            }
            return result;
        }
        #endregion

        #region SET
        public string SetContactUserFilter
        {
            set
            {
                // Contact does not have any user filters
            }
        }

        public string SetContactSIMSUDF
        {
            set { simsudf = value; }
        }

        /// <summary>
        /// SetContactEmail
        /// </summary>
        /// <param name="personid"></param>
        /// <param name="emailValue"></param>
        /// <param name="main"></param>
        /// <param name="primary"></param>
        /// <param name="notes"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool SetContactEmail(Int32 personid, string emailValue, string main, string primary, string notes, string location)
        {
            try
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Classes.Contacts.SetContactEmail(personid=" + personid + ", emailValue=" + emailValue +
                    ", main=" + main + ", primary=" + primary + ", notes=" + notes + ", location=" + location);
                var contactprocess = new EditContact();

                // Load Contact record
                contactprocess.Populate(new PersonID(personid));
                IContactRelation relation = null;
                relation = new StudentContact();
                contactprocess.Contact.ContactRelation = relation;

                // Create a new Email record
                var email = new EMail();

                // Set the email address
                email.AddressAttribute.Value = emailValue;

                // Set Main
                switch (main)
                {
                    case "Yes":
                        email.MainAttribute.Value = (contactprocess.Contact.Emails.Value.Count > 0)
                            ? EMailMainCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : EMailMainCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                    case "Yes (Overwrite)":
                        email.MainAttribute.Value = EMailMainCollection.GetValues().Item(1);
                        break;
                    case "No":
                        email.MainAttribute.Value = EMailMainCollection.GetValues().Item(0);
                        break;
                    default:
                        email.MainAttribute.Value = (contactprocess.Contact.Emails.Value.Count > 0)
                            ? EMailMainCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : EMailMainCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                }

                switch (primary)
                {
                    case "Yes":
                        email.PrimaryAttribute.Value = (contactprocess.Contact.Emails.Value.Count > 0)
                            ? EMailPrimaryCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : EMailPrimaryCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                    case "Yes (Overwrite)":
                        email.PrimaryAttribute.Value = EMailPrimaryCollection.GetValues().Item(1);
                        break;
                    case "No":
                        email.PrimaryAttribute.Value = EMailPrimaryCollection.GetValues().Item(0);
                        break;
                    default:
                        email.PrimaryAttribute.Value = (contactprocess.Contact.Emails.Value.Count > 0)
                            ? EMailPrimaryCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : EMailPrimaryCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                }

                // Set the notes
                if (!string.IsNullOrWhiteSpace(notes))
                    email.NotesAttribute.Value = notes;

                // Set the location
                email.LocationAttribute.Value = PersonCache.EmailLocations.ItemByDescription(location);

                // Run Validation against the new record
                email.Validate();
                logger.Log(LogLevel.Debug, "Email Valid: " + email.Valid());

                // Save the new record to the database
                contactprocess.Contact.Emails.Add(email);
                return contactprocess.Save();
            }
            catch (Exception SetContactEmail_Exception)
            {
                logger.Log(LogLevel.Error, SetContactEmail_Exception);
                return false;
            }
        }

        /// <summary>
        /// SetContactTelephone
        /// </summary>
        /// <param name="personid"></param>
        /// <param name="telephone"></param>
        /// <param name="main"></param>
        /// <param name="primary"></param>
        /// <param name="notes"></param>
        /// <param name="location"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public bool SetContactTelephone(Int32 personid, string telephone, string main, string primary, string notes, string location, string device)
        {
            try
            {
                logger.Log(LogLevel.Debug, "Trace:: SIMSBulkImport.Classes.Contacts.SetContactTelephone(personid=" + personid + ", telephone=" + telephone +
                    ", main=" + main + ", primary=" + primary + ", notes=" + notes + ", location=" + location + ", device=" + device);
                var contactprocess = new EditContact();

                // Load Contact record
                contactprocess.Populate(new PersonID(personid));
                IContactRelation relation = null;
                relation = new StudentContact();
                contactprocess.Contact.ContactRelation = relation;

                // Create a new Email record
                var phone = new Telephone();

                // Set the email address
                phone.NumberAttribute.Value = telephone;


                // Set Main
                switch (main)
                {
                    case "Yes":
                        phone.MainAttribute.Value = (contactprocess.Contact.Phones.Value.Count > 0)
                            ? TelephoneMainCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : TelephoneMainCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                    case "Yes (Overwrite)":
                        phone.MainAttribute.Value = TelephoneMainCollection.GetValues().Item(1);
                        break;
                    case "No":
                        phone.MainAttribute.Value = TelephoneMainCollection.GetValues().Item(0);
                        break;
                    default:
                        phone.MainAttribute.Value = (contactprocess.Contact.Phones.Value.Count > 0)
                            ? TelephoneMainCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : TelephoneMainCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                }

                // Set Primary
                switch (primary)
                {
                    case "Yes":
                        phone.PrimaryAttribute.Value = (contactprocess.Contact.Phones.Value.Count > 0)
                            ? TelephoneMainCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : TelephoneMainCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                    case "Yes (Overwrite)":
                        phone.PrimaryAttribute.Value = TelephoneMainCollection.GetValues().Item(1);
                        break;
                    case "No":
                        phone.PrimaryAttribute.Value = TelephoneMainCollection.GetValues().Item(0);
                        break;
                    default:
                        phone.PrimaryAttribute.Value = (contactprocess.Contact.Phones.Value.Count > 0)
                            ? TelephoneMainCollection.GetValues().Item(1) as CodeDescriptionEntity
                            : TelephoneMainCollection.GetValues().Item(0) as CodeDescriptionEntity;
                        break;
                }

                // Set the notes
                if (!string.IsNullOrWhiteSpace(notes))
                    phone.NotesAttribute.Value = notes;

                // Set the location
                phone.LocationAttribute.Value = PersonCache.TelephoneLocations.ItemByDescription(location);

                // Set the device (telephone\fax)
                phone.DeviceAttribute.Value = PersonCache.TelephoneDevices.ItemByDescription(device);

                // Run Validation against the new record
                phone.Validate();
                logger.Log(LogLevel.Debug, "Telephone Valid: " + phone.Valid());

                // Writes the new record to the database
                contactprocess.Contact.Phones.Add(phone);
                return contactprocess.Save();
            }
            catch (Exception SetContactsTelephone_Exception)
            {
                logger.Log(LogLevel.Error, "SetContactsTelephone " + SetContactsTelephone_Exception);
                return false;
            }
        }

        public bool SetContactUDF(Int32 personid, string udfvalue)
        {
            try
            {
                using (EditContact process = new EditContact())
                {
                    // Load Contact record
                    process.Populate(new PersonID(personid));
                    foreach (UDFValue udfVal in process.Contact.UDFValues)
                    {
                        if (simsudf == udfVal.TypedValueAttribute.Description)
                        {
                            if (Core.SetUdf(udfVal, udfvalue))
                            {
                                // Get a NullReferenceException if there isn't a contact relation. So set a 'null' one so it can save, but doesn't update any relationships.
                                StudentContact relation = new StudentContact(0);
                                relation.RelationType = new RelationType();
                                process.Contact.ContactRelation = relation;
                                return process.Save();
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }

                    logger.Error("UDF {0} not found.", simsudf);
                    return false;
                }
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, "SetContactUDF " + e);
                return false;
            }
        }
        #endregion
    }
}
