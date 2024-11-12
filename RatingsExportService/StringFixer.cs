using System.Text.RegularExpressions;

namespace RatingsExportService
{
    internal static class StringFixer
    {
        private static readonly Regex _regex = new(@"[\s\r\n]+");
        public static string Fix(this string? input) => string.IsNullOrEmpty(input) ? string.Empty : _regex.Replace(input, " ").Trim();
    }
}
