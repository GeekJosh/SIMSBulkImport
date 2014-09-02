/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Net;

namespace Matt40k.SIMSBulkImport.Updater
{
    public class Proxy
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private string _proxy;
        private Uri _uri;

        private bool useProxy
        {
            get
            {
                WebProxy proxy = (WebProxy)WebProxy.GetDefaultProxy();

                // See what proxy is used for resource.
                Uri resourceProxy = proxy.GetProxy(_uri);

                // Test to see whether a proxy was selected.
                if (resourceProxy == _uri)
                {
                    _proxy = null;
                    logger.Log(NLog.LogLevel.Debug, "Proxy: None");
                    return false;
                }
                else
                {
                    _proxy = resourceProxy.ToString();
                    logger.Log(NLog.LogLevel.Debug, "Proxy: " + _proxy);
                    return true;
                }
            }
        }

        public string GetProxyAddress
        {
            get
            {
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
                _uri = new Uri(value);
            }
        }

        public WebProxy GetWebProxy(string address, int? port, string user, string pass, bool? useWin)
        {
            bool _useWin = false;
            int _port = 0;

            if (useWin.HasValue)
                _useWin = (bool)useWin;

            if (port.HasValue)
                _port = (int)port;

            logger.Log(NLog.LogLevel.Debug, port + " - " + _port);

            return getWebProxy(address, _port, user, pass, _useWin);
        }

        private WebProxy getWebProxy(string address, int port, string user, string pass, bool useWin)
        {
            logger.Log(NLog.LogLevel.Debug, "Address: " + address + " - Port: " + port);
            WebProxy _proxy = null;
            try
            {
                _proxy = new WebProxy(address, port);
                _proxy.UseDefaultCredentials = useWin;

                if (!string.IsNullOrWhiteSpace(user))
                {
                    NetworkCredential nc = new NetworkCredential();

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
                logger.Log(NLog.LogLevel.Error, getWebProxy_Exception);
            }
            return _proxy;
        }
    }
}