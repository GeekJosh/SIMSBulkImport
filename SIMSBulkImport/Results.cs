using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    internal class Results
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string tmpHtml;

        public Results(DataTable resultTable, Interfaces.UserType userType)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.Results.useProxy(resultTable: " + resultTable + ", userType: " +
                userType + ")");
            tmpHtml = getTmpHtmlFileName;

            try
            {
                XslCompiledTransform transform = LoadTransform(userType);

                using (XmlReader inputData = LoadInputData(resultTable, userType))
                {
                    using (XmlWriter writer = new XmlTextWriter(tmpHtml, Encoding.UTF8))
                    {
                        transform.Transform(inputData, null, writer);
                    }
                }
            }
            catch (Exception Results_Exception)
            {
                logger.Log(LogLevel.Error, Results_Exception);
            }

            openReportHtml();
        }

        private XmlReader LoadInputData(DataTable resultTable, Interfaces.UserType userType)
        {
            var ds = new DataSet(getCleanXmlTitle);
            ds.Tables.Add(resultTable);
            ds.Tables[0].TableName = getCleanXmlName(ds.Tables[0].TableName, userType);
            ds.Tables.Add(addPropertiesTable);


            StringWriter data = new StringWriter();
            using (XmlWriter loadWriter = XmlWriter.Create(data))
            {
                ds.WriteXml(loadWriter);
            }

            return XmlReader.Create(new StringReader(data.ToString()));
        }

        private XslCompiledTransform LoadTransform(Interfaces.UserType userType)
        {
            XslCompiledTransform transform = new XslCompiledTransform();
            using (XmlReader reader = XmlReader.Create(getXslFileName(userType)))
            {
                transform.Load(reader);
            }

            return transform;
        }

        private DataTable addPropertiesTable
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Results.addPropertiesTable(GET)");
                var propertiesTable = new DataTable("Properties");
                propertiesTable.Columns.Add(new DataColumn("Title", typeof (string)));
                propertiesTable.Columns.Add(new DataColumn("Version", typeof (string)));
                propertiesTable.Columns.Add(new DataColumn("Copyright", typeof (string)));
                propertiesTable.Columns.Add(new DataColumn("Date", typeof (string)));

                DataRow newrow = propertiesTable.NewRow();
                newrow["Title"] = GetExe.Title;
                newrow["Version"] = GetExe.Version;
                newrow["Copyright"] = GetExe.Copyright;
                newrow["Date"] = DateTime.Now.ToShortDateString();
                propertiesTable.Rows.Add(newrow);
                return propertiesTable;
            }
        }

        private string getCleanXmlTitle
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Results.getCleanXmlTitle(GET)");
                logger.Log(LogLevel.Debug, GetExe.Title.Replace(" ", "_"));
                return GetExe.Title.Replace(" ", "_");
            }
        }

        private string getTmpHtmlFileName
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Results.getTmpHtmlFileName(GET)");
                return Path.ChangeExtension(TempFile.GetNewTempFile, ".html");
            }
        }

        private string getXslFileName(Interfaces.UserType userType)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.Results.getXslFileName(userType : " + userType + ")");
            return "Report_" + importTypeToName(userType) + ".xsl";
        }

        private string importTypeToName(Interfaces.UserType userType)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.Results.importTypeToName(userType : " + userType + ")");
            switch (userType)
            {
                case Interfaces.UserType.Staff:
                    return "Staff";
                case Interfaces.UserType.Pupil:
                    return "Pupil";
                case Interfaces.UserType.Contact:
                    return "Contact";
                default:
                    return "";
            }
        }

        private string getCleanXmlName(string oldname, Interfaces.UserType userType)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.Results.getCleanXmlName(oldname: " + oldname + ", userType : " +
                userType + ")");
            string newname;

            if (oldname != "Table1")
            {
                newname = oldname.Replace(" ", "_");
            }
            else
            {
                //newname = importTypeToName(userType) + "_Import_Results";
                newname = "Pupil_Import_Results";
            }
            logger.Log(LogLevel.Debug, newname);
            return newname;
        }

        private void openReportHtml()
        {
            logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Results.openReportHtml()");
            try
            {
                var process = new Process();
                process.StartInfo.FileName = tmpHtml;
                process.Start();
            }
            catch (Exception openReportHtml_Exception)
            {
                logger.Log(LogLevel.Error, openReportHtml_Exception);
            }
        }
    }
}
