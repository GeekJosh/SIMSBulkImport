/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

namespace Matt40k.SIMSBulkImport.Updater
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
