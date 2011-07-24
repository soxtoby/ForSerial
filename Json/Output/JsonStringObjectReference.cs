
namespace json.Json
{
    public partial class JsonStringBuilder
    {
        private class JsonStringObjectReference : ParseObjectBase
        {
            private readonly uint referenceId;

            public JsonStringObjectReference(uint referenceId)
            {
                this.referenceId = referenceId;
            }

            public override string ToString()
            {
                return @"{""_ref"":" + referenceId + "}";
            }

            public override void AddToObject(ParseObject obj, string name)
            {
                ((JsonStringObject)obj).AddRegularProperty(name, this);
            }

            public override void AddToArray(ParseArray array)
            {
                ((JsonStringArray)array).AddRegularValue(this);
            }
        }
    }
}