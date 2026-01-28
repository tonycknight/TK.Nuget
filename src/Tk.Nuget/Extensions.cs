using System.Diagnostics.CodeAnalysis;
using NuGet.Protocol.Core.Types;

namespace Tk.Nuget
{
    internal static class Extensions
    {
        [ExcludeFromCodeCoverage]
        public static T ArgNotNull<T>(this T value, string paramName) where T : class
        {
            if (ReferenceEquals(null, value))
            {
                throw new ArgumentNullException(paramName: paramName);
            }
            return value;
        }

        [ExcludeFromCodeCoverage]
        public static string ArgNotEmpty(this string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(paramName);
            }
            return value;
        }

        public static PackageMetadata ToPackageMetadata(this IPackageSearchMetadata value) => new PackageMetadata()
        {
            Id = value.Identity.Id,
            Version = value.Identity?.Version?.ToNormalizedString() ?? "",
            Authors = value.Authors,
            Title = value.Title,
            Description = value.Description,
            Summary = value.Summary,
            DownloadCount = value.DownloadCount,
            IconUrl = value.IconUrl,
            LicenseUrl = value.LicenseUrl,
            License = value.LicenseMetadata?.License,
            ProjectUrl = value.ProjectUrl,
            ReadmeUrl = value.ReadmeUrl,
            Published = value.Published,
            RequireLicenseAcceptance = value.RequireLicenseAcceptance,
            Tags = value.Tags,
        };
    }
}
