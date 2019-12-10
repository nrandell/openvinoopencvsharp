using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PeopleDetectionOpenCV
{
    public class AssemblyLoadConfiguration
    {
        public static void For<T>() => new AssemblyLoadConfiguration(typeof(T).Assembly).Configure();

        public Assembly Assembly { get; }

        public AssemblyLoadConfiguration(Assembly assembly)
        {
            Assembly = assembly;
        }

        public void Configure()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ConfigureForWindows();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                ConfigureForOSX();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                ConfigureForLinux();
            }
            else
            {
                throw new NotSupportedException($"Unsupported operating system {RuntimeInformation.OSDescription}");
            }
        }

        private void ConfigureForWindows()
        {
            return;
        }

        private void ConfigureForOSX()
        {
            return;
        }

        private void ConfigureForLinux()
        {
            var osRelease = new FileInfo("/etc/os-release");
            if (osRelease.Exists)
            {
                var map = File.ReadAllLines(osRelease.FullName)
                    .Select(line => line.Split('=', StringSplitOptions.RemoveEmptyEntries))
                    .Where(split => split.Length == 2)
                    .ToDictionary(split => split[0], split => split[1].Replace("\"", ""), StringComparer.OrdinalIgnoreCase);

                if (map.TryGetValue("ID", out var id) && map.TryGetValue("VERSION_ID", out var versionId))
                {
                    var architecture = RuntimeInformation.ProcessArchitecture;
                    var architectureName = architecture switch
                    {
                        Architecture.Arm => "arm32",
                        Architecture.Arm64 => "arm64",
                        Architecture.X64 => "x64",
                        Architecture.X86 => "x86",
                        _ => throw new NotSupportedException($"Unknown architecture: {architecture}"),
                    };

                    NativeLibrary.SetDllImportResolver(Assembly, (libraryName, assembly, searchPath) =>
                    {
                        Console.WriteLine($"Looking for library {libraryName} for {assembly.FullName} in ${id} ${versionId}");
                        if (MapLibraryName(assembly.Location, $"runtimes/{id}/{versionId}/{architectureName}/native/lib{libraryName}.so", out var mappedName)
                        || MapLibraryName(assembly.Location, $"runtimes/{id}/{architectureName}/native/lib{libraryName}.so", out mappedName))
                        {
                            return NativeLibrary.Load(mappedName, assembly, searchPath);
                        }
                        else
                        {
                            return NativeLibrary.Load(libraryName, assembly, searchPath);
                        }
                    });
                    return;
                }
                else
                {
                    throw new NotSupportedException($"Failed to find ID or VERSION_ID from {osRelease.FullName}");
                }
            }
            else
            {
                throw new NotSupportedException($"Failed to find {osRelease.FullName}");
            }
        }

        private static bool MapLibraryName(string assemblyLocation, string relative, [NotNullWhen(true)] out string? mappedName)
        {
            var candidate = Path.Combine(
                Path.GetDirectoryName(assemblyLocation)!,
                relative);

            Console.WriteLine($"Looking for {candidate}");

            if (File.Exists(candidate))
            {
                mappedName = candidate;
                return true;
            }
            else
            {
                mappedName = default;
                return false;
            }
        }
    }
}
