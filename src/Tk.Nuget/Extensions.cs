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

        public static string NormaliseVersion(this string version)
        {
            version = version.Trim();
            var shaIdx = version.IndexOf('+');
            if (shaIdx >= 0)
            {
                return version.Substring(0, shaIdx);
            }

            return version;
        }
    }
}
