namespace Tk.Nuget
{
    public record AlternatePackage
    {
        public string Name { get; init; } = "";
        public string Range { get; init; } = "";
    }
}
