using System.Diagnostics.CodeAnalysis;

namespace Tk.Nuget
{
    internal static class Extensions
    {
        [ExcludeFromCodeCoverage]
        public static T ArgNotNull<T>(this T value, string paramName) where T : class
        {
            if (ReferenceEquals(null, value))
            {
                throw new ArgumentNullException(paramName: paramName);
            }
            return value;
        }

        [ExcludeFromCodeCoverage]
        public static string ArgNotEmpty(this string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(paramName);
            }
            return value;
        }
    }
}
