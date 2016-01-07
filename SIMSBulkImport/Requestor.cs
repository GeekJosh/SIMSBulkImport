using System;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using NLog;
using Matt40k.SIMSBulkImport.Updater;

namespace Matt40k.SIMSBulkImport
{
    public class Requestor
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string _appGUID = Switcher.ConfigManClass.GetAppGUID;
        private readonly string _appVersion = GetExe.Version;

        private string _apiUrl = "https://simsbulkimport.uk" ;
        private int _appID = 1;
        private WebProxy _proxy;
        private bool useProxy;

        public bool NeedUpdate
        {
            get
            {
                logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Requestor.GetVersion(GET)");
                var ds = new DataSet("simsbulkimport");
                string jsonResp = null;
                var url = new Uri(_apiUrl + "/versions/" + GetExe.Version);
                HttpWebResponse response = request(url, "GET", null);


                logger.Log(LogLevel.Debug,
                    "StatusCode: " + response.StatusCode + " - StatusDescription: " + response.StatusDescription);




                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    jsonResp = reader.ReadToEnd();
                    // BUGFIX: For some stupid reason it returns with [] which Newtonsoft.Json doesn't like.
                    if (jsonResp.Length > 2)
                        jsonResp = jsonResp.Substring(1, (jsonResp.Length - 2));
                }
                logger.Log(LogLevel.Debug, "ReturnData :: " + jsonResp);

                var results = JsonConvert.DeserializeObject<CheckResp>(jsonResp);
                switch (results.current)
                {
                    case "0":
                        return true;
                    case "1":
                        return false;
                    default:
                        // Default\failed = no updated needed
                        return false;
                }
            }
        }

        public string SetApiUrl
        {
            set
            {
                logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Requestor.SetApiUrl(SET: " + value + ")");
                if (!string.IsNullOrEmpty(value))
                    _apiUrl = value;
            }
        }

        public WebProxy SetWebProxy
        {
            set
            {
                logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Requestor.SetWebProxy(SET: " + value + ")");
                _proxy = value;
            }
        }

        public bool UseProxy
        {
            set
            {
                logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Requestor.UseProxy(SET: " + value + ")");
                useProxy = value;
            }
        }

        private HttpWebResponse request(Uri url, string method, string postData)
        {
            logger.Log(LogLevel.Debug,
                "Trace:: Matt40k.SIMSBulkImport.Requestor.request(url: " + url + ", method: " + method + ", postData: " +
                postData + ")");
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.UserAgent = GetExe.Title + "\\" + GetExe.Version;
            StreamWriter requestWriter;
            request.Method = method;
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _appGUID;
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
            return (HttpWebResponse) request.GetResponse();
        }

        public DataTable SubmitLog(string email, string log)
        {
            logger.Log(LogLevel.Debug,
                "Trace:: Matt40k.SIMSBulkImport.Requestor.SubmitLog(email: " + email + ", log: " + log + ")");
            var ds = new DataSet("simsbulkimport");
            string result = null;
            var url = new Uri(_apiUrl + "log");
            string postData = "appid=" + _appID + "&guid=" + _appGUID + "&email=" + email + "&log=" + log;
            //logger.Log(NLog.LogLevel.Debug, "PostData :: " + postData);
            HttpWebResponse response = request(url, "POST", postData);
            logger.Log(LogLevel.Debug,
                "StatusCode: " + response.StatusCode + " - StatusDescription: " + response.StatusDescription);

            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            logger.Log(LogLevel.Debug, "ReturnData :: " + result);

            XmlDocument doc = JsonConvert.DeserializeXmlNode(result, "simsbulkimport");
            XmlReader xmlReader = new XmlNodeReader(doc);
            ds.ReadXml(xmlReader);

            if (ds.Tables.Contains("simsbulkimport"))
                return ds.Tables["simsbulkimport"];

            return null;
        }
    }
}