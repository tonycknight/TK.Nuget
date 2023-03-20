using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Tk.Nuget
{
    public class NugetClient : INugetClient
    {
        public async Task<string?> GetLatestNugetVersionAsync(string packageId, string? sourceUrl = null)
        {
            try
            {
                sourceUrl ??= NuGetConstants.V3FeedUrl;
                var logger = new NuGet.Common.NullLogger();
                var sourceRepository = Repository.Factory.GetCoreV3(new PackageSource(sourceUrl));
                var mdr = sourceRepository.GetResource<MetadataResource>();

                using var cache = new SourceCacheContext();

                var vsn = await mdr.GetLatestVersion(packageId, true, false, cache, logger, CancellationToken.None);

                return vsn?.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
