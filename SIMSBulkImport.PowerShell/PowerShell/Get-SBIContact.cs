using System;
using System.Management.Automation;
using System.Runtime.Serialization;

namespace SIMSBulkImport.PowerShell
{
    /// <summary>
    /// Returns contacts in SIMS .net
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SBIContact", SupportsShouldProcess = false)]
    public class Get_SBIContact : PSCmdlet
    {
        /// <summary>
        /// Filter by SIMS system ID (optional)
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = ParameterAttribute.AllParameterSets, ValueFromPipeline = true, HelpMessage = "SIMS Person (system) ID")]
        public string PersonID;

        protected override void ProcessRecord()
        {
            Contact _pupils = new Contact();
 

            WriteObject(_pupils, true);
        }

        [Serializable]
        public class Contact
        {
            /// <summary>
            /// SIMS system ID
            /// </summary>
            [DataMember]
            public int? PersonID { get; set; }

            /// <summary>
            /// Forename
            /// </summary>
            [DataMember]
            public string Forename { get; set; }

            /// <summary>
            /// Surname
            /// </summary>
            [DataMember]
            public string Surname { get; set; }

        }
    }
}