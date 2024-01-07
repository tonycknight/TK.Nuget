namespace Tk.Nuget
{
    public interface INugetClient
    {
        /// <summary>
        /// Get the latest version of a given Nuget package ID.
        /// </summary>
        /// <param name="packageId">The package ID.</param>
        /// <param name="includePrerelease">By default, prereleases are not included.</param>
        /// <param name="sourceUrl">The repo source URL for private package repositories.</param>
        /// <returns>The latest version if found, null if not.</returns>
        Task<string?> GetLatestNugetVersionAsync(string packageId, bool includePrerelease = false, string? sourceUrl = null);

        /// <summary>
        /// Gets a package version to upgrade to, if the current version is out of date.
        /// </summary>
        /// <param name="packageId">The package ID.</param>
        /// <param name="currentVersion">The version to test with.</param>
        /// <param name="includePrerelease">By default, prereleases are not included.</param>
        /// <param name="sourceUrl">The repo source URL for private package repositories.</param>
        /// <returns>The latest version if eligible, null if not.</returns>
        Task<string?> GetUpgradeVersionAsync(string packageId, string currentVersion, bool includePrerelease = false, string? sourceUrl = null);
    }
}
