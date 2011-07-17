using System.Text.RegularExpressions;

namespace json.Json.ValueFactory
{
    public partial class JsonStringBuilder
    {
        private class JsonStringString : ParseString
        {
            public JsonStringString(string value) : base(EscapeForJson(value)) { }

            // TODO escape control characters as well
            private static readonly Regex CharactersToEscape = new Regex(@"[""\\]", RegexOptions.Compiled);

            private static string EscapeForJson(string value)
            {
                return CharactersToEscape.Replace(value, @"\$0");
            }

            public override ParseObject AsObject()
            {
                ParseObject obj = new JsonStringObject();
                obj.AddString("value", value);
                return obj;
            }
        }
    }
}