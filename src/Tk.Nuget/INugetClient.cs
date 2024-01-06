namespace Tk.Nuget
{
    public interface INugetClient
    {
        Task<string?> GetLatestNugetVersionAsync(string packageId, bool includePrerelease = false, string? sourceUrl = null);
    }
}
