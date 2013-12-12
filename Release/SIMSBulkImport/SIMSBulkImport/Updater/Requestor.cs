/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Data;
using System.IO;
using System.Net;
using NLog;

namespace Matt40k.SIMSBulkImport.Updater
{
    public class Requestor
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private string _apiUrl;
        private WebProxy _proxy;
        private bool useProxy;

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
                Uri url = new Uri(_apiUrl + "simsbulkimport_v1.xml");
                HttpWebResponse response = request(url, "GET", null);
                logger.Log(NLog.LogLevel.Debug, response.StatusCode + " - " + response.StatusDescription);
                ds.ReadXml(response.GetResponseStream());

                if (ds.Tables.Contains("simsbulkimport"))
                    return ds.Tables["simsbulkimport"];

                return null;
            }
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
