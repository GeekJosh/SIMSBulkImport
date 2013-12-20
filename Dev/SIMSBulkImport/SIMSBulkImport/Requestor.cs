﻿/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    public class Requestor
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private string _apiUrl = "http://api.matt40k.co.uk/";
        private WebProxy _proxy;
        private bool useProxy;

        private int _appID = 1;
        private string _appVersion = GetExe.Version;
        private string _appGUID = "2b36ee34-8341-4ff2-ab7b-90f472ff51bc";

        private HttpWebResponse request(Uri url, string method, string postData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = GetExe.Title + "\\" + GetExe.Version;
            StreamWriter requestWriter;
            request.Method = method;
            request.ContentType = "application/x-www-form-urlencoded";
            if (useProxy)
            {
                request.Proxy = _proxy;
            }
            else
            {
                request.Proxy = null;
            }
            if (!string.IsNullOrEmpty(postData))
            {
                using (requestWriter = new StreamWriter(request.GetRequestStream()))
                {
                    requestWriter.Write(postData);
                }
            }
            return (HttpWebResponse)request.GetResponse();
        }

        public DataTable GetVersion
        {
            get
            {
                DataSet ds = new DataSet("simsbulkimport");
                string result = null;
                Uri url = new Uri(_apiUrl + "version");
                string postData = "appid=" + _appID .ToString() + "&guid=" + _appGUID + "&version=" + _appVersion;
                logger.Log(NLog.LogLevel.Trace, "PostData :: " + postData);
                HttpWebResponse response = request(url, "POST", postData);
                logger.Log(NLog.LogLevel.Debug, response.StatusCode + " - " + response.StatusDescription);

                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                logger.Log(NLog.LogLevel.Trace, "ReturnData :: " + result);

                XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(result, "simsbulkimport");
                XmlReader xmlReader = new XmlNodeReader(doc);
                ds.ReadXml(xmlReader);

                if (ds.Tables.Contains("simsbulkimport"))
                    return ds.Tables["simsbulkimport"];

                return null;
            }
        }

        public DataTable SubmitLog(string email, string log)
        {
            DataSet ds = new DataSet("simsbulkimport");
            string result = null;
            Uri url = new Uri(_apiUrl + "log");
            string postData = "appid=" + _appID.ToString() + "&guid=" + _appGUID + "&email=" + email + "&log=" + log;
            logger.Log(NLog.LogLevel.Trace, "PostData :: " + postData);
            HttpWebResponse response = request(url, "POST", postData);
            logger.Log(NLog.LogLevel.Debug, response.StatusCode + " - " + response.StatusDescription);

            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            logger.Log(NLog.LogLevel.Trace, "ReturnData :: " + result);

            XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(result, "simsbulkimport");
            XmlReader xmlReader = new XmlNodeReader(doc);
            ds.ReadXml(xmlReader);

            if (ds.Tables.Contains("simsbulkimport"))
                return ds.Tables["simsbulkimport"];

            return null;
        }

        public string SetApiUrl
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _apiUrl = value;
            }
        }

        public WebProxy SetWebProxy
        {
            set
            {
                _proxy = value;
            }
        }

        public bool UseProxy
        {
            set
            {
                useProxy = value;
            }
        }
    }
}

