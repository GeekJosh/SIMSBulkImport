using System.Management.Automation;

namespace SIMSBulkImport.PowerShell
{
    public class Core
    {
        private static Matt40k.SIMSBulkImport.SIMSAPI simsapi;

        [Cmdlet("Connect", "SBI", SupportsShouldProcess = false)]
        public class ConnectSBI : PSCmdlet
        {
            [Parameter(Mandatory = false, Position = 0, ParameterSetName = ParameterAttribute.AllParameterSets, ValueFromPipeline = true, HelpMessage = "SIMS .net username")]
            public string Username;

            [Parameter(Mandatory = false, Position = 1, ParameterSetName = ParameterAttribute.AllParameterSets, ValueFromPipeline = true, HelpMessage = "SIMS .net password")]
            public string Password;

            [Parameter(Mandatory = false, Position = 2, ParameterSetName = ParameterAttribute.AllParameterSets, ValueFromPipeline = true, HelpMessage = "SIMS .net application directory")]
            public string SIMSAppDir;

            protected override void ProcessRecord()
            {
                // Get the SIMS Application directory - this is read from %WinDir%\SIMS.ini, normally C:\Windows\SIMS.ini
                if (string.IsNullOrEmpty(SIMSAppDir))
                    SIMSAppDir = SimsIni.GetSimsDir;
                simsapi = new Matt40k.SIMSBulkImport.SIMSAPI(SIMSAppDir);

                // If no Username is passed then assume we're using Trusted logins
                if (!string.IsNullOrEmpty(Username))
                {
                    simsapi.SetSimsUser = Username;
                    simsapi.SetSimsPass = Password;
                    simsapi.SetIsTrusted = false;
                }
                else
                {
                    simsapi.SetIsTrusted = true;
                }

                // Actually try and connect
                bool result = simsapi.Connect;
            }
        }
    }
}
 