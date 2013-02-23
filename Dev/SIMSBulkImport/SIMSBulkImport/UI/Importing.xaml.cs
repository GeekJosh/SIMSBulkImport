/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    /// Interaction logic for Importing.xaml
    /// </summary>
    public partial class Importing
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private int countImport;
        private int countImported;

        public Importing()
        {
            InitializeComponent();

            countImported = 0;
            countImport = Switcher.ImportListClass.GetImportCount;
            Switcher.ImportClass = new Import();

            process();
        }

        private void process()
        {
            while (countImported < countImport)
            {
                DataRow row = Switcher.ImportListClass.GetListRow(countImported);
                string type = (string)row["Type"];
                Int32 personID = (Int32)row["PersonID"];
                string value = (string)row["Value"];
                bool result = false;

                switch (type)
                {
                    case "Email":
                        result = Switcher.ImportClass.SetEmail(personID, value);
                        logger.Log(NLog.LogLevel.Debug, "Set Email: " + result + " - " + personID + " - " + value);
                        break;
                    case "Telephone":
                        result = Switcher.ImportClass.SetTelephone(personID, value);
                        logger.Log(NLog.LogLevel.Debug, "Set Telephone: " + result + " - " + personID + " - " + value);
                        break;
                    case "UDF":
                        result = Switcher.ImportClass.SetUDF(personID, value);
                        logger.Log(NLog.LogLevel.Debug, "Set UDF: " + result + " - " + personID + " - " + value);
                        break;
                    default:
                        result = false;
                        logger.Log(NLog.LogLevel.Error, "process: type not defined - " + type);
                        break;
                }

                // Add to result table

                countImported++;
            }
        }
    }
}
