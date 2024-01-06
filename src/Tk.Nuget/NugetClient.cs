using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Tk.Nuget
{
    public class NugetClient : INugetClient
    {
        public async Task<string?> GetLatestNugetVersionAsync(string packageId, bool includePrerelease = false, string? sourceUrl = null)
        {
            if (packageId == null) throw new ArgumentNullException(nameof(packageId));
            if (string.IsNullOrWhiteSpace(packageId)) throw new ArgumentException(nameof(packageId));


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
    }
}
