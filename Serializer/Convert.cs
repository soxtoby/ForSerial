using System;
using json.Json;
using json.JsonObjects;
using json.Objects;

namespace json
{
    public class Convert
    {
        private Convert() { }

        private static Convert instance;
        public static Convert From
        {
            get { return instance = instance ?? new Convert(); }
        }
    }

    public class ConversionExtensionPoint
    {
        private readonly Func<Writer, Output> parse;

        private ConversionExtensionPoint(Func<Writer, Output> parseMethod)
        {
            parse = parseMethod;
        }

        public static ConversionExtensionPoint Create(Func<Writer, Output> parseMethod)
        {
            if (parseMethod == null) throw new ArgumentNullException("parseMethod");

            return new ConversionExtensionPoint(parseMethod);
        }

        public Output WithBuilder(Writer valueFactory)
        {
            return parse(valueFactory);
        }
    }

    public static class ConversionExtensions
    {
        public static ConversionExtensionPoint Json(this Convert convert, string json)
        {
            return null;
            //return ConversionExtensionPoint.Create(b => JsonParser.Parse(json, b));   // TODO probably just remove all this crappy fluent stuff
        }

        public static ConversionExtensionPoint JsonObject(this Convert convert, JsonObject obj)
        {
            return null;
            //return ConversionExtensionPoint.Create(b => JsonObjectReader.Read(obj, b));
        }

        public static ConversionExtensionPoint Object(this Convert convert, object obj, ObjectParsingOptions options = null)
        {
            return null;
            //return ConversionExtensionPoint.Create(b => ObjectReader.Read(obj, b, options ?? new ObjectParsingOptions()));
        }

        public static object ToJsonObject(this ConversionExtensionPoint parser)
        {
            Output result = parser.WithBuilder(JsonObjectBuilder.Instance);
            OutputStructure outputStructure = result == null ? null : result.AsStructure();
            return JsonObjectBuilder.GetResult(outputStructure);
        }

        public static T ToObject<T>(this ConversionExtensionPoint parser)
        {
            Output result = parser.WithBuilder(new TypedObjectBuilder(typeof(T)));
            return TypedObjectBuilder.GetResult<T>(result);
        }

        public static string ToJson(this ConversionExtensionPoint parser, JsonStringBuilder.Options options = JsonStringBuilder.Options.Default)
        {
            JsonStringBuilder jsonStringBuilder = options == JsonStringBuilder.Options.Default
                ? JsonStringBuilder.GetDefault()
                : new JsonStringBuilder(options);
            Output result = parser.WithBuilder(jsonStringBuilder);
            return JsonStringBuilder.GetResult(result.AsStructure());
        }

        public static string ToTypedJson(this ConversionExtensionPoint parser)
        {
            Output result = parser.WithBuilder(TypedJsonStringBuilder.Instance);
            return JsonStringBuilder.GetResult(result.AsStructure());
        }
    }
}
