using ForSerial.Json;
using ForSerial.Objects;

namespace ForSerial.JsonObjects
{
    public class PreDeserializeAttribute : PreBuildAttribute
    {
        public PreDeserializeAttribute() : base(typeof(JsonReader), typeof(JsonMap)) { }

        public override Writer GetWriter()
        {
            return new JsonObjectWriter();
        }

        public override object GetContextValue(Writer writer)
        {
            return ((JsonObjectWriter)writer).Result;
        }

        public override void ReadPreBuildResult(object preBuildResult, Writer writer)
        {
            JsonObjectReader.Read((JsonMap)preBuildResult, writer);
        }
    }
}