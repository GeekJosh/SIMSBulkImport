/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Net;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    public class Proxy
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private string _proxy;
        private Uri _uri;

        private bool useProxy
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Proxy.useProxy(GET)");
                IWebProxy proxy = WebRequest.DefaultWebProxy;

                // See what proxy is used for resource.
                Uri resourceProxy = proxy.GetProxy(_uri);

                // Test to see whether a proxy was selected.
                if (resourceProxy == _uri)
                {
                    _proxy = null;
                    logger.Log(LogLevel.Debug, "Proxy: None");
                    return false;
                }
                _proxy = resourceProxy.ToString();
                logger.Log(LogLevel.Debug, "Proxy: " + _proxy);
                return true;
            }
        }

        public string GetProxyAddress
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Proxy.GetProxyAddress(GET)");
                if (useProxy)
                {
                    if (string.IsNullOrWhiteSpace(_proxy))
                    {
                        return null;
                    }
                    string[] proxyPart = _proxy.Split(':');
                    if (proxyPart.Length >= 2)
                        return proxyPart[1].Substring(2);
                }
                return null;
            }
        }

        public string GetProxyPort
        {
            get
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Proxy.GetProxyPort(GET)");
                if (string.IsNullOrWhiteSpace(_proxy))
                {
                    return null;
                }
                string[] proxyPart = _proxy.Split(':');

                if (proxyPart.Length >= 3)
                    return proxyPart[2].Substring(0, proxyPart[2].Length - 1);
                return null;
            }
        }

        public string SetUrl
        {
            set
            {
                logger.Log(LogLevel.Trace, "Trace:: Matt40k.SIMSBulkImport.Proxy.SetUrl(SET: " + value + ")");
                _uri = new Uri(value);
            }
        }

        public WebProxy GetWebProxy(string address, int? port, string user, string pass, bool? useWin)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.Proxy.GetWebProxy(address: " + address + ", port: " + port + ", user: " +
                user + ", pass: " + pass + ", useWin: " + useWin + ")");
            bool _useWin = false;
            int _port = 0;

            if (useWin.HasValue)
                _useWin = (bool) useWin;

            if (port.HasValue)
                _port = (int) port;

            logger.Log(LogLevel.Debug, port + " - " + _port);

            return getWebProxy(address, _port, user, pass, _useWin);
        }

        private WebProxy getWebProxy(string address, int port, string user, string pass, bool useWin)
        {
            logger.Log(LogLevel.Trace,
                "Trace:: Matt40k.SIMSBulkImport.Proxy.GetWebProxy(address: " + address + ", port: " + port + ", user: " +
                user + ", pass: " + pass + ", useWin: " + useWin + ")");
            WebProxy _proxy = null;
            try
            {
                _proxy = new WebProxy(address, port);
                _proxy.UseDefaultCredentials = useWin;

                if (!string.IsNullOrWhiteSpace(user))
                {
                    var nc = new NetworkCredential();

                    string[] userParts = user.Split('\\');
                    if (userParts.Length == 2)
                    {
                        nc.UserName = userParts[0];
                        nc.Domain = userParts[1];
                    }
                    else
                    {
                        nc.UserName = user;
                        nc.Domain = null;
                    }
                    nc.Password = pass;

                    _proxy.Credentials = nc;
                }
            }
            catch (Exception getWebProxy_Exception)
            {
                logger.Log(LogLevel.Error, getWebProxy_Exception);
            }
            return _proxy;
        }
    }
}
