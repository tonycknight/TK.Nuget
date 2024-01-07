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

            var v = Version.Parse(vsn);
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
    }
}