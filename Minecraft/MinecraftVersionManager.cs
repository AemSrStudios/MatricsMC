using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MatricsMC
{
    public class MinecraftVersion
    {
        public string Id { get; set; } = "";

        public string Type { get; set; } = "";

        public DateTime ReleaseTime { get; set; }

        public DateTime Time { get; set; }

        public string Url { get; set; } = "";

        public string DisplayName
        {
            get
            {
                return Id;
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public class MinecraftVersionManifest
    {
        public MinecraftLatestVersions Latest { get; set; } = new();

        public List<MinecraftVersion> Versions { get; set; } = new();
    }

    public class MinecraftLatestVersions
    {
        public string Release { get; set; } = "";

        public string Snapshot { get; set; } = "";
    }

    public class MinecraftVersionDetails
    {
        public string Id { get; set; } = "";

        public string Type { get; set; } = "";

        public DateTime ReleaseTime { get; set; }

        public DateTime Time { get; set; }

        public string MainClass { get; set; } = "";

        public List<MinecraftLibrary> Libraries { get; set; } = new();
    }

    public class MinecraftLibrary
    {
        public string Name { get; set; } = "";
    }

    public class MinecraftVersionManager
    {
        private const string ManifestUrl =
            "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";

        private static readonly HttpClient HttpClient =
            new HttpClient();

        private MinecraftVersionManifest? _manifest;

        public MinecraftVersionManager()
        {
        }

        public async Task<MinecraftVersionManifest> GetManifestAsync(
            CancellationToken cancellationToken = default)
        {
            if (_manifest != null)
            {
                return _manifest;
            }

            using HttpResponseMessage response =
                await HttpClient.GetAsync(
                    ManifestUrl,
                    cancellationToken);

            response.EnsureSuccessStatusCode();

            string json =
                await response.Content.ReadAsStringAsync(
                    cancellationToken);

            MinecraftVersionManifest? manifest =
                JsonSerializer.Deserialize<MinecraftVersionManifest>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (manifest == null)
            {
                throw new InvalidOperationException(
                    "Minecraft returned an invalid version list.");
            }

            if (manifest.Versions == null)
            {
                manifest.Versions =
                    new List<MinecraftVersion>();
            }

            _manifest = manifest;

            return manifest;
        }

        public async Task<MinecraftVersionDetails>
            GetVersionDetailsAsync(
                MinecraftVersion version,
                CancellationToken cancellationToken = default)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            return await GetVersionDetailsAsync(
                version.Id,
                cancellationToken);
        }

        public async Task<MinecraftVersionDetails>
            GetVersionDetailsAsync(
                string version,
                CancellationToken cancellationToken = default)
        {
            MinecraftVersion? minecraftVersion =
                await FindVersionAsync(
                    version,
                    cancellationToken);

            if (minecraftVersion == null)
            {
                throw new InvalidOperationException(
                    $"Minecraft version '{version}' was not found.");
            }

            using HttpResponseMessage response =
                await HttpClient.GetAsync(
                    minecraftVersion.Url,
                    cancellationToken);

            response.EnsureSuccessStatusCode();

            string json =
                await response.Content.ReadAsStringAsync(
                    cancellationToken);

            MinecraftVersionDetails? details =
                JsonSerializer.Deserialize<MinecraftVersionDetails>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (details == null)
            {
                throw new InvalidOperationException(
                    "Minecraft returned invalid version information.");
            }

            return details;
        }

        public async Task<List<MinecraftVersion>>
            GetReleaseVersionsAsync(
                CancellationToken cancellationToken = default)
        {
            MinecraftVersionManifest manifest =
                await GetManifestAsync(
                    cancellationToken);

            return manifest.Versions
                .Where(version =>
                    string.Equals(
                        version.Type,
                        "release",
                        StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public async Task<List<MinecraftVersion>>
            GetAllVersionsAsync(
                CancellationToken cancellationToken = default)
        {
            MinecraftVersionManifest manifest =
                await GetManifestAsync(
                    cancellationToken);

            return manifest.Versions;
        }

        public async Task<MinecraftVersion?>
            FindVersionAsync(
                string version,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                return null;
            }

            MinecraftVersionManifest manifest =
                await GetManifestAsync(
                    cancellationToken);

            return manifest.Versions.FirstOrDefault(
                item =>
                    string.Equals(
                        item.Id,
                        version.Trim(),
                        StringComparison.OrdinalIgnoreCase));
        }

        public async Task<string>
            GetLatestReleaseAsync(
                CancellationToken cancellationToken = default)
        {
            MinecraftVersionManifest manifest =
                await GetManifestAsync(
                    cancellationToken);

            return manifest.Latest.Release;
        }

        public async Task<string>
            GetLatestSnapshotAsync(
                CancellationToken cancellationToken = default)
        {
            MinecraftVersionManifest manifest =
                await GetManifestAsync(
                    cancellationToken);

            return manifest.Latest.Snapshot;
        }

        public void ClearCache()
        {
            _manifest = null;
        }
    }
}