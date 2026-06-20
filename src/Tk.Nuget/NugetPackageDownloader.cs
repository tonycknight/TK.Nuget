using System.IO.Compression;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace Tk.Nuget
{
    internal class NugetPackageDownloader
    {
        public async Task<string> DownloadPackageAsync(string packageId, string version, string targetPath, bool decompress, CancellationToken cancellation = default)
        {
            try
            {
                var nugetVersion = await GetVerifiedNugetVersionAsync(packageId, version, cancellation);

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

        private static async Task<NuGetVersion> GetVerifiedNugetVersionAsync(string packageId, string version, CancellationToken cancellation)
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

            return nugetVersion;
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
    }
}
