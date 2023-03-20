using Microsoft.Extensions.DependencyInjection;

namespace Tk.Nuget
{
    public static class Extensions
    {
        public static IServiceCollection AddNugetClient(this IServiceCollection collection)
            => collection.AddSingleton<INugetClient, NugetClient>();
    }
}
