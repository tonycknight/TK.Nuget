namespace Tk.Nuget
{
    public record PackageMetadata
    {
        public string Id { get; init; } = string.Empty;
        public string Version { get; init; } = string.Empty;
        public string Authors { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public long? DownloadCount { get; init; }
        public Uri? IconUrl { get; init; }
        public Uri? LicenseUrl { get; init; }
        public string? License { get; init; }
        public Uri? ProjectUrl { get; init; }
        public Uri? ReadmeUrl { get; init; }
        public DateTimeOffset? Published { get; init; }
        public bool RequireLicenseAcceptance { get; init; }
        public string Summary { get; init; } = string.Empty;
        public string Tags { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
    }
}
