namespace json.Objects
{
    public partial class TypedObjectBuilder
    {
        private class TypedObjectString : ParseString
        {
            public TypedObjectString(string value) : base(value) { }

            public override ParseObject AsObject()
            {
                return new TypedObjectObject(value);
            }
        }
    }
}
