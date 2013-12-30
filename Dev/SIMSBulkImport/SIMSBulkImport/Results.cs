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

        public Results(DataTable resultTable, Interfaces.UserType userType)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Results.useProxy(resultTable: " + resultTable + ", userType: " + userType + ")");
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
            catch (Exception Results_Exception)
            {
                logger.Log(NLog.LogLevel.Error, Results_Exception);
            }

            openReportHtml();
        }

        private DataTable addPropertiesTable
        {
            get
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Results.addPropertiesTable(GET)");
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

        private string getXslFileName(Interfaces.UserType userType)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Results.getXslFileName(userType : " + userType + ")");
            return "Report_" + importTypeToName(userType) + ".xsl";
        }

        private string importTypeToName(Interfaces.UserType userType)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Results.importTypeToName(userType : " + userType + ")");
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
            
        private string getCleanXmlTitle
        {
            get
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Results.getCleanXmlTitle(GET)");
                logger.Log(NLog.LogLevel.Debug, GetExe.Title.Replace(" ", "_"));
                return GetExe.Title.Replace(" ", "_");
            }
        }

        private string getCleanXmlName(string oldname, Interfaces.UserType userType)
        {
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Results.getCleanXmlName(oldname: " + oldname + ", userType : " + userType + ")");
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
            logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Results.openReportHtml()");
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = tmpHtml;
                process.Start();
            }
            catch (Exception openReportHtml_Exception)
            {
                logger.Log(NLog.LogLevel.Error, openReportHtml_Exception);
            }
        }

        private string getTmpHtmlFileName
        {
            get
            {
                logger.Log(NLog.LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Results.getTmpHtmlFileName(GET)");
                return Path.ChangeExtension(TempFile.GetNewTempFile, ".html");
            }
        }
    }
}
