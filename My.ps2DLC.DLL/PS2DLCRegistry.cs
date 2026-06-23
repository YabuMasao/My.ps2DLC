using System;
using Microsoft.Win32;

namespace My.ps2DLC.DLL
{
    public partial class PS2DLC
    {
        private static PS2DLCFileStatus CreateRegistryStatus(string resourceName)
        {
            string normalizedName = NormalizeResourceName(resourceName);
            string fileName = normalizedName.Substring(ResourceRoot.Length);
            bool isApplied = IsRegistryResourceApplied(normalizedName);
            string target = GetRegistryResourceTarget(normalizedName);
            string message = isApplied ? "Registry applied." : "Registry missing.";

            return new PS2DLCFileStatus(fileName, normalizedName, "Registry", true, isApplied, target, message);
        }

        private static bool IsRegistryResourceApplied(string resourceName)
        {
            if (resourceName.EndsWith("regkeyPowerShellEngine.reg", StringComparison.OrdinalIgnoreCase))
            {
                return HasPowerShellEngineRegistryKeys();
            }

            if (resourceName.EndsWith("regkeyPowerShell.reg", StringComparison.OrdinalIgnoreCase))
            {
                return HasRegistryKeys();
            }

            return false;
        }

        private static string GetRegistryResourceTarget(string resourceName)
        {
            if (resourceName.EndsWith("regkeyPowerShellEngine.reg", StringComparison.OrdinalIgnoreCase))
            {
                return @"HKEY_LOCAL_MACHINE\" + PowerShellEngineKeyPath;
            }

            return @"HKEY_LOCAL_MACHINE\" + PowerShellKeyPath;
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
                return HasPowerShellRegistryValues(localMachine)
                    && HasPowerShellEngineRegistryValues(localMachine)
                    && HasShellIdRegistryValues(localMachine);
            }
        }

        private static bool HasPowerShellEngineRegistryKeys()
        {
            using (RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, GetRegistryView()))
            {
                return HasPowerShellEngineRegistryValues(localMachine);
            }
        }

        private static bool HasPowerShellRegistryValues(RegistryKey localMachine)
        {
            return HasValue(localMachine, PowerShellKeyPath, "Install", 1)
                && HasValue(localMachine, PowerShellKeyPath, "PID", Pid);
        }

        private static bool HasPowerShellEngineRegistryValues(RegistryKey localMachine)
        {
            return HasValue(localMachine, PowerShellEngineKeyPath, "ApplicationBase", PowerShellApplicationBase)
                && HasValue(localMachine, PowerShellEngineKeyPath, "ConsoleHostAssemblyName", ConsoleHostAssemblyName)
                && HasValue(localMachine, PowerShellEngineKeyPath, "ConsoleHostModuleName", ConsoleHostModuleName)
                && HasValue(localMachine, PowerShellEngineKeyPath, "PowerShellVersion", "2.0")
                && HasValue(localMachine, PowerShellEngineKeyPath, "PSCompatibleVersion", "1.0, 2.0")
                && HasValue(localMachine, PowerShellEngineKeyPath, "RuntimeVersion", "v2.0.50727");
        }

        private static bool HasShellIdRegistryValues(RegistryKey localMachine)
        {
            return HasValue(localMachine, MicrosoftPowerShellShellIdKeyPath, "Path", PowerShellPath)
                && HasValue(localMachine, MicrosoftPowerShellShellIdKeyPath, "ExecutionPolicy", "Bypass")
                && HasValue(localMachine, ScriptedDiagnosticsShellIdKeyPath, "ExecutionPolicy", "Unrestricted");
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
    }
}
