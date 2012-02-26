using json.Objects;

namespace json.Json
{
    public class PreDeserializeJsonAttribute : PreBuildAttribute
    {
        public PreDeserializeJsonAttribute() : base(typeof(JsonParser), typeof(string)) { }

        public override Writer GetWriter()
        {
            //return JsonStringBuilder.GetDefault();
            return null;
        }

        public override object GetContextValue(Writer parsedContext)
        {
            //return JsonStringBuilder.GetResult(parsedContext);
            return null;
        }

        public override void ReadPreBuildResult(object preBuildResult, Writer valueFactory)
        {
            //JsonParser.Parse((string)preBuildResult, valueFactory);//TODO reimplement
        }
    }
}
