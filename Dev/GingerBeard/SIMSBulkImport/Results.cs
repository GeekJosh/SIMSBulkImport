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

        public Results(DataTable resultTable, int importType)
        {
            tmpHtml = getTmpHtmlFileName;

            try
            {
                DataSet ds = new DataSet(getCleanXmlTitle);
                ds.Tables.Add(resultTable);
                ds.Tables[0].TableName = getCleanXmlName(ds.Tables[0].TableName, importType);

                ds.Tables.Add(addPropertiesTable);

                XmlDataDocument xmlDoc = new XmlDataDocument(ds);
                XmlTextWriter writer = new XmlTextWriter(tmpHtml, Encoding.UTF8);
                XslTransform xslTran = new XslTransform();

                xslTran.Load(getXslFileName(importType));
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

        private string getXslFileName(int importType)
        {
            return "Report_" + importTypeToName(importType) + ".xsl";
        }

        private string importTypeToName(int importType)
        {
            switch (importType)
            {
                case 1:
                    return "Staff";
                case 2:
                    return "Pupil";
                case 3:
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

        private string getCleanXmlName(string oldname, int importType)
        {
            string newname;

            if (oldname != "Table1")
            {
                newname = oldname.Replace(" ", "_");
            }
            else
            {
                //newname = importTypeToName(importType) + "_Import_Results";
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
                string tmp = Path.GetTempFileName();
                return Path.Combine(Path.GetDirectoryName(tmp), Path.GetFileNameWithoutExtension(tmp) + ".html");
            }
        }
    }
}
