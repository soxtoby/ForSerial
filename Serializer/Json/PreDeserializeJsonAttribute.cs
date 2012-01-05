using json.Objects;

namespace json.Json
{
    public class PreDeserializeJsonAttribute : PreBuildAttribute
    {
        public PreDeserializeJsonAttribute() : base(typeof(JsonParser), typeof(string)) { }

        public override Writer GetWriter()
        {
            return JsonStringBuilder.Default;
        }

        public override object GetContextValue(OutputStructure parsedContext)
        {
            return JsonStringBuilder.GetResult(parsedContext);
        }

        public override void ReadPreBuildResult(object preBuildResult, Writer valueFactory)
        {
            JsonParser.Parse((string)preBuildResult, valueFactory);
        }
    }
}