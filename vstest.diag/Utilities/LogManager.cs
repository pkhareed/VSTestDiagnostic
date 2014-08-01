namespace vstest.diag
{
    using System;
    using System.IO;
    using Utilities;

    public class LogManager
    {
        /// <summary>
        /// Log file name for the tool.
        /// </summary>
        private const string LogFile = "DiagnosticsLog.txt";


        public static void WriteLog(string strMessage)
        {
            if (string.IsNullOrEmpty(strMessage))
            {
                return;
            }

            var fileName = Path.Combine(Path.GetTempPath(), LogFile);

            // Write the log message and test results in a output text file.
            using (var writer = new StreamWriter(new FileStream(fileName, FileMode.Append)))
            {
                writer.WriteLine(Environment.TickCount + " : " + strMessage);
            }
        }

        public static void EnableLogs(IInvoke invokeExe)
        {
            // 1. Enable wcf logging
            string processOutput;
            string processError;
            var logmanArg = "start WCFETWTracing -p \"Microsoft-Windows-Application Server-Applications\" 0xFFFFFFFF  0x5 -bs 64 -nb 120 320 -ets -ct perf -f bincirc -max 500 -o " + Path.Combine(Path.GetTempPath(), "WCFEtwTrace.etl");
            invokeExe.InvokeExe(Path.Combine(Environment.SystemDirectory, "logman.exe"), logmanArg, out processOutput, out processError, true, 0);

            // 2. Enable execution engine logging
            var testWindowPath = Path.Combine(Environment.GetEnvironmentVariable("VS120COMNTOOLS"), @"..\IDE\CommonExtensions\Microsoft\TestWindow");
            XmlTrace.EnableXmlTrace(Path.Combine(testWindowPath, @"vstest.executionengine.x86.exe.config"));
            XmlTrace.EnableXmlTrace(Path.Combine(testWindowPath, @"vstest.executionengine.exe.config"));
            // 3.Enable vstest.console logs
            XmlTrace.EnableXmlTrace(Path.Combine(testWindowPath, @"vstest.console.exe.config"));
            // 4.Enable Discovery logs
            XmlTrace.EnableXmlTrace(Path.Combine(testWindowPath, @"vstest.discoveryengine.x86.exe.config"));
            XmlTrace.EnableXmlTrace(Path.Combine(testWindowPath, @"vstest.discoveryengine.exe.config"));

            // 5. Visual Studio IDE logs
            XmlTrace.EnableXmlTrace(
                Path.Combine(Environment.GetEnvironmentVariable("VS120COMNTOOLS"),
                @"..\IDE\devenv.exe.config"));
        }


        public static void DisableLogs()
        {
            // 1. Disable Wcf logging
            string processOutput;
            string processError;
            new Invoke().InvokeExe(Path.Combine(Environment.SystemDirectory, "logman.exe"), "stop WCFETWTracing -ets", out processOutput, out processError, true);

            // 2. Disable vstest logging
            var testWindowPath = Path.Combine(Environment.GetEnvironmentVariable("VS120COMNTOOLS"), @"..\IDE\CommonExtensions\Microsoft\TestWindow");
            XmlTrace.DisableXmlTrace(Path.Combine(testWindowPath, @"vstest.executionengine.x86.exe.config"));
            XmlTrace.DisableXmlTrace(Path.Combine(testWindowPath, @"vstest.executionengine.exe.config"));
            XmlTrace.DisableXmlTrace(Path.Combine(testWindowPath, @"vstest.console.exe.config"));
            XmlTrace.DisableXmlTrace(Path.Combine(testWindowPath, @"vstest.discoveryengine.x86.exe.config"));
            XmlTrace.DisableXmlTrace(Path.Combine(testWindowPath, @"vstest.discoveryengine.exe.config"));

            // 3. Disable Visual Studio logging
            XmlTrace.DisableXmlTrace(
                Path.Combine(Environment.GetEnvironmentVariable("VS120COMNTOOLS"),
                @"..\IDE\devenv.exe.config"));

        }
    }
}