using System;
using System.Management.Automation;
using System.Runtime.Serialization;

namespace SIMSBulkImport.PowerShell
{
    /// <summary>
    /// Returns staff in SIMS .net
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SBIStaff", SupportsShouldProcess = false)]
    public class Get_SBIStaff : PSCmdlet
    {
        /// <summary>
        /// Filter by SIMS system ID (optional)
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = ParameterAttribute.AllParameterSets, ValueFromPipeline = true, HelpMessage = "SIMS Person (system) ID")]
        public string PersonID;

        protected override void ProcessRecord()
        {
            Staff _pupils = new Staff();
 

            WriteObject(_pupils, true);
        }

        [Serializable]
        public class Staff
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

            /// <summary>
            /// Gender
            /// </summary>
            [DataMember]
            public string Gender { get; set; }

            /// <summary>
            /// Date of birth
            /// </summary>
            [DataMember]
            public string DOB { get; set; }
        }
    }
}