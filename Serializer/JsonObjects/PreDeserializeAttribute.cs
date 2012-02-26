using json.Json;
using json.Objects;

namespace json.JsonObjects
{
    public class PreDeserializeAttribute : PreBuildAttribute
    {
        public PreDeserializeAttribute() : base(typeof(JsonParser), typeof(JsonObject)) { }

        public override Writer GetWriter()
        {
            return JsonObjectBuilder.Instance;
        }

        public override object GetContextValue(Output parsedContext)
        {
            //return JsonObjectBuilder.GetResult(parsedContext);
            return null;
        }

        public override void ReadPreBuildResult(object preBuildResult, Writer valueFactory)
        {
            // TODO reimplement prebuild
            //JsonObjectReader.Read((JObject)preBuildResult, valueFactory);
        }
    }
}