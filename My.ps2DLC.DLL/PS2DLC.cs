using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using Microsoft.Win32;
using System.EnterpriseServices.Internal;

namespace My.ps2DLC.DLL
{
    public class PS2DLC
    {
        private const string ResourceRoot = "Original/";
        private const string BinaryRoot = ResourceRoot + "Binaries/";
        private const string ResourceBinaryRoot = ResourceRoot + "ResourceBinaries/";
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

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetSystemDefaultLocaleName(StringBuilder localeName, int localeNameLength);

        public static bool Enable()
        {
            EnsureAdministrator();

            using (Payload payload = Payload.Extract(GetRequiredResourceNames()))
            {
                Publish publish = new Publish();
                foreach (string assemblyPath in payload.AssemblyPaths)
                {
                    publish.GacInstall(assemblyPath);
                }
            }

            WriteRegistryKeys();

            return Check();
        }

        public static bool Disable()
        {
            EnsureAdministrator();

            using (Payload payload = Payload.Extract(GetRequiredResourceNames()))
            {
                Publish publish = new Publish();
                foreach (string assemblyPath in payload.AssemblyPaths)
                {
                    publish.GacRemove(assemblyPath);
                }
            }

            DeleteRegistryKeys();

            return !Check();
        }

        public static bool Check()
        {
            try
            {
                using (Payload payload = Payload.Extract(GetRequiredResourceNames()))
                {
                    foreach (string assemblyPath in payload.AssemblyPaths)
                    {
                        if (!IsAssemblyInGac(assemblyPath))
                        {
                            return false;
                        }
                    }
                }

                return HasRegistryKeys();
            }
            catch
            {
                return false;
            }
        }

