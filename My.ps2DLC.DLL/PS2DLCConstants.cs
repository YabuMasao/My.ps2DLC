namespace My.ps2DLC.DLL
{
    public partial class PS2DLC
    {
        private const string ResourceRoot = "Original/";
        private const string BinaryRoot = ResourceRoot + "Binaries/";
        private const string ResourceBinaryRoot = ResourceRoot + "ResourceBinaries/";
        private const string RegistryRoot = ResourceRoot + "regkeysNew/";

        private const string PowerShellKeyPath = @"SOFTWARE\Microsoft\PowerShell\1";
        private const string PowerShellEngineKeyPath = PowerShellKeyPath + @"\PowerShellEngine";
        private const string ShellIdsKeyPath = PowerShellKeyPath + @"\ShellIds";
        private const string MicrosoftPowerShellShellIdKeyPath = ShellIdsKeyPath + @"\Microsoft.PowerShell";
        private const string ScriptedDiagnosticsShellIdKeyPath = ShellIdsKeyPath + @"\ScriptedDiagnostics";

        private const string Pid = "89383-100-0001260-04309";
        private const string PowerShellApplicationBase = @"C:\Windows\System32\WindowsPowerShell\v1.0";
        private const string ConsoleHostAssemblyName = "Microsoft.PowerShell.ConsoleHost, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, ProcessorArchitecture=msil";
        private const string ConsoleHostModuleName = @"C:\Windows\System32\WindowsPowerShell\v1.0\Microsoft.PowerShell.ConsoleHost.dll";
        private const string PowerShellPath = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
    }
}
