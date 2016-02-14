using System;
using System.Management.Automation;
using SIMSBulkImport.Core;

namespace SIMSBulkImport.PowerShell
{
    public class Core
    {
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
                SIMSBulkImport.SIMSAPI simsapi;
                string _simsAppDir = null;
                // Get the SIMS Application directory - this is read from %WinDir%\SIMS.ini, normally C:\Windows\SIMS.ini
                if (string.IsNullOrEmpty(_simsAppDir))
                    _simsAppDir = SimsIni.GetSimsDir;
                WriteObject(_simsAppDir);
                simsapi = new SIMSAPI(_simsAppDir);

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

                bool result;

                // Actually try and connect
                try
                {
                    result = simsapi.Connect;
                }
                catch (Exception ConnectSBI_ProcessRecord_Exception)
                {
                    result = false;
                    WriteError(new ErrorRecord(ConnectSBI_ProcessRecord_Exception, "ConnectFailed", ErrorCategory.ConnectionError, "Connect"));
                }

                if (result)
                    WriteObject("Connection successed to SIMS .net");
                else
                {
                    WriteObject("Connection failed to SIMS .net");
                }
            }
        }
    }
}
 