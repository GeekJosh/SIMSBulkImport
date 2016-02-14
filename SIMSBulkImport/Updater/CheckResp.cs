namespace SIMSBulkImport.Updater
{
    public class CheckResp
    {
        public string id { get; set; }
        public string version { get; set; }
        public string current { get; set; }
        public string notes { get; set; }
        public string download_url { get; set; }
    }
}
