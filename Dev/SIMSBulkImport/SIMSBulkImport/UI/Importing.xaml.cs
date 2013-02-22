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

                switch (type)
                {
                    case "Email":
                        break;
                    case "Telephone":
                        break;
                    case "UDF":
                        break;
                    default:
                        logger.Log(NLog.LogLevel.Error, "process: type not defined - " + type);
                        break;
                }

                countImported++;
            }
        }
    }
}
