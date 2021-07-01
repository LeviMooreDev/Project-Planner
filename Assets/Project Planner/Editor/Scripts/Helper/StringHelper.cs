using System.Globalization;

namespace ProjectPlanner
{
    public static class StringHelper
    {
        public static string ToTitleCase(this string val)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(val.ToLower());
        }

        public static string Prefix(this string val, string prefix)
        {
            return prefix + val;
        }
    }
}