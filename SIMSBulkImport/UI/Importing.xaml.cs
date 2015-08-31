using System;
using System.Data;
using System.Windows;
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


        /// <summary>
        /// 
        /// </summary>
        public Importing()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Importing()");
            InitializeComponent();

            countImported = 0;
            countImport = Switcher.ImportListClass.GetImportCount;
            Switcher.ImportClass = new Import();

            process();
        }

        /// <summary>
        /// 
        /// </summary>
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
                string main = row["Main"].ToString();
                string primary = row["Primary"].ToString();
                string notes =  row["Notes"].ToString();
                string location =  row["Location"].ToString();
                string device = row["Device"].ToString();

                switch (type)
                {
                    case "Email":
                        result = Switcher.ImportClass.SetEmail(personID, value, main, primary, notes, location);
                        logger.Log(LogLevel.Trace, "Set Email: " + result + " - " + personID + " - " + value);
                        Switcher.ResultsImportClass.AddToResultsTable(personID.ToString(), result.ToString(), "Email",
                            value, null, surname, forename, title, gender, staffCode, dob, admissionNumber, year,
                            registration, house, postCode, town, main, primary, location, null);
                        break;
                    case "Telephone":
                        result = Switcher.ImportClass.SetTelephone(personID, value, main, primary, notes, location, device);
                        logger.Log(LogLevel.Trace, "Set Telephone: " + result + " - " + personID + " - " + value);
                        Switcher.ResultsImportClass.AddToResultsTable(personID.ToString(), result.ToString(),
                            "Telephone", value, null, surname, forename, title, gender, staffCode, dob, admissionNumber,
                            year, registration, house, postCode, town, main, primary, location, device);
                        break;
                    case "UDF":
                        result = Switcher.ImportClass.SetUDF(personID, value);
                        logger.Log(LogLevel.Trace, "Set UDF: " + result + " - " + personID + " - " + value);
                        Switcher.ResultsImportClass.AddToResultsTable(personID.ToString(), result.ToString(), "UDF",
                            value, null, surname, forename, title, gender, staffCode, dob, admissionNumber, year,
                            registration, house, postCode, town, null, null, null, null);
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

            logger.Log(LogLevel.Info, "Import complete");

            ButtonAnother.Visibility = Visibility.Visible;

            // Open report
            logger.Log(LogLevel.Trace, "Open result report");
            Switcher.ResultsImportClass.OpenResultsReport();
        }

        private void ButtonAnother_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Import complete return to menu
            logger.Log(LogLevel.Trace, "Return to menu");
            Switcher.Switch(new Menu());
        }
    }
}