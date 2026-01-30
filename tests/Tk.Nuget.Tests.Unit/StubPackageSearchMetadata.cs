using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Tk.Nuget.Tests.Unit
{
    public record StubPackageSearchMetadata : IPackageSearchMetadata
    {
        public string Authors { get; set; }

        public IEnumerable<PackageDependencyGroup> DependencySets => throw new NotImplementedException();

        public string Description { get; set; }

        public long? DownloadCount { get; set; }

        public Uri IconUrl { get; set; }

        public PackageIdentity Identity { get; set; }

        public Uri LicenseUrl { get; set; }

        public Uri ProjectUrl { get; set; }

        public Uri ReadmeUrl { get; set; }

        public string ReadmeFileUrl { get; set; }

        public Uri ReportAbuseUrl { get; set; }

        public Uri PackageDetailsUrl { get; set; }

        public DateTimeOffset? Published { get; set; }

        public IReadOnlyList<string> OwnersList { get; set; }

        public string Owners { get; set; }

        public bool RequireLicenseAcceptance { get; set; }

        public string Summary { get; set; }

        public string Tags { get; set; }

        public string Title { get; set; }

        public bool IsListed { get; set; }

        public bool PrefixReserved { get; set; }

        public LicenseMetadata LicenseMetadata { get; set; }

        public IEnumerable<PackageVulnerabilityMetadata> Vulnerabilities { get; set; }

        public Task<PackageDeprecationMetadata> GetDeprecationMetadataAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VersionInfo>> GetVersionsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
