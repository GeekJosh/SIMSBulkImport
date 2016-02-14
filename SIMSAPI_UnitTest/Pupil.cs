using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SIMSAPI_UnitTest
{
    [TestClass]
    public class Pupil
    {
        private bool runSimsApiTests;

        public Pupil()
        {
            string simsDir = "c:\\program files (x86)\\sims\\sims .net";
            Matt40k.SIMSBulkImport.SIMSAPI simsapi = new Matt40k.SIMSBulkImport.SIMSAPI(simsDir);
            // Please set the SIMS username and password
            simsapi.SetSimsUser = "<<removed>>";
            simsapi.SetSimsPass = "<<removed>>";

            // Change to true to enable SIMS Api unit tests
            runSimsApiTests = false;

            if (runSimsApiTests)
            {
                bool result = simsapi.Connect;
                simsapi.SetUserType = Matt40k.SIMSBulkImport.Interfaces.UserType.Pupil;
            }
     
        }

        [TestMethod]
        public void GetDefaultPupil()
        {
            if (runSimsApiTests)
            {
                Matt40k.SIMSBulkImport.Classes.Pupils _pupils = new Matt40k.SIMSBulkImport.Classes.Pupils();
                var id = _pupils.GetDefaultStudentPersonId;
                Assert.IsNotNull(id);
            }
        }

        [TestMethod]
        public void GetPupilUsernameUDFs()
        {
            if (runSimsApiTests)
            {
                Matt40k.SIMSBulkImport.Classes.Pupils _pupils = new Matt40k.SIMSBulkImport.Classes.Pupils();
                var udfs = _pupils.GetPupilUsernameUDFs;
                int cnt = udfs.Count;
                Assert.AreEqual(1, cnt);
            }
        }
    }
}
