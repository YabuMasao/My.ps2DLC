using System;
using System.Collections.Generic;

namespace My.ps2DLC.DLL
{
    public partial class PS2DLC
    {
        public static bool Enable()
        {
            EnsureAdministrator();

            using (Payload payload = Payload.Extract(GetRequiredAssemblyResourceNames()))
            {
                InstallAssembliesToGac(payload.AssemblyPaths);
            }

            WriteRegistryKeys();

            return Check();
        }

        public static bool Disable()
        {
            EnsureAdministrator();

            using (Payload payload = Payload.Extract(GetRequiredAssemblyResourceNames()))
            {
                RemoveAssembliesFromGac(payload.AssemblyPaths);
            }

            DeleteRegistryKeys();

            return !Check();
        }

        public static bool Check()
        {
            PS2DLCFileStatus[] fileStatuses;
            return Check(out fileStatuses);
        }

        public static bool Check(out PS2DLCFileStatus[] fileStatuses)
        {
            fileStatuses = CheckFiles();
            foreach (PS2DLCFileStatus fileStatus in fileStatuses)
            {
                if (!fileStatus.IsEnabled)
                {
                    return false;
                }
            }

            return true;
        }

        public static PS2DLCFileStatus[] CheckFiles()
        {
            List<PS2DLCFileStatus> fileStatuses = new List<PS2DLCFileStatus>();

            try
            {
                List<string> assemblyResourceNames = GetRequiredAssemblyResourceNames();
                using (Payload payload = Payload.Extract(assemblyResourceNames))
                {
                    for (int index = 0; index < assemblyResourceNames.Count; index++)
                    {
                        fileStatuses.Add(CreateAssemblyStatus(assemblyResourceNames[index], payload.AssemblyPaths[index]));
                    }
                }

                foreach (string registryResourceName in GetRequiredRegistryResourceNames())
                {
                    fileStatuses.Add(CreateRegistryStatus(registryResourceName));
                }
            }
            catch (Exception ex)
            {
                fileStatuses.Add(new PS2DLCFileStatus("Check", string.Empty, "Error", false, false, string.Empty, ex.Message));
            }

            return fileStatuses.ToArray();
        }
    }
}
