using Microsoft.Extensions.DependencyInjection;

namespace Tk.Nuget
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Add a <see cref="INugetClient"/> instance to the <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/> to populate.</param>
        /// <returns>The populated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddNugetClient(this IServiceCollection collection)
            => collection.AddSingleton<INugetClient, NugetClient>();
    }
}
