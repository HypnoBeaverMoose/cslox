namespace Lox
{
    public static class StringExtensions
    {
        public static string SubstringFromTo(this string input, int start, int end)
        {
            return input.Substring(start, end - start);
        }
    }
}