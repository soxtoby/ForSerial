using json.Objects;

namespace json.Json
{
    public class PreDeserializeJsonAttribute : PreBuildAttribute
    {
        public PreDeserializeJsonAttribute() : base(typeof(JsonParser), typeof(string)) { }

        public override ParseValueFactory GetBuilder()
        {
            return JsonStringBuilder.Instance;
        }

        public override object GetContextValue(ParseObject parsedContext)
        {
            return JsonStringBuilder.GetResult(parsedContext);
        }

        public override void ParsePreBuildResult(object preBuildResult, ParseValueFactory valueFactory)
        {
            JsonParser.Parse((string)preBuildResult, valueFactory);
        }
    }
}