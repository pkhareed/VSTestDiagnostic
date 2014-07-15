using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace vstest.diag
{
    using vstest.diag.Utilities;

    class Program
    {
        static int Main(string[] args)
        {
            if (args == null)
            {
                PrintHelp();
                return 1;
            }

            if (args.Length == 0)
            {
                PrintHelp();
                return 1;
            }

            var taskRun = new ArgumentParser().Parse(args[0]);

            switch (taskRun)
            {
                default:
                // case DiagnosticTask.Help:
                    PrintHelp();
                    break;
                case DiagnosticTask.EnableLogs:
                    LogManager.EnableLogs(new Invoke());
                    break;
                case DiagnosticTask.DisableLogs:
                    LogManager.DisableLogs();
                    break;
                case DiagnosticTask.RunTests:
                    // TestManager.RunTests();
                    throw new NotImplementedException();
                    //// break;
            }
            return 0;
        }

        private static void PrintHelp()
        {
            var exeDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            if (exeDir != null)
            {
                using (var reader = new StreamReader(Path.Combine(exeDir, "Help.txt")))
                {
                    var line = reader.ReadLine();
                    while (null != line)
                    {
                        Console.WriteLine(line);
                        line = reader.ReadLine();
                    }
                }
            }
        }
    }
}
