using Shouldly;

namespace Tk.Nuget.Tests.Unit
{
    public class NugetClientTests
    {
        [Fact]
        public void GetLatestNugetVersionAsync_NullPackageId_ExceptionThrown()
        {
            var c = new NugetClient();

            var func = () => c.GetLatestNugetVersionAsync(null!, false, null);

            func.ShouldThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void GetLatestNugetVersionAsync_EmptyPackageId_ExceptionThrown(string id)
        {
            var c = new NugetClient();

            var func = () => c.GetLatestNugetVersionAsync(id, false, null);

            func.ShouldThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InlineData("Newtonsoft.Json")]
        [InlineData("Microsoft.Extensions.DependencyInjection")]
        public async Task GetLatestNugetVersionAsync_KnownPackage_VersionReturned(string id)
        {
            var c = new NugetClient();

            var vsn = await c.GetLatestNugetVersionAsync(id, false, null);

            // We've no control over version numbers, so we'll just assert that a version string is returned.
            var v = Version.Parse(vsn!);
            v.ToString().ShouldBe(vsn);
        }

        [Fact]
        public async Task GetLatestNugetVersionAsync_UnknownPackage_NoVersionReturned()
        {
            var id = Guid.NewGuid().ToString();
            var c = new NugetClient();

            var vsn = await c.GetLatestNugetVersionAsync(id, false, null);

            vsn.ShouldBeNull();
        }

        // We assume Newtonsoft.Json has a latest version of 13.0.3
        // This will change at some point, giving brittle tests!
        // However, the rate of upticks on Newtonsoft.Json is low at this time.
        [Theory]
        [InlineData("Newtonsoft.Json", "0.0.0", true)]
        [InlineData("Newtonsoft.Json", "13.0.2+sha256", true)]
        [InlineData("Newtonsoft.Json", "13.0.3", true)]
        [InlineData("Newtonsoft.Json", "13.0.3+sha256", true)]
        [InlineData("Newtonsoft.Json", "13.0.4+sha256", false)]
        [InlineData("Newtonsoft.Json", "13.0.4", false)]
        [InlineData("Newtonsoft.Json", "999.0.0", false)]
        [InlineData("Newtonsoft.Json", "999.0.0-preview", false)]
        [InlineData("Newtonsoft.Json", "999.0.0+sha256", false)]
        public async Task GetUpgradeVersionAsync__VersionReturned(string id, string currentVsn, bool upgradeExpected)
        {
            var c = new NugetClient();

            var vsn = await c.GetUpgradeVersionAsync(id, currentVsn, false, null);

            if (upgradeExpected)
            {
                vsn.ShouldNotBeNull();
            }
            else
            {
                vsn.ShouldBeNull();
            }
        }

        [Theory]
        [InlineData("Tk.Nuget")]
        [InlineData("pkgchk-cli")]
        [InlineData("Newtonsoft.Json")]
        [InlineData("Microsoft.Extensions.DependencyInjection")]
        public async Task GetMetadataAsync_KnownPackage_MetadataReturned(string id)
        {
            var c = new NugetClient();

            var meta = await c.GetMetadataAsync(id, CancellationToken.None, null);

            meta.ShouldNotBeNull();
            meta.Id.ShouldBe(id);
            meta.Description.ShouldNotBeEmpty();
            meta.Authors.ShouldNotBeEmpty();
            meta.Version.ShouldNotBeEmpty();
            meta.Title.ShouldNotBeEmpty();
            meta.Summary.ShouldNotBeEmpty();
        }

        [Theory]
        [InlineData("babb7044-6d80-4fa0-a756-b24260efd319")]        
        public async Task GetMetadataAsync_UnknownPackage_NullReturned(string id)
        {
            var c = new NugetClient();

            var meta = await c.GetMetadataAsync(id, CancellationToken.None, null);

            meta.ShouldBeNull();            
        }


        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetMetadataAsync_InvalidPackageId_ExceptionThrown(string id)
        {
            var c = new NugetClient();

            Func<Task<PackageMetadata?>> get = async () => await c.GetMetadataAsync(id, CancellationToken.None, null);

            get.ShouldThrow<ArgumentException>();
        }
    }
}