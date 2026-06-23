using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Reflection;

namespace My.ps2DLC.DLL
{
    public partial class PS2DLC
    {
        private static void InstallAssembliesToGac(IEnumerable<string> assemblyPaths)
        {
            Publish publish = new Publish();
            foreach (string assemblyPath in assemblyPaths)
            {
                publish.GacInstall(assemblyPath);
            }
        }

        private static void RemoveAssembliesFromGac(IEnumerable<string> assemblyPaths)
        {
            Publish publish = new Publish();
            foreach (string assemblyPath in assemblyPaths)
            {
                publish.GacRemove(assemblyPath);
            }
        }

        private static PS2DLCFileStatus CreateAssemblyStatus(string resourceName, string assemblyPath)
        {
            string normalizedName = NormalizeResourceName(resourceName);
            string fileName = normalizedName.Substring(ResourceRoot.Length);
            string target = string.Empty;
            bool isInstalled = false;
            string message;

            try
            {
                target = AssemblyName.GetAssemblyName(assemblyPath).FullName;
                isInstalled = IsAssemblyInGac(assemblyPath);
                message = isInstalled ? "GAC installed." : "GAC missing.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return new PS2DLCFileStatus(fileName, normalizedName, "GAC", true, isInstalled, target, message);
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
    }
}
