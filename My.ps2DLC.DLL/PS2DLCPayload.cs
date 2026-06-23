using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace My.ps2DLC.DLL
{
    public partial class PS2DLC
    {
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
