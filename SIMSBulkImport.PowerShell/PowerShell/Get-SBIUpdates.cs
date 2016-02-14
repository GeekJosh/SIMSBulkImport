using System.Management.Automation;

namespace SIMSBulkImport.PowerShell
{
    [Cmdlet("Get", "SBIUpdates", SupportsShouldProcess = false)]
    public class Get_SBIUpdates : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(string.Format("{0}", GetExe.Version));
        }

    }
}
