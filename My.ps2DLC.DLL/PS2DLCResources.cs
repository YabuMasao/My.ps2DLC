using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace My.ps2DLC.DLL
{
    public partial class PS2DLC
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetSystemDefaultLocaleName(StringBuilder localeName, int localeNameLength);

        private static List<string> GetRequiredAssemblyResourceNames()
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

        private static List<string> GetRequiredRegistryResourceNames()
        {
            Assembly assembly = typeof(PS2DLC).Assembly;
            string[] resourceNames = assembly.GetManifestResourceNames();
            List<string> requiredResourceNames = new List<string>();

            foreach (string resourceName in resourceNames)
            {
                string normalizedName = NormalizeResourceName(resourceName);
                if (normalizedName.StartsWith(RegistryRoot, StringComparison.OrdinalIgnoreCase)
                    && normalizedName.EndsWith(".reg", StringComparison.OrdinalIgnoreCase))
                {
                    requiredResourceNames.Add(resourceName);
                }
            }

            if (requiredResourceNames.Count == 0)
            {
                throw new DirectoryNotFoundException("Embedded registry resources were not found.");
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
    }
}
