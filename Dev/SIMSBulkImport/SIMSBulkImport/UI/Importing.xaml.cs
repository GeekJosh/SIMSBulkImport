/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for Importing.xaml
    /// </summary>
    public partial class Importing
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly int countImport;
        private int countImported;

        private DateTime endTime;
        private DateTime startTime;

        public Importing()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Importing()");
            InitializeComponent();

            countImported = 0;
            countImport = Switcher.ImportListClass.GetImportCount;
            Switcher.ImportClass = new Import();

            process();
        }

        private void process()
        {
            startTime = DateTime.Now;
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Importing.countImport=" + countImport);
            while (countImported < countImport)
            {
                DataRow row = Switcher.ImportListClass.GetListRow(countImported);
                var type = (string) row["Type"];
                var personID = (Int32) row["PersonID"];
                var value = (string) row["Value"];
                bool result = false;
                string surname = row["Surname"].ToString();
                string forename = row["Forename"].ToString();

                string town = row["Town"].ToString();
                string postCode = row["Postcode"].ToString();
                string gender = row["Gender"].ToString();
                string staffCode = row["Staff Code"].ToString();
                string dob = row["Date of Birth"].ToString();
                string year = row["Year"].ToString();
                string registration = row["Registration"].ToString();
                string house = row["House"].ToString();
                string admissionNumber = row["Admission Number"].ToString();
                string title = row["Title"].ToString();

                switch (type)
                {
                    case "Email":
                        result = Switcher.ImportClass.SetEmail(personID, value);
                        logger.Log(LogLevel.Trace, "Set Email: " + result + " - " + personID + " - " + value);
                        Switcher.ResultsImportClass.AddToResultsTable(personID.ToString(), result.ToString(), "Email",
                            value, null, surname, forename, title, gender, staffCode, dob, admissionNumber, year,
                            registration, house, postCode, town);


                        //null, forename, surname, town, postCode, gender, staffCode, dob, year, registration, house, admissionNumber);
                        break;
                    case "Telephone":
                        result = Switcher.ImportClass.SetTelephone(personID, value);
                        logger.Log(LogLevel.Trace, "Set Telephone: " + result + " - " + personID + " - " + value);
                        Switcher.ResultsImportClass.AddToResultsTable(personID.ToString(), result.ToString(),
                            "Telephone", value, null, surname, forename, title, gender, staffCode, dob, admissionNumber,
                            year, registration, house, postCode, town);
                        break;
                    case "UDF":
                        result = Switcher.ImportClass.SetUDF(personID, value);
                        logger.Log(LogLevel.Trace, "Set UDF: " + result + " - " + personID + " - " + value);
                        Switcher.ResultsImportClass.AddToResultsTable(personID.ToString(), result.ToString(), "UDF",
                            value, null, surname, forename, title, gender, staffCode, dob, admissionNumber, year,
                            registration, house, postCode, town);
                        break;
                    default:
                        result = false;
                        logger.Log(LogLevel.Error, "process: type not defined - " + type);
                        break;
                }
                // Add to result table

                countImported++;
            }
            endTime = DateTime.Now;
            progressRing.IsActive = false;

            // Import complete return to menu
            Switcher.Switch(new Menu());

            // Open report
            Switcher.ResultsImportClass.OpenResultsReport();

            // TMP - Close application
            Environment.Exit(0);
        }
    }
}