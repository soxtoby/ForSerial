
namespace json.Json
{
    public partial class JsonStringBuilder
    {
        private class JsonStringObjectReference : OutputStructureBase
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

            public override void AddToStructure(OutputStructure structure, string name)
            {
                ((JsonStringObject)structure).AddRegularProperty(name, this);
            }

            public override void AddToSequence(SequenceOutput sequence)
            {
                ((JsonStringArray)sequence).AddRegularValue(this);
            }
        }
    }
}