using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Tk.Nuget
{
    internal class MetadataReader
    {
        private readonly string _packageId;
        private readonly string _sourceId;

        public MetadataReader(string packageId, string? sourceUrl)
        {
            _packageId = packageId;
            _sourceId = sourceUrl ??= NuGetConstants.V3FeedUrl;
        }

        public async Task<IList<IPackageSearchMetadata>> GetMetadataAsync(bool includePrerelease, CancellationToken cancellation)
        {
            var logger = new NuGet.Common.NullLogger();
            var sourceRepository = Repository.Factory.GetCoreV3(new PackageSource(_sourceId));
            var mdr = await sourceRepository.GetResourceAsync<PackageMetadataResource>(cancellation);

            using var cache = new SourceCacheContext();

            var results = (await mdr.GetMetadataAsync(_packageId, includePrerelease, false, cache, logger, cancellation));

            return results.OrderByDescending(x => x.Identity.Version).ToList();
        }
    }
}
