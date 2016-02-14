using System.Management.Automation;

namespace SIMSBulkImport.PowerShell
{
    [Cmdlet("Get", "SBIVersion", SupportsShouldProcess = false)]
    public class Get_SBIVersion : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(string.Format("{0}", GetExe.Version));
        }

    }
}
