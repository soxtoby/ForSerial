namespace ForSerial.Json
{
    internal static class JsonStringEscaper
    {
        private const string Backslash = "\\";
        private const string EscapedBackslash = "\\\\";
        private const string QuoteString = "\"";
        private const string EscapedQuote = "\\\"";
        private const string Return = "\r";
        private const string NewLine = "\n";
        private const string EscapedReturn = "\\r";
        private const string EscapedNewLine = "\\n";

        public static string EscapeForJson(string value)
        {
            return value
                .Replace(Backslash, EscapedBackslash)
                .Replace(QuoteString, EscapedQuote)
                .Replace(Return, EscapedReturn)
                .Replace(NewLine, EscapedNewLine);
        }

        public static string UnescapeString(string value)
        {
            return value
                .Replace(EscapedBackslash, Backslash)
                .Replace(EscapedQuote, QuoteString)
                .Replace(EscapedReturn, Return)
                .Replace(EscapedNewLine, NewLine);
        }
    }
}