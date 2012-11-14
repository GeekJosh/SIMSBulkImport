namespace GingerBeard
{
    public class License
    {
        string licensee_la = "873";
        string licensee_sch = "4604";
        string licenseeName = "Thomas Clarkson Community College";

        public bool IsLicensed(string leacode, string schcode)
        {
            /* LA License
             * ==========
            if (leacode == licensee_la) { return true; }
             */

            /* School License
             * ============== */
            if (leacode == licensee_la) { if (schcode == licensee_sch) { return true; } }
            return false;
        }

        public string DisplayLicense
        {
            get
            {
                /* LA License
                 * ==========
                return "Licensed to: "+ licenseeName +" (" + licensee + ") LA schools only";
                 */

                /* School License
                * ============== */
                return "Licensed to: " + licenseeName + " (" + licensee_la + licensee_sch + ") only";
            }
        }
    }
}


