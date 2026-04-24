namespace Tk.Nuget
{
    public record PackageDeprecation
    {
        public string Description { get; init; } = "";
        public AlternatePackage? AlternatePackage { get; init; }
        public IList<string> Reasons { get; init; } = [];
    }
}
