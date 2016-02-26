using System;
using System.Management.Automation;
using System.Runtime.Serialization;

namespace SIMSBulkImport.PowerShell
{
    /// <summary>
    /// Returns pupils in SIMS .net
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SBIPupil", SupportsShouldProcess = false)]
    public class Get_SBIPupil : PSCmdlet
    {
        /// <summary>
        /// Filter by SIMS system ID (optional)
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = ParameterAttribute.AllParameterSets, ValueFromPipeline = true, HelpMessage = "SIMS Person (system) ID")]
        public string PersonID;

        protected override void ProcessRecord()
        {
            Pupil _pupils = new Pupil();
 

            WriteObject(_pupils, true);
        }

        [Serializable]
        public class Pupil
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

            /// <summary>
            /// Admission number
            /// </summary>
            [DataMember]
            public string AdmissionNumber { get; set; }

            /// <summary>
            /// Admission date
            /// </summary>
            [DataMember]
            public string AdmissionDate { get; set; }

            /// <summary>
            /// Registration Group
            /// </summary>
            [DataMember]
            public string RegGroup { get; set; }

            /// <summary>
            /// Year
            /// </summary>
            [DataMember]
            public string Year { get; set; }
            
            /// <summary>
            /// House
            /// </summary>
            [DataMember]
            public string House { get; set; }

        }
    }
}