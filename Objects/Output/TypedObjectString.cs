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

            public override void AddToObject(ParseObject obj, string name)
            {
                ((TypedObjectObject)obj).AddProperty(name, value);
            }

            public override void AddToArray(ParseArray array)
            {
                ((TypedObjectArray)array).AddItem(value);
            }
        }
    }
}
