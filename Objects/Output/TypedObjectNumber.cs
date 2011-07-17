namespace json.Objects
{
    public partial class TypedObjectBuilder
    {
        private class TypedObjectNumber : ParseNumber
        {
            public TypedObjectNumber(double value) : base(value) { }

            public override ParseObject AsObject()
            {
                return new TypedObjectObject(value);
            }
        }
    }
}
