namespace vstest.diag.test
{
    using System;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using FluentAssertions;

    using vstest.diag.Utilities;

    [TestClass]
    public class LogManagerTests
    {
        private const string LogFile = "DiagnosticsLog.txt";
        private readonly string _logFilePath;

        public LogManagerTests()
        {
            _logFilePath = Path.Combine(Path.GetTempPath(), LogFile);
        }

        [TestMethod]
        public void WriteLogHandlesNullEmptyMessage()
        {
            // If not yet existing - create it
            if (!File.Exists(_logFilePath))
            {
                LogManager.WriteLog("Test");
            }
            DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(_logFilePath);
            LogManager.WriteLog("");
            File.GetLastWriteTimeUtc(_logFilePath).Should().Be(lastWriteTimeUtc);
            LogManager.WriteLog(null);
            File.GetLastWriteTimeUtc(_logFilePath).Should().Be(lastWriteTimeUtc);
        }

        [TestMethod]
        public void WriteLogHandlesValidMessage()
        {
            // If not yet existing - create it
            if (!File.Exists(_logFilePath))
            {
                LogManager.WriteLog("Test");
            }
            var lastWriteTimeUtc = File.GetLastWriteTimeUtc(_logFilePath);
            LogManager.WriteLog("Testing Again");
            File.GetLastWriteTimeUtc(_logFilePath).Should().BeAfter(lastWriteTimeUtc);
        }

        [TestMethod]
        public void EnableLogsThrowsNoException()
        {
            Action enableLogsAction = () => LogManager.EnableLogs(new TestInvoke());
            enableLogsAction.ShouldNotThrow("Did not expect the EnableLogs call to throw an exception");
        }

        [TestMethod]
        public void DisableLogsThrowsNoException()
        {
            Action disableLogsAction = LogManager.DisableLogs;
            disableLogsAction.ShouldNotThrow("Did not expect the DisableLogs call to throw an exception");
        }

        [TestMethod]
        public void EnableLogsEnablesWcfLogs()
        {
            var system32Path = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "system32");
            var logmanCommand = "logman.exe start WCFETWTracing -p \"Microsoft-Windows-Application Server-Applications\" 0xFFFFFFFF  0x5 -bs 64 -nb 120 320 -ets -ct perf -f bincirc -max 500 -o";
            var testInvokeExe = new TestInvoke();
            LogManager.EnableLogs(testInvokeExe);

            testInvokeExe.commandCalled.Should().NotBeNullOrEmpty();
            testInvokeExe.commandCalled.Should().StartWith(system32Path + Path.DirectorySeparatorChar + logmanCommand);
            testInvokeExe.commandCalled.Should().EndWith("WCFEtwTrace.etl");
        }
    }
}
