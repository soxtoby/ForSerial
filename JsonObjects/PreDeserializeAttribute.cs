using json.Json;
using json.Objects;

namespace json.JsonObjects
{
    public class PreDeserializeAttribute : PreBuildAttribute
    {
        public PreDeserializeAttribute() : base(typeof(JsonParser), typeof(JsonObject)) { }

        public override ParseValueFactory GetBuilder()
        {
            return JsonObjectBuilder.Instance;
        }

        public override object GetContextValue(ParseObject parsedContext)
        {
            return JsonObjectBuilder.GetResult(parsedContext);
        }

        public override void ParsePreBuildResult(object preBuildResult, ParseValueFactory valueFactory)
        {
            JsonObjectParser.Parse((JsonObject)preBuildResult, valueFactory);
        }
    }
}