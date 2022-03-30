namespace nomoretrolls.Formatting
{
    internal static class MarkdownExtensions
    {
        public static string ToMaxLength(this string value)
        {
            const int maxLength = 1000;
            var l = value.Length;
            return l > maxLength ? value.Substring(0, maxLength) : value;
        }

        public static string ToCode(this string value)
        {
            if (value.StartsWith("``") || value.EndsWith("``"))
                return value;
            else
                return $"``{value}``";
        }
    }
}