        private static void EnsureAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                throw new InvalidOperationException("Administrator privileges are required.");
            }
        }

        private static List<string> GetRequiredResourceNames()
        {
            string architecture = GetArchitectureName();
            string localeName = GetSystemLocaleName();
            string binaryPrefix = BinaryRoot + architecture + "/";
            string resourceBinaryPrefix = ResourceBinaryRoot + architecture + "/" + localeName + "/";

            Assembly assembly = typeof(PS2DLC).Assembly;
            string[] resourceNames = assembly.GetManifestResourceNames();
            List<string> requiredResourceNames = new List<string>();

            foreach (string resourceName in resourceNames)
            {
                string normalizedName = NormalizeResourceName(resourceName);
                if (IsDllResource(normalizedName, binaryPrefix) || IsDllResource(normalizedName, resourceBinaryPrefix))
                {
                    requiredResourceNames.Add(resourceName);
                }
            }

            if (!HasResource(requiredResourceNames, binaryPrefix))
            {
                throw new DirectoryNotFoundException("Embedded binary resources were not found for architecture: " + architecture);
            }

            requiredResourceNames.Sort(StringComparer.OrdinalIgnoreCase);
            return requiredResourceNames;
        }

        private static bool IsDllResource(string resourceName, string prefix)
        {
            return resourceName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                && resourceName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase);
        }

        private static bool HasResource(IEnumerable<string> resourceNames, string prefix)
        {
            foreach (string resourceName in resourceNames)
            {
                if (NormalizeResourceName(resourceName).StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetArchitectureName()
        {
            string architecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            if (string.Equals(architecture, "AMD64", StringComparison.OrdinalIgnoreCase))
            {
                return "amd64";
            }

            if (string.Equals(architecture, "x86", StringComparison.OrdinalIgnoreCase))
            {
                return "x86";
            }

            throw new PlatformNotSupportedException("Unsupported architecture: " + architecture);
        }

        private static string GetSystemLocaleName()
        {
            StringBuilder localeName = new StringBuilder(85);
            if (GetSystemDefaultLocaleName(localeName, localeName.Capacity) > 0)
            {
                return localeName.ToString();
            }

            return CultureInfo.InstalledUICulture.Name;
        }

        private static string NormalizeResourceName(string resourceName)
        {
            return resourceName.Replace('\\', '/');
        }

        private static bool IsAssemblyInGac(string assemblyPath)
        {
            AssemblyName assemblyName = AssemblyName.GetAssemblyName(assemblyPath);

            try
            {
                Assembly assembly = Assembly.ReflectionOnlyLoad(assemblyName.FullName);
                return assembly.GlobalAssemblyCache;
            }
            catch
            {
                return false;
            }
        }

        private static void WriteRegistryKeys()
        {
            using (RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, GetRegistryView()))
            {
                using (RegistryKey key = localMachine.CreateSubKey(PowerShellKeyPath))
                {
                    key.SetValue("Install", 1, RegistryValueKind.DWord);
                    key.SetValue("PID", Pid, RegistryValueKind.String);
                }

                using (RegistryKey key = localMachine.CreateSubKey(PowerShellEngineKeyPath))
                {
                    key.SetValue("ApplicationBase", PowerShellApplicationBase, RegistryValueKind.String);
                    key.SetValue("ConsoleHostAssemblyName", ConsoleHostAssemblyName, RegistryValueKind.String);
                    key.SetValue("ConsoleHostModuleName", ConsoleHostModuleName, RegistryValueKind.String);
                    key.SetValue("PowerShellVersion", "2.0", RegistryValueKind.String);
                    key.SetValue("PSCompatibleVersion", "1.0, 2.0", RegistryValueKind.String);
                    key.SetValue("RuntimeVersion", "v2.0.50727", RegistryValueKind.String);
                }

                localMachine.CreateSubKey(ShellIdsKeyPath).Dispose();

                using (RegistryKey key = localMachine.CreateSubKey(MicrosoftPowerShellShellIdKeyPath))
                {
                    key.SetValue("Path", PowerShellPath, RegistryValueKind.String);
                    key.SetValue("ExecutionPolicy", "Bypass", RegistryValueKind.String);
                }

                using (RegistryKey key = localMachine.CreateSubKey(ScriptedDiagnosticsShellIdKeyPath))
                {
                    key.SetValue("ExecutionPolicy", "Unrestricted", RegistryValueKind.String);
                }
            }
        }

        private static void DeleteRegistryKeys()
        {
            using (RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, GetRegistryView()))
            {
                localMachine.DeleteSubKeyTree(PowerShellKeyPath, false);
            }
        }

        private static bool HasRegistryKeys()
        {
            using (RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, GetRegistryView()))
            {
                return HasValue(localMachine, PowerShellKeyPath, "Install", 1)
                    && HasValue(localMachine, PowerShellKeyPath, "PID", Pid)
                    && HasValue(localMachine, PowerShellEngineKeyPath, "ApplicationBase", PowerShellApplicationBase)
                    && HasValue(localMachine, PowerShellEngineKeyPath, "ConsoleHostAssemblyName", ConsoleHostAssemblyName)
                    && HasValue(localMachine, PowerShellEngineKeyPath, "ConsoleHostModuleName", ConsoleHostModuleName)
                    && HasValue(localMachine, PowerShellEngineKeyPath, "PowerShellVersion", "2.0")
                    && HasValue(localMachine, PowerShellEngineKeyPath, "PSCompatibleVersion", "1.0, 2.0")
                    && HasValue(localMachine, PowerShellEngineKeyPath, "RuntimeVersion", "v2.0.50727")
                    && HasValue(localMachine, MicrosoftPowerShellShellIdKeyPath, "Path", PowerShellPath)
                    && HasValue(localMachine, MicrosoftPowerShellShellIdKeyPath, "ExecutionPolicy", "Bypass")
                    && HasValue(localMachine, ScriptedDiagnosticsShellIdKeyPath, "ExecutionPolicy", "Unrestricted");
            }
        }

        private static bool HasValue(RegistryKey localMachine, string keyPath, string valueName, object expectedValue)
        {
            using (RegistryKey key = localMachine.OpenSubKey(keyPath))
            {
                if (key == null)
                {
                    return false;
                }

                object actualValue = key.GetValue(valueName);
                return object.Equals(actualValue, expectedValue);
            }
        }

        private static RegistryView GetRegistryView()
        {
            return Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
        }

        private sealed class Payload : IDisposable
        {
            private readonly string directoryPath;

            private Payload(string directoryPath, List<string> assemblyPaths)
            {
                this.directoryPath = directoryPath;
                AssemblyPaths = assemblyPaths;
            }

            public List<string> AssemblyPaths { get; private set; }

            public static Payload Extract(IEnumerable<string> resourceNames)
            {
                string directoryPath = Path.Combine(Path.GetTempPath(), "My.ps2DLC.DLL", Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(directoryPath);

                List<string> assemblyPaths = new List<string>();
                Assembly assembly = typeof(PS2DLC).Assembly;

                foreach (string resourceName in resourceNames)
                {
                    string targetPath = GetTargetPath(directoryPath, resourceName);
                    string targetDirectory = Path.GetDirectoryName(targetPath);
                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    using (Stream input = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (input == null)
                        {
                            throw new FileNotFoundException("Embedded resource was not found.", resourceName);
                        }

                        using (Stream output = File.Create(targetPath))
                        {
                            input.CopyTo(output);
                        }
                    }

                    assemblyPaths.Add(targetPath);
                }

                return new Payload(directoryPath, assemblyPaths);
            }

            public void Dispose()
            {
                try
                {
                    if (Directory.Exists(directoryPath))
                    {
                        Directory.Delete(directoryPath, true);
                    }
                }
                catch
                {
                }
            }

            private static string GetTargetPath(string directoryPath, string resourceName)
            {
                string normalizedName = NormalizeResourceName(resourceName);
                string relativePath = normalizedName.Substring(ResourceRoot.Length).Replace('/', Path.DirectorySeparatorChar);
                return Path.Combine(directoryPath, relativePath);
            }
        }
    }
}
