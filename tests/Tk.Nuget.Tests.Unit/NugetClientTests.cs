using FluentAssertions;

namespace Tk.Nuget.Tests.Unit
{
    public class NugetClientTests
    {
        [Fact]
        public void GetLatestNugetVersionAsync_NullPackageId_ExceptionThrown()
        {
            var c = new NugetClient();

            var func = () => c.GetLatestNugetVersionAsync(null!, false, null);

            func.Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void GetLatestNugetVersionAsync_EmptyPackageId_ExceptionThrown(string id)
        {
            var c = new NugetClient();

            var func = () => c.GetLatestNugetVersionAsync(id, false, null);

            func.Should().ThrowAsync<ArgumentNullException>();
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
            v.ToString().Should().Be(vsn);
        }

        [Fact]
        public async Task GetLatestNugetVersionAsync_UnknownPackage_NoVersionReturned()
        {
            var id = Guid.NewGuid().ToString();
            var c = new NugetClient();

            var vsn = await c.GetLatestNugetVersionAsync(id, false, null);

            vsn.Should().BeNull();
        }

        // We assume Newtonsoft.Json has a latest version of 13.0.3
        // This will change at some point, giving brittle tests!
        // However, the rate of upticks on Newtonsoft.Json is low at this time.
        [Theory]
        [InlineData("Newtonsoft.Json", "0.0.0", true)]
        [InlineData("Newtonsoft.Json", "13.0.2+sha256", true)]
        [InlineData("Newtonsoft.Json", "13.0.3", false)]
        [InlineData("Newtonsoft.Json", "13.0.3+sha256", false)]
        [InlineData("Newtonsoft.Json", "13.0.4+sha256", false)]
        [InlineData("Newtonsoft.Json", "13.0.4-preview", false)]
        [InlineData("Newtonsoft.Json", "999.0.0", false)]
        [InlineData("Newtonsoft.Json", "999.0.0-preview", false)]
        [InlineData("Newtonsoft.Json", "999.0.0+sha256", false)]
        public async Task GetUpgradeVersionAsync__VersionReturned(string id, string currentVsn, bool upgradeExpected)
        {
            var c = new NugetClient();

            var vsn = await c.GetUpgradeVersionAsync(id, currentVsn, false, null);

            if (upgradeExpected)
            {
                vsn.Should().NotBeNull();
            }
            else
            {
                vsn.Should().BeNull();
            }
        }
    }
}