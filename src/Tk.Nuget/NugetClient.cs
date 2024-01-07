using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using Semver;

namespace Tk.Nuget
{
    public class NugetClient : INugetClient
    {
        /// <inheritdoc/>
        public async Task<string?> GetLatestNugetVersionAsync(string packageId, bool includePrerelease = false, string? sourceUrl = null)
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

                var vsn = await mdr.GetLatestVersion(packageId, includePrerelease, false, cache, logger, CancellationToken.None);

                return vsn?.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<string?> GetUpgradeVersionAsync(string packageId, string currentVersion, bool includePrerelease = false, string? sourceUrl = null)
        {
            packageId.ArgNotNull(nameof(packageId));
            packageId.ArgNotEmpty(nameof(packageId));

            var latestVersion = await this.GetLatestNugetVersionAsync(packageId, includePrerelease, sourceUrl);

            if (latestVersion == null) return null;

            var latestVsn = SemVersion.Parse(latestVersion, SemVersionStyles.Any);
            var testVsn = SemVersion.Parse(currentVersion, SemVersionStyles.Any);

            if (latestVsn.ComparePrecedenceTo(testVsn) > 0)
            {
                return latestVersion;
            }
            return null;
        }
    }
}
