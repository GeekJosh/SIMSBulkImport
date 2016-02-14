using System.Data;
using NLog;

namespace SIMSBulkImport.Support
{
    public static class Submit
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static Requestor _requestor;
        private static Proxy _proxy;
        private static DataTable logSubResults;


        public static DataTable Logs(string email, string log)
        {
            logger.Log(LogLevel.Debug,
                "Trace:: SIMSBulkImport.Support.Submit.Logs(email: " + email + ", log: " + log + ")");
            _requestor = new Requestor();
            _proxy = new Proxy();
            //_proxy.SetUrl = ConfigMan.UpdateUrl;
            //_requestor.SetApiUrl = ConfigMan.UpdateUrl;

            return logSubResults = _requestor.SubmitLog(email, log);
        }
    }
}