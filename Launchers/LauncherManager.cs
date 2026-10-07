using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MatricsMC
{
    public class DetectedLauncher
    {
        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public string Path { get; set; } = "";

        public string Type { get; set; } = "";

        public string ExecutablePath { get; set; } = "";

        public bool IsInstalled { get; set; }

        public bool IsSelected { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public static class LauncherManager
    {
        public static List<DetectedLauncher>
            DetectLaunchers()
        {
            List<DetectedLauncher> launchers = new();

            string appData =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData);

            string localAppData =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData);

            AddLauncher(
                launchers,
                "Minecraft Launcher",
                "Official Minecraft Launcher",
                Path.Combine(
                    appData,
                    ".minecraft"),
                "Official",
                FindOfficialLauncher());

            AddLauncher(
                launchers,
                "Prism Launcher",
                "Prism Launcher",
                Path.Combine(
                    appData,
                    "PrismLauncher"),
                "Prism",
                FindExecutable(
                    Path.Combine(
                        localAppData,
                        "Programs",
                        "PrismLauncher"),
                    "prismlauncher.exe"));

            AddLauncher(
                launchers,
                "MultiMC",
                "MultiMC Launcher",
                Path.Combine(
                    appData,
                    "MultiMC"),
                "MultiMC",
                FindExecutable(
                    Path.Combine(
                        localAppData,
                        "Programs",
                        "MultiMC"),
                    "MultiMC.exe"));

            AddLauncher(
                launchers,
                "ATLauncher",
                "ATLauncher",
                Path.Combine(
                    appData,
                    "ATLauncher"),
                "ATLauncher",
                FindExecutable(
                    Path.Combine(
                        localAppData,
                        "Programs",
                        "ATLauncher"),
                    "ATLauncher.exe"));

            AddLauncher(
                launchers,
                "CurseForge",
                "CurseForge Minecraft Launcher",
                Path.Combine(
                    localAppData,
                    "Overwolf",
                    "CurseForge"),
                "CurseForge",
                FindExecutable(
                    Path.Combine(
                        localAppData,
                        "Programs",
                        "CurseForge"),
                    "CurseForge.exe"));

            AddLauncher(
                launchers,
                "Modrinth App",
                "Modrinth Minecraft Launcher",
                Path.Combine(
                    localAppData,
                    "ModrinthApp"),
                "Modrinth",
                FindExecutable(
                    Path.Combine(
                        localAppData,
                        "Programs",
                        "ModrinthApp"),
                    "Modrinth App.exe"));

            AddLauncher(
                launchers,
                "Lunar Client",
                "Lunar Client",
                Path.Combine(
                    localAppData,
                    "Programs",
                    "lunarclient"),
                "Lunar",
                FindExecutable(
                    Path.Combine(
                        localAppData,
                        "Programs",
                        "lunarclient"),
                    "Lunar Client.exe"));

            AddLauncher(
                launchers,
                "Badlion Client",
                "Badlion Client",
                Path.Combine(
                    localAppData,
                    "Badlion Client"),
                "Badlion",
                FindExecutable(
                    Path.Combine(
                        localAppData,
                        "Badlion Client"),
                    "Badlion Client.exe"));

            AddLauncher(
                launchers,
                "GDLauncher",
                "GDLauncher",
                Path.Combine(
                    localAppData,
                    "Programs",
                    "gdlauncher"),
                "GDLauncher",
                FindExecutable(
                    Path.Combine(
                        localAppData,
                        "Programs",
                        "gdlauncher"),
                    "GDLauncher.exe"));

            AddLauncher(
                launchers,
                "SKLauncher",
                "SKLauncher",
                Path.Combine(
                    appData,
                    ".sklauncher"),
                "SKLauncher",
                FindExecutable(
                    Path.Combine(
                        localAppData,
                        "Programs",
                        "SKlauncher"),
                    "SKlauncher.exe"));

            AddLauncher(
                launchers,
                "HMCL",
                "Hello Minecraft Launcher",
                Path.Combine(
                    appData,
                    ".hmcl"),
                "HMCL",
                FindExecutable(
                    Path.Combine(
                        localAppData,
                        "Programs",
                        "HMCL"),
                    "HMCL.exe"));

            return launchers;
        }

        private static void AddLauncher(
            List<DetectedLauncher> launchers,
            string name,
            string description,
            string path,
            string type,
            string executablePath)
        {
            bool directoryExists =
                Directory.Exists(path);

            bool executableExists =
                !string.IsNullOrWhiteSpace(executablePath) &&
                File.Exists(executablePath);

            launchers.Add(
                new DetectedLauncher
                {
                    Name = name,
                    Description = description,
                    Path = path,
                    Type = type,
                    ExecutablePath =
                        executablePath ?? "",
                    IsInstalled =
                        directoryExists ||
                        executableExists
                });
        }

        private static string FindExecutable(
            string directory,
            string fileName)
        {
            if (!Directory.Exists(directory))
            {
                return "";
            }

            string direct =
                Path.Combine(
                    directory,
                    fileName);

            if (File.Exists(direct))
            {
                return direct;
            }

            try
            {
                string? found =
                    Directory
                        .EnumerateFiles(
                            directory,
                            fileName,
                            SearchOption.AllDirectories)
                        .FirstOrDefault();

                return found ?? "";
            }
            catch
            {
                return "";
            }
        }

        private static string FindOfficialLauncher()
        {
            string localAppData =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData);

            string appData =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData);

            string[] possiblePaths =
            {
                Path.Combine(
                    localAppData,
                    "Packages",
                    "Microsoft.4297127D64EC6_8wekyb3d8bbwe",
                    "LocalCache",
                    "Local",
                    "game",
                    "launcher.exe"),

                Path.Combine(
                    localAppData,
                    "MinecraftLauncher",
                    "MinecraftLauncher.exe"),

                Path.Combine(
                    appData,
                    ".minecraft",
                    "MinecraftLauncher.exe")
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return "";
        }

        public static List<DetectedLauncher>
            GetInstalledLaunchers()
        {
            return DetectLaunchers()
                .Where(
                    launcher =>
                        launcher.IsInstalled)
                .ToList();
        }

        public static DetectedLauncher?
            FindLauncher(string name)
        {
            return DetectLaunchers()
                .FirstOrDefault(
                    launcher =>
                        string.Equals(
                            launcher.Name,
                            name,
                            StringComparison.OrdinalIgnoreCase));
        }
    }
}