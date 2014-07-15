namespace vstest.diag.test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;

    [TestClass]
    public class ArgumentParserTests
    {
        [TestMethod]
        public void ParseShouldReturnHelpTaskForInvalidArgument()
        {
            var output = CreateAndParseArgumentsOutput("/invalidcommand");

            output.Should().Be(DiagnosticTask.Help);
        }

        [TestMethod]
        public void ParseShouldReturnHelpForHelpArgument()
        {
            var output = CreateAndParseArgumentsOutput("/Help");

            output.Should().Be(DiagnosticTask.Help);
        }

        [TestMethod]
        public void EnableLogsArgShouldReturnEnableLogTask()
        {
            var output = CreateAndParseArgumentsOutput("/enablelogs");

            output.Should().Be(DiagnosticTask.EnableLogs);
        }

        [TestMethod]
        public void DisableLogsArgShouldReturnDisableLogsTask()
        {
            var output = CreateAndParseArgumentsOutput("/disablelogs");

            output.Should().Be(DiagnosticTask.DisableLogs);
        }

        [TestMethod]
        public void RunTestsArgShouldReturnTestRunTests()
        {
            var output = CreateAndParseArgumentsOutput("/runtests");

            output.Should().Be(DiagnosticTask.RunTests);
        }

        private static DiagnosticTask CreateAndParseArgumentsOutput(string args)
        {
            return new ArgumentParser().Parse(args);
        }
    }
}
