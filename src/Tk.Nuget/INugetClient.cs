namespace Tk.Nuget
{
    public interface INugetClient
    {
        Task<string?> GetLatestNugetVersionAsync(string packageId, string? sourceUrl = null);
    }
}
