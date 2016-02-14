using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SIMSAPI_UnitTest
{
    [TestClass]
    public class Main
    {
        private Matt40k.SIMSBulkImport.SIMSAPI simsapi;
        private bool runSimsApiTests;

        public Main()
        {
            // Change to true to enable SIMS Api unit tests
            runSimsApiTests = false;

            string simsDir = "c:\\program files (x86)\\sims\\sims .net";
            simsapi = new Matt40k.SIMSBulkImport.SIMSAPI(simsDir);
            // Please set the SIMS username and password
            simsapi.SetSimsUser = "<<removed>>";
            simsapi.SetSimsPass = "<<removed>>";

            if (runSimsApiTests)
            {
                bool result = simsapi.Connect;
            }
        }

        [TestMethod]
        public void GetPupilUsernameUDFs()
        {
            if (runSimsApiTests)
            {
                simsapi.SetUserType = Matt40k.SIMSBulkImport.Interfaces.UserType.Pupil;
                var result = simsapi.GetPupilUsernameUDFs;
                int cnt = result.Count;
                Assert.AreEqual(1, cnt);
            }
        }
    }
}
