using System.Diagnostics.CodeAnalysis;
using NuGet.Protocol;
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

        public static async Task<PackageMetadata> ToPackageMetadata(this IPackageSearchMetadata value) =>
            new PackageMetadata()
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
                Deprecation = (await value.GetDeprecationMetadataAsync())?.ToPackageDeprecation(),
                Vulnerabilities = value.Vulnerabilities.ToPackageVulnerabilities()
            };

        private static IList<PackageVulnerability> ToPackageVulnerabilities(this IEnumerable<PackageVulnerabilityMetadata>? values) =>
            (values ?? []).Select(v => new PackageVulnerability()
            {
                AdvisoryUrl = v.AdvisoryUrl.ToString(),
                Severity = v.Severity.ToSeverityString(),

            }).ToArray();

        private static PackageDeprecation ToPackageDeprecation(this PackageDeprecationMetadata value) =>
            new PackageDeprecation()
            {
                Description = value.Message ?? "",
                AlternatePackage = value.AlternatePackage?.ToPackageAlternate(),
                Reasons = (value.Reasons ?? []).ToArray(),
            };

        private static AlternatePackage ToPackageAlternate(this AlternatePackageMetadata value) =>
            new AlternatePackage()
            {
                Name = value.PackageId ?? "",
                Range = value.Range?.ToString() ?? "",
            };

        private static PackageVulnerabilitySeverity ToSeverityString(this int severity) =>
            severity switch
            {
                0 => PackageVulnerabilitySeverity.Low,
                1 => PackageVulnerabilitySeverity.Medium,
                2 => PackageVulnerabilitySeverity.High,
                3 => PackageVulnerabilitySeverity.Critical,
                _ => PackageVulnerabilitySeverity.Unknown
            };
    }
}
