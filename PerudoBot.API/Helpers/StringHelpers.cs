using System.Text.RegularExpressions;

namespace PerudoBot.API.Helpers
{
    public static class StringHelpers
    {
        public static string StripSpecialCharacters(this string text)
        {
            return Regex.Replace(text, "[^0-9a-zA-Z ,.@<>$#:'\\\"!%\\-]+", "");
        }
    }
}
