using System.Text.RegularExpressions;

namespace json.Json
{
    public partial class JsonStringBuilder
    {
        private class JsonStringString : StringOutput
        {
            public JsonStringString(string value) : base(EscapeForJson(value)) { }

            // TODO escape control characters as well
            private static readonly Regex CharactersToEscape = new Regex(@"[""\\]", RegexOptions.Compiled);

            private static string EscapeForJson(string value)
            {
                return CharactersToEscape.Replace(value, @"\$0");
            }

            public override OutputStructure AsStructure()
            {
                JsonStringObject obj = new JsonStringObject();
                obj.AddString("value", value);
                return obj;
            }

            public override void AddToStructure(OutputStructure structure, string name)
            {
                ((JsonStringObject)structure).AddString(name, value);
            }

            public override void AddToSequence(SequenceOutput sequence)
            {
                ((JsonStringArray)sequence).AddString(value);
            }
        }
    }
}