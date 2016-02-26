using System.Management.Automation;

namespace SIMSBulkImport.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "SBIUpdate", SupportsShouldProcess = false)]
    public class Get_SBIUpdates : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(string.Format("{0}", GetExe.Version));
        }

    }
}
