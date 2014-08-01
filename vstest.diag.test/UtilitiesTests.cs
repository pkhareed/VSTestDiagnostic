using System.IO;

namespace vstest.diag.test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;
    using Utilities;

    [TestClass]
    public class UtilitiesTests
    {
        #region InvokeExeTests
        [TestMethod]
        public void InvokeExeHandlesNullOrEmptyParams()
        {
            string outStr;
            string errStr;          
            new Invoke().InvokeExe(string.Empty, 
                string.Empty, out outStr, out errStr,
                false).Should().NotBe(0);
        }

        [TestMethod]
        public void InvokeExeHandlesWrongProgram()
        {
            string outStr;
            string errStr;
            new Invoke().InvokeExe("echotwice",
                string.Empty, out outStr, out errStr,
                false).Should().NotBe(0);
        }

        [TestMethod]
        public void InvokeExeHandlesWrongParam()
        {
            string outStr;
            string errStr;
            new Invoke().InvokeExe("dir",
                "GhostFile", out outStr, out errStr,
                false).Should().NotBe(0);
        }

        [TestMethod]
        public void InvokeExeHandlesLongProgramExceedingTime()
        {
            string outStr;
            string errStr;
            new Invoke().InvokeExe("ping",
                " 1.1.1.1 -n 1 -w 3000 > nul", out outStr, out errStr,
                false, 1000).Should().NotBe(0);
        }

        [TestMethod]
        public void InvokeExeHandlesLongProgramNegativeTime()
        {
            string outStr;
            string errStr;
            new Invoke().InvokeExe("ping",
                " 1.1.1.1 -n 1 -w 3000 > nul", out outStr, out errStr,
                false, -11).Should().NotBe(0);
        }
        #endregion

        #region TraceXMLTests

        [TestMethod]
        public void EnableXmlTraceHandlesNonExistingFile()
        {
            XmlTrace.EnableXmlTrace("FooBar.xml").Should().BeFalse();
        }

        [TestMethod]
        public void EnableXmlTraceHandlesNonConfigFile()
        {
            XmlTrace.EnableXmlTrace("Resources\\" + "vstest_diag_test.testsettings").Should().BeFalse();
        }
  
        [TestMethod]
        public void EnableXmlTraceTakesBackupAlsoCheckTimeStamp()
        {
            var lastWriteTimeOriginalConfig = File.GetLastWriteTimeUtc("Resources\\" + Constants.DiscoveryX86Config);
            XmlTrace.EnableXmlTrace("Resources\\" + Constants.DiscoveryX86Config)
                .Should().BeTrue("Unable to enable trace for {0}", Constants.DiscoveryX86Config);
            File.Exists("Resources\\" + Constants.DiscoveryX86Config + "_bk").Should().BeTrue();
            var lastWriteTimeOriginalConfigBackup =
                File.GetLastWriteTimeUtc("Resources\\" + Constants.DiscoveryX86Config + "_bk");
            lastWriteTimeOriginalConfigBackup.Should()
                .BeBefore(File.GetLastWriteTimeUtc("Resources\\" + Constants.DiscoveryX86Config));
            lastWriteTimeOriginalConfigBackup.Should().Be(lastWriteTimeOriginalConfig);
        }

        [TestMethod]
        public void EnableTraceXmlEnablesTraceForConfigWithNode()
        {
            const string textFilter = "TpTraceLevel";
            string outStr, errStr;
            XmlTrace.EnableXmlTrace("Resources\\" + Constants.DiscoveryX86Config);
            new Invoke().InvokeExe("cmd.exe",
                "/C findstr /spin /c:" + "\"" + textFilter + "\" " + "Resources\\" + Constants.DiscoveryX86Config,
                out outStr, out errStr,
                false, 5000);
            outStr.Should().Contain("TpTraceLevel\" value=\"4\"");
        }

        [TestMethod]
        public void EnableTraceXmlEnablesTraceForConfigWithOutNode()
        {
            const string textFilter = "TpTraceLevel";
            string outStr, errStr;
            XmlTrace.EnableXmlTrace("Resources\\" + Constants.DevenvConfig);
            new Invoke().InvokeExe("cmd.exe",
                "/C findstr /spin /c:" + "\"" + textFilter + "\" " + "Resources\\" + Constants.DevenvConfig,
                out outStr, out errStr,
                false, 5000);
            outStr.Should().Contain("TpTraceLevel\" value=\"4\"");
        }

        [TestMethod]
        public void DisableXmlTraceHandlesNonExistingFile()
        {
            XmlTrace.DisableXmlTrace("FooBar.xml").Should().BeFalse();
        }

        [TestMethod]
        public void DisableXmlTraceHandlesNonConfigFile()
        {
            XmlTrace.DisableXmlTrace("Resources\\" + "vstest_diag_test.testsettings").Should().BeFalse();
        }

        [TestMethod]
        public void DisableTraceXmlRestoresOriginalConfig()
        {
            // Setup Backup First
            XmlTrace.EnableXmlTrace("Resources\\" + Constants.DiscoveryX86Config);

            var lastWriteTimeOriginalConfigBackup =
                File.GetLastWriteTimeUtc("Resources\\" + Constants.DiscoveryX86Config + "_bk");
            XmlTrace.DisableXmlTrace("Resources\\" + Constants.DiscoveryX86Config)
                .Should().BeTrue("Unable to disable trace for {0}", Constants.DiscoveryX86Config);
            File.GetLastWriteTimeUtc("Resources\\" + Constants.DiscoveryX86Config)
                .Should()
                .Be(lastWriteTimeOriginalConfigBackup);
        }

        #endregion
    }
}
