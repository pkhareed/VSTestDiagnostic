namespace vstest.diag.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Eventing.Reader;
    using System.Text;

    public interface IInvoke
    {
        int InvokeExe(string process, string args, out string output, out string error, bool wait, int maxTime);
    }

    public class TestInvoke : IInvoke
    {
        public string commandCalled;

        public int InvokeExe(
            string processName,
            string processArgs,
            out string processOutput,
            out string processError,
            bool waitForExit,
            int maxTimeOut = 120000)
        {
            Console.WriteLine("InvokeExe called with arguments:{0} {1}", processName, processArgs);
            commandCalled = processName + " " + processArgs;
            processOutput = processError = string.Empty;
            return 0;
        }
    }

    public class Invoke : IInvoke
    {
        private StringBuilder output;

        public int InvokeExe(string processName, string processArgs, out string processOutput,
            out string processError, bool waitForExit, int maxTimeOut = 120000)
        {
            Console.WriteLine("{0} Process called with arguments: {1}", processName, processArgs);
            processOutput = processError = string.Empty;
            if (string.IsNullOrEmpty(processName))
            {
                Console.WriteLine("Process name cannot be null or empty!");
                return -1;
            }
            
            output = new StringBuilder(string.Empty);
            var psStartInfo = new ProcessStartInfo(processName)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = processArgs,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true
            };
            var process = new Process {StartInfo = psStartInfo};
            try
            {
                process.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "InvokeExe generated an exception for process: {0}\n Exception details: {1}",
                    processName, 
                    e);
                processError = e.ToString();
                return -1;
            }
            
            process.OutputDataReceived += OutputHandler;
            process.BeginOutputReadLine();          
            processError = process.StandardError.ReadToEnd();
            if (waitForExit)
            {
                process.WaitForExit(maxTimeOut);
            }

            if (!process.HasExited)
            {
                process.Kill();
                processOutput = null;
                return -1;
            }
            processOutput = output.ToString();
            return process.ExitCode;
        }

        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            // Collect the command output. 
            if (!string.IsNullOrEmpty(outLine.Data))
            {
                // Add the text to the collected output.
                output.Append(Environment.NewLine + outLine.Data);
            }
        }
    }
}
