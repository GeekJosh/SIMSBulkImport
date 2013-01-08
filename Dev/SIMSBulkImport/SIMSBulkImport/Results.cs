/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace Matt40k.SIMSBulkImport
{
    class Results
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private string tmpHtml;

        public Results(DataTable resultTable, SIMSAPI.UserType userType)
        {
            tmpHtml = getTmpHtmlFileName;

            try
            {
                DataSet ds = new DataSet(getCleanXmlTitle);
                ds.Tables.Add(resultTable);
                ds.Tables[0].TableName = getCleanXmlName(ds.Tables[0].TableName, userType);

                ds.Tables.Add(addPropertiesTable);

                XmlDataDocument xmlDoc = new XmlDataDocument(ds);
                XmlTextWriter writer = new XmlTextWriter(tmpHtml, Encoding.UTF8);
                XslTransform xslTran = new XslTransform();

                xslTran.Load(getXslFileName(userType));
                xslTran.Transform(xmlDoc, null, writer);
            }
            catch (Exception ResultsException)
            {
                logger.Log(NLog.LogLevel.Error, ResultsException);
            }

            openReportHtml();
        }

        private DataTable addPropertiesTable
        {
            get
            {
                DataTable propertiesTable = new DataTable("Properties");
                propertiesTable.Columns.Add(new DataColumn("Title", typeof(string)));
                propertiesTable.Columns.Add(new DataColumn("Version", typeof(string)));
                propertiesTable.Columns.Add(new DataColumn("Copyright", typeof(string)));
                propertiesTable.Columns.Add(new DataColumn("Date", typeof(string)));

                DataRow newrow = propertiesTable.NewRow();
                newrow["Title"] = GetExe.Title;
                newrow["Version"] = GetExe.Version;
                newrow["Copyright"] = GetExe.Copyright;
                newrow["Date"] = DateTime.Now.ToShortDateString();
                propertiesTable.Rows.Add(newrow);
                return propertiesTable;
            }
        }

        private string getXslFileName(SIMSAPI.UserType userType)
        {
            return "Report_" + importTypeToName(userType) + ".xsl";
        }

        private string importTypeToName(SIMSAPI.UserType userType)
        {
            switch (userType)
            {
                case SIMSAPI.UserType.Staff:
                    return "Staff";
                case SIMSAPI.UserType.Pupil:
                    return "Pupil";
                case SIMSAPI.UserType.Contact:
                    return "Contact";
                default:
                    return "";
            }
        }
            
        private string getCleanXmlTitle
        {
            get
            {
                logger.Log(NLog.LogLevel.Debug, GetExe.Title.Replace(" ", "_"));
                return GetExe.Title.Replace(" ", "_");
            }
        }

        private string getCleanXmlName(string oldname, SIMSAPI.UserType userType)
        {
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
            logger.Log(NLog.LogLevel.Debug, newname);
            return newname;
            
        }

        private void openReportHtml()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = tmpHtml;
                process.Start();
            }
            catch (Exception openReportHtmlException)
            {
                logger.Log(NLog.LogLevel.Error, openReportHtmlException);
            }
        }

        private string getTmpHtmlFileName
        {
            get
            {
                return Path.ChangeExtension(TempFile.GetNewTempFile, ".html");
            }
        }
    }
}
