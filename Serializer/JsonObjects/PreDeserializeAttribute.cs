using json.Json;
using json.Objects;

namespace json.JsonObjects
{
    public class PreDeserializeAttribute : PreBuildAttribute
    {
        public PreDeserializeAttribute() : base(typeof(JsonParser), typeof(JObject)) { }

        public override NewWriter GetWriter()
        {
            return new JsonObjectWriter();
        }

        public override object GetContextValue(NewWriter parsedContext)
        {
            //return JsonObjectBuilder.GetResult(parsedContext);
            return null;
        }

        public override void ReadPreBuildResult(object preBuildResult, NewWriter valueFactory)
        {
            // TODO reimplement prebuild
            //JsonObjectReader.Read((JObject)preBuildResult, valueFactory);
        }
    }
}