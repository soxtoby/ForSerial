using json.Objects;

namespace json.Json
{
    public class PreDeserializeJsonAttribute : PreBuildAttribute
    {
        public PreDeserializeJsonAttribute() : base(typeof(JsonParser), typeof(string)) { }

        public override Writer GetWriter()
        {
            return JsonStringBuilder.GetDefault();
        }

        public override object GetContextValue(Output parsedContext)
        {
            return JsonStringBuilder.GetResult(parsedContext);
        }

        public override void ReadPreBuildResult(object preBuildResult, Writer valueFactory)
        {
            //JsonParser.Parse((string)preBuildResult, valueFactory);//TODO reimplement
        }
    }
}
