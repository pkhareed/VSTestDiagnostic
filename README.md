# vstest.diag - Diagnostic tool for the Visual Studio Unit Test platform

This tool enables diagnostic log collection for various unit test framework components. 

It will collect following logs:

>	DiagnosticsLog.txt (The log for this tool)
>	WCFEtwTrace.etl (WCF log)
>	vstest.executionengine.TpTrace.log 
>	vstest.executionengine.x86.TpTrace.log 
>	vstest.discoveryengine.TpTrace.log
>	vstest.discoveryengine.x86.TpTrace.log
>	devenv.TpTrace

## Usage
`vstest.diag (/EnableLogs|/DisableLogs)`

### Options

/EnableLogs     Enables various diagnostic logs for unit test framework. 

    Post this you can execute the test platform scenarios, the details will be captured by the logs.
    Run this tool again with /DisableLogs to stop collecting the verbose logs.
    Leaving logs enabled will cause slow performance.

/DisableLogs    Disables the logs if enabled earlier.
/Help           Show help

## Example
Below is a typical usage of this tool. All commands are run from Administrator command prompt.

    $ vstest.diag.exe /enablelogs
    $ vstest.console.exe mytests.dll # run your unit tests that reproduce a bug in the product
    $ vstest.diag.exe /disablelogs
    $ # copy logs from %temp%

## License
Apache License v2.0.

## Development
Clone the project, run `nuget restore`, patch and send a pull request!

We (VS Unit Testing team at Microsoft) developed this tool as a side project. It
collates the numerous diagnostics switches for test platform from various MSDN
blog posts into a single tool.

Hope it helps! <3
