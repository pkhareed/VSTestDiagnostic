using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vstest.diag.Utilities;

namespace vstest.diag.test
{
    //[TestClass]
    public class BasicE2E
    {
        private static string _exe;
        [ClassInitialize]
        public static void ClassInit(TestContext t)
        {
            _exe = Path.Combine(Environment.CurrentDirectory, @"..\..\..\vstest.diag\bin\Debug\vstest.diag.exe");
        }
       
        [TestMethod]
        public void IsExeAvail()
        {
            Console.WriteLine(Path.Combine(Environment.CurrentDirectory, _exe));
            Assert.IsTrue(File.Exists(_exe));
        }

        [TestMethod]
        public void IsExeRunnable()
        {
            string exeOut, exeErr;
            new Invoke().InvokeExe(Path.Combine(Environment.CurrentDirectory, _exe), "", out exeOut, out exeErr, true);
            Assert.IsTrue(string.IsNullOrEmpty(exeErr));
        }

        [TestMethod]
        public void IsExeHelpAvail()
        {
            string exeOut, exeErr;
            const string helpText = @"Usage: vstest.diag.exe [Options]
Description: Enables logs and runs some basic framework and platform tests to check environment sanity.
Options:
/RunTests - Runs only tests (preferably after enabling logs, generates %temp%\DiagnosticsLog.txt)
/EnableLogs - Enables various TpTrace logs for unit test framework executables.
/DisableLogs - Disables the logs if enabled earlier.
/All - Enables logs, Runs tests and disables the logs in the mentioned order.
/Help - Display this help and exit.";
            new Invoke().InvokeExe(Path.Combine(Environment.CurrentDirectory, _exe), "", out exeOut, out exeErr, true);
            Assert.IsTrue(string.IsNullOrEmpty(exeErr));
            StringAssert.Contains(exeOut, helpText);
        }

        [TestMethod]
        public void IsExeSetLogs()
        {
            string exeOut, exeErr;
            const string helpText = "Help: ";
            new Invoke().InvokeExe(Path.Combine(Environment.CurrentDirectory, _exe), "", out exeOut, out exeErr, true);
            Assert.IsTrue(string.IsNullOrEmpty(exeErr));
            //Assert.IsTrue(!string.IsNullOrEmpty(exeOut));
            StringAssert.Contains(exeOut, helpText, "Expected: {0}", helpText);
        }

        [TestMethod]
        public void IsExeUnSetLogs()
        {
            string exeOut, exeErr;
            const string helpText = "Help: ";
            new Invoke().InvokeExe(Path.Combine(Environment.CurrentDirectory, _exe), "", out exeOut, out exeErr, true);
            Assert.IsTrue(string.IsNullOrEmpty(exeErr));
            //Assert.IsTrue(!string.IsNullOrEmpty(exeOut));
            StringAssert.Contains(exeOut, helpText, "Expected: {0}", helpText);
        }
    }
}
