namespace Tk.Nuget
{
    public interface INugetClient
    {
        Task<string?> GetLatestNugetVersionAsync(string packageId, bool includePrerelease = false, string? sourceUrl = null);

        Task<string?> GetUpgradeVersionAsync(string packageId, string currentVersion, bool includePrerelease = false, string? sourceUrl = null);
    }
}
