using System.IO.Compression;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Semver;

namespace Tk.Nuget
{
    public class NugetClient : INugetClient
    {
        /// <inheritdoc/>
        public async Task<string?> GetLatestNugetVersionAsync(string packageId, bool includePrerelease, CancellationToken cancellation = default, string? sourceUrl = null)
        {
            packageId.ArgNotNull(nameof(packageId));
            packageId.ArgNotEmpty(nameof(packageId));

            try
            {
                sourceUrl ??= NuGetConstants.V3FeedUrl;
                var logger = new NuGet.Common.NullLogger();
                var sourceRepository = Repository.Factory.GetCoreV3(new PackageSource(sourceUrl));
                var mdr = sourceRepository.GetResource<MetadataResource>();

                using var cache = new SourceCacheContext();

                var vsn = await mdr.GetLatestVersion(packageId, includePrerelease, false, cache, logger, cancellation);

                return vsn?.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<string?> GetUpgradeVersionAsync(string packageId, string currentVersion, bool includePrerelease, CancellationToken cancellation = default, string? sourceUrl = null)
        {
            packageId.ArgNotNull(nameof(packageId));
            packageId.ArgNotEmpty(nameof(packageId));

            var latestVersion = await GetLatestNugetVersionAsync(packageId, includePrerelease, cancellation, sourceUrl);

            if (latestVersion == null) return null;

            var latestVsn = SemVersion.Parse(latestVersion, SemVersionStyles.Any);
            var testVsn = SemVersion.Parse(currentVersion, SemVersionStyles.Any);

            if (latestVsn.ComparePrecedenceTo(testVsn) > 0)
            {
                return latestVersion;
            }
            return null;
        }

        /// <inheritdoc/>
        public async Task<PackageMetadata?> GetLatestMetadataAsync(string packageId, CancellationToken cancellation = default, string? sourceUrl = null)
        {
            packageId.ArgNotNull(nameof(packageId));
            packageId.ArgNotEmpty(nameof(packageId));

            var entries = await GetMetadataAsync(packageId, sourceUrl, cancellation, false);
            var metadata = entries.OrderByDescending(x => x.Identity.Version).FirstOrDefault();

            if (metadata != null)
            {
                return await metadata?.ToPackageMetadata()!;
            }
            return null;
        }

        /// <inheritdoc/>
        public async Task<PackageMetadata?> GetMetadataAsync(string packageId, string version, CancellationToken cancellation = default, string? sourceUrl = null)
        {
            packageId.ArgNotNull(nameof(packageId));
            packageId.ArgNotEmpty(nameof(packageId));

            var entries = await GetMetadataAsync(packageId, sourceUrl, cancellation, true);
            var metadata = entries.FirstOrDefault(x => x.Identity.Version.ToString() == version);

            if (metadata != null)
            {
                return await metadata?.ToPackageMetadata()!;
            }
            return null;
        }

        /// <inheritdoc/>
        public async Task<IList<PackageMetadata>> GetAllMetadataAsync(string packageId, bool includePrerelease, CancellationToken cancellation = default, string? sourceUrl = null)
        {
            packageId.ArgNotNull(nameof(packageId));
            packageId.ArgNotEmpty(nameof(packageId));

            var entries = await GetMetadataAsync(packageId, sourceUrl, cancellation, includePrerelease);

            var result = (entries ?? []).Select(x => x.ToPackageMetadata()).ToList();

            return await Task.WhenAll(result);
        }

        /// <inheritdoc/>
        public async Task<string> DownloadNugetPackageAsync(string packageId, string version, string targetPath, bool decompress, CancellationToken cancellation = default)
        {
            packageId.ArgNotNull(nameof(packageId));
            packageId.ArgNotEmpty(nameof(packageId));
            version.ArgNotNull(nameof(version));
            version.ArgNotEmpty(nameof(version));
            targetPath.ArgNotNull(nameof(targetPath));
            targetPath.ArgNotEmpty(nameof(targetPath));

            try
            {
                var sourceUrl = NuGetConstants.V3FeedUrl;
                var logger = new NuGet.Common.NullLogger();
                var sourceRepository = Repository.Factory.GetCoreV3(new PackageSource(sourceUrl));

                using var cache = new SourceCacheContext();

                // Get FindPackageByIdResource to verify package version exists
                var findResource = await sourceRepository.GetResourceAsync<FindPackageByIdResource>(cancellation);
                var allVersions = await findResource.GetAllVersionsAsync(packageId, cache, logger, cancellation);
                var nugetVersion = NuGetVersion.Parse(version);

                if (!allVersions.Contains(nugetVersion))
                {
                    throw new InvalidOperationException($"Package '{packageId}' version '{version}' not found in {sourceUrl}.");
                }

                // Create target directory if it doesn't exist
                var targetDir = Path.GetDirectoryName(targetPath);
                if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                targetPath = await DownloadPackageAsync(packageId, version, targetPath, nugetVersion, cancellation);

                // Decompress if requested
                if (decompress)
                {
                    targetPath = ExtractPackage(targetPath);
                }
                return targetPath;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to download package '{packageId}' version '{version}'.", ex);
            }
        }

        private static string ExtractPackage(string targetPath)
        {
            var extractPath = Path.Combine(
                                    Path.GetDirectoryName(targetPath) ?? ".",
                                    Path.GetFileNameWithoutExtension(targetPath)
                                );

            if (Directory.Exists(extractPath))
            {
                Directory.Delete(extractPath, true);
            }

            ZipFile.ExtractToDirectory(targetPath, extractPath);
            File.Delete(targetPath); // Delete the original .nupkg file after extraction

            return extractPath;
        }

        private static async Task<string> DownloadPackageAsync(string packageId, string version, string targetPath, NuGetVersion nugetVersion, CancellationToken cancellation)
        {
            // Download the package from the content repository
            // Standard NuGet package download URL format
            var nugetOrgUrl = "https://api.nuget.org/v3-flatcontainer";
            var packageUrl = $"{nugetOrgUrl}/{packageId.ToLowerInvariant()}/{nugetVersion.ToNormalizedString()}/{packageId.ToLowerInvariant()}.{nugetVersion.ToNormalizedString()}.nupkg";

            using (var client = new System.Net.Http.HttpClient())
            {
                using (var response = await client.GetAsync(packageUrl, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellation))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new InvalidOperationException($"Package '{packageId}' version '{version}' could not be downloaded.");
                    }

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        using (var fileStream = File.Create(targetPath))
                        {
                            await contentStream.CopyToAsync(fileStream, cancellation);

                            return fileStream.Name;
                        }
                    }
                }
            }
        }

        private static async Task<IList<IPackageSearchMetadata>> GetMetadataAsync(string packageId, string? sourceUrl, CancellationToken cancellation, bool includePrerelease)
        {
            sourceUrl ??= NuGetConstants.V3FeedUrl;
            var logger = new NuGet.Common.NullLogger();
            var sourceRepository = Repository.Factory.GetCoreV3(new PackageSource(sourceUrl));
            var mdr = await sourceRepository.GetResourceAsync<PackageMetadataResource>(cancellation);

            using var cache = new SourceCacheContext();

            var results = (await mdr.GetMetadataAsync(packageId, includePrerelease, false, cache, logger, cancellation));

            return results.OrderByDescending(x => x.Identity.Version).ToList();
        }
    }
}
