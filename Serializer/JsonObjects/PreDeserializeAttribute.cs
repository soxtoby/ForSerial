using json.Json;
using json.Objects;

namespace json.JsonObjects
{
    public class PreDeserializeAttribute : PreBuildAttribute
    {
        public PreDeserializeAttribute() : base(typeof(JsonParser), typeof(JsonObject)) { }

        public override Writer GetWriter()
        {
            return new JsonObjectWriter();
        }

        public override object GetContextValue(Writer parsedContext)
        {
            //return JsonObjectBuilder.GetResult(parsedContext);
            return null;
        }

        public override void ReadPreBuildResult(object preBuildResult, Writer valueFactory)
        {
            // TODO reimplement prebuild
            //JsonObjectReader.Read((JsonObject)preBuildResult, valueFactory);
        }
    }
}