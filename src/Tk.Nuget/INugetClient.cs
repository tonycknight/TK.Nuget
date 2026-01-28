namespace Tk.Nuget
{
    /// <summary>
    /// A procedural abstraction over Nuget.
    /// </summary>
    public interface INugetClient
    {
        /// <summary>
        /// Get the latest version of a given Nuget package ID.
        /// </summary>
        /// <param name="packageId">The package ID.</param>
        /// <param name="includePrerelease">By default, prereleases are not included.</param>
        /// <param name="sourceUrl">The repo source URL for private package repositories. If null, https://api.nuget.org/v3/index.json is assumed,</param>
        /// <returns>The latest version if found, null if not.</returns>
        Task<string?> GetLatestNugetVersionAsync(string packageId, bool includePrerelease, CancellationToken cancellation, string? sourceUrl = null);

        /// <summary>
        /// Gets a package version to upgrade to, if the current version is out of date.
        /// </summary>
        /// <param name="packageId">The package ID.</param>
        /// <param name="currentVersion">The version to test with.</param>
        /// <param name="includePrerelease">By default, prereleases are not included.</param>
        /// <param name="sourceUrl">The repo source URL for private package repositories. If null, https://api.nuget.org/v3/index.json is assumed,</param>
        /// <returns>The latest version if eligible, null if not.</returns>
        Task<string?> GetUpgradeVersionAsync(string packageId, string currentVersion, bool includePrerelease, CancellationToken cancellation, string? sourceUrl = null);

        /// <summary>
        /// Gets a package's metadata, of the latest version.
        /// </summary>
        /// <param name="packageId">The package ID.</param>
        /// <param name="sourceUrl">The repo source URL for private package repositories. If null, https://api.nuget.org/v3/index.json is assumed,</param>
        /// <returns>Package metadata if the package exists, or null if the package does not exist.</returns>
        Task<PackageMetadata?> GetMetadataAsync(string packageId, CancellationToken cancellation, string? sourceUrl = null);
    }
}
