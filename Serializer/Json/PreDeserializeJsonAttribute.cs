using json.Objects;

namespace json.Json
{
    public class PreDeserializeJsonAttribute : PreBuildAttribute
    {
        public PreDeserializeJsonAttribute() : base(typeof(JsonParser), typeof(string)) { }

        public override NewWriter GetWriter()
        {
            //return JsonStringBuilder.GetDefault();
            return null;
        }

        public override object GetContextValue(NewWriter parsedContext)
        {
            //return JsonStringBuilder.GetResult(parsedContext);
            return null;
        }

        public override void ReadPreBuildResult(object preBuildResult, NewWriter valueFactory)
        {
            //JsonParser.Parse((string)preBuildResult, valueFactory);//TODO reimplement
        }
    }
}
