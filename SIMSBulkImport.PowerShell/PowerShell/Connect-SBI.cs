using System;
using System.Management.Automation;
using SIMSBulkImport.Core;

namespace SIMSBulkImport.PowerShell
{
    [Cmdlet("Connect", "SBI", SupportsShouldProcess = false)]
    public class ConnectSBI : PSCmdlet
    {
        /// <summary>
        /// SIMS Username
        /// Leave undefiend for Windows Authentication
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = ParameterAttribute.AllParameterSets, ValueFromPipeline = true, HelpMessage = "SIMS .net username")]
        public string Username;

        /// <summary>
        /// SIMS Password
        /// Not required for Windows Authentication
        /// </summary>
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = ParameterAttribute.AllParameterSets, ValueFromPipeline = true, HelpMessage = "SIMS .net password")]
        public string Password;

        /// <summary>
        /// SIMS Application Directory
        /// If not defined then the path set in the (c:\windows\) SIMS.ini file will be used
        /// </summary>
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = ParameterAttribute.AllParameterSets, ValueFromPipeline = true, HelpMessage = "SIMS .net application directory")]
        public string SIMSAppDir;

        protected override void ProcessRecord()
        {
            string _simsAppDir = null;
            // Get the SIMS Application directory - this is read from %WinDir%\SIMS.ini, normally C:\Windows\SIMS.ini
            if (string.IsNullOrEmpty(_simsAppDir))
                _simsAppDir = SimsIni.GetSimsDir;
            WriteVerbose("SIMS Application directory: " + _simsAppDir);

            bool result;

            // Actually try and connect
            try
            {
                Core.SimsApi = new SIMSAPI(_simsAppDir);

                // If no Username is passed then assume we're using Trusted logins
                if (!string.IsNullOrEmpty(Username))
                {
                    Core.SimsApi.SetSimsUser = Username;
                    Core.SimsApi.SetSimsPass = Password;
                }

                result = Core.SimsApi.Connect;
                WriteObject(Core.SimsApi.GetConnectError);
            }
            catch (Exception ConnectSBI_ProcessRecord_Exception)
            {
                result = false;
                WriteVerbose(ConnectSBI_ProcessRecord_Exception.ToString());
                WriteError(new ErrorRecord(ConnectSBI_ProcessRecord_Exception, "ConnectFailed", ErrorCategory.OpenError, "Connect"));
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