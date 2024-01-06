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
    }
}