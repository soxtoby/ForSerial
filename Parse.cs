using System;
using json.Json;
using json.JsonObjects;
using json.Objects;

namespace json
{
    public class Parse
    {
        private Parse() { }

        private static Parse instance;
        public static Parse From
        {
            get { return instance = instance ?? new Parse(); }
        }
    }

    public class ParseExtensionPoint
    {
        private readonly Func<ParseValueFactory, ParseValue> parse;

        private ParseExtensionPoint(Func<ParseValueFactory, ParseValue> parseMethod)
        {
            parse = parseMethod;
        }

        public static ParseExtensionPoint Create(Func<ParseValueFactory, ParseValue> parseMethod)
        {
            if (parseMethod == null) throw new ArgumentNullException("parseMethod");

            return new ParseExtensionPoint(parseMethod);
        }

        public ParseValue Parse(ParseValueFactory valueFactory)
        {
            return parse(valueFactory);
        }
    }

    public static class ParseExtensions
    {
        public static ParseExtensionPoint Json(this Parse parse, string json)
        {
            return ParseExtensionPoint.Create(b => JsonParser.Parse(json, b));
        }

        public static ParseExtensionPoint Object(this Parse parse, object obj)
        {
            return ParseExtensionPoint.Create(b => ObjectParser.Parse(obj, b));
        }

        public static JsonObject ToJsonObject(this ParseExtensionPoint parser)
        {
            ParseValue result = parser.Parse(JsonObjectBuilder.Instance);
            ParseObject parseObject = result == null ? null : result.AsObject();
            return JsonObjectBuilder.GetResult(parseObject);
        }

        public static T ToObject<T>(this ParseExtensionPoint parser)
        {
            ParseValue result = parser.Parse(TypedObjectBuilder.Instance);
            return TypedObjectBuilder.GetResult<T>(result);
        }

        public static string ToJson(this ParseExtensionPoint parser)
        {
            ParseValue result = parser.Parse(JsonStringBuilder.Instance);
            return JsonStringBuilder.GetResult(result.AsObject());
        }

        public static string ToTypedJson(this ParseExtensionPoint parser)
        {
            ParseValue result = parser.Parse(TypedJsonStringBuilder.Instance);
            return JsonStringBuilder.GetResult(result.AsObject());
        }
    }
}
