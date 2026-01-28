using NuGet.Packaging;
using Shouldly;

namespace Tk.Nuget.Tests.Unit
{
    public class ExtensionsTests
    {
        [Theory]
        [MemberData(nameof(ToPackageMetadata_Maps_ValueReturned_Data))]
        public void ToPackageMetadata_Maps_ValueReturned(StubPackageSearchMetadata value)
        {
            var result = value.ToPackageMetadata();

            result.Id.ShouldBe(value.Identity.Id);
            result.Authors.ShouldBe(value.Authors);
            result.Description.ShouldBe(value.Description);
            result.DownloadCount.ShouldBe(value.DownloadCount);
            result.IconUrl.ShouldBe(value.IconUrl);
            result.License.ShouldBe(value.LicenseMetadata?.License);
            result.LicenseUrl.ShouldBe(value.LicenseUrl);
            result.ProjectUrl.ShouldBe(value.ProjectUrl);
            result.ReadmeUrl.ShouldBe(value.ReadmeUrl);
            result.Published.ShouldBe(value.Published);
            result.RequireLicenseAcceptance.ShouldBe(value.RequireLicenseAcceptance);
            result.Summary.ShouldBe(value.Summary);
            result.Tags.ShouldBe(value.Tags);
            result.Title.ShouldBe(value.Title);
        }

        public static IEnumerable<object[]> ToPackageMetadata_Maps_ValueReturned_Data()
        {
            yield return new[]
            {                
                new StubPackageSearchMetadata()
                {
                    Authors = "author",
                    Description = "description",
                    DownloadCount = 42,
                    IconUrl = new Uri("http://icon.url"),
                    Identity = new NuGet.Packaging.Core.PackageIdentity("packageId", new NuGet.Versioning.NuGetVersion("1.0.0")),
                    IsListed = true,
                    LicenseUrl = new Uri("http://license.url"),
                    Owners = "owner",
                    LicenseMetadata = new LicenseMetadata(LicenseType.Expression, "MIT", null, [], new Version(1,0,0)),
                    ProjectUrl = new Uri("http://project.url"),
                    OwnersList = new List<string> { "owner1", "owner2" },
                    Published = DateTimeOffset.UtcNow,
                    PackageDetailsUrl = new Uri("http://packagedetails.url"),
                    PrefixReserved = true,
                    RequireLicenseAcceptance = true,
                    Summary = "summary",
                    Tags = "tag1 tag2",
                    Title = "title",
                    ReadmeFileUrl = "http://readme.url",
                    ReadmeUrl = new Uri("http://readme.url"),
                    ReportAbuseUrl = new Uri("http://reportabuse.url"),
                    Vulnerabilities = []
                }
            };
        }
    }
}
