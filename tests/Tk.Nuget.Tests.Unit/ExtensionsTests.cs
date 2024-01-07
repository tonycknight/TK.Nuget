using FluentAssertions;

namespace Tk.Nuget.Tests.Unit
{
    public class ExtensionsTests
    {
        [Theory]
        [InlineData(" ", "")]
        [InlineData("1.2.3", "1.2.3")]
        [InlineData("1.2.3-preview", "1.2.3-preview")]
        [InlineData("1.2.3+sha256value", "1.2.3")]
        public void NormaliseVersion_VersionsMapped(string vsn, string expected)
        {
            var r = vsn.NormaliseVersion();

            r.Should().Be(expected);
        }
    }
}
