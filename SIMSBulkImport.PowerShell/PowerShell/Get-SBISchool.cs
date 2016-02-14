using System;
using System.Management.Automation;
using System.Runtime.Serialization;

namespace SIMSBulkImport.PowerShell
{
    [Cmdlet("Get", "SBISchool", SupportsShouldProcess = false)]
    public class Get_SBISchool : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            School _school = new School();
            _school.Name = "Test";
            _school.LA = 000;
            _school.DfE = 0000;

            WriteObject(_school, true);
        }

        [Serializable]
        public class School
        {
            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public int LA { get; set; }

            [DataMember]
            public int DfE { get; set; }
        }
    }
}