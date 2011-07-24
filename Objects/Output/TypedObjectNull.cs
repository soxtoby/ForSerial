namespace json.Objects
{
    public partial class TypedObjectBuilder
    {
        private class TypedObjectNull : ParseNull
        {
            private TypedObjectNull() { }

            private static TypedObjectNull value;
            public static TypedObjectNull Value
            {
                get { return value = value ?? new TypedObjectNull(); }
            }

            public override ParseObject AsObject()
            {
                return new TypedObjectObject((object)null);
            }

            public override void AddToObject(ParseObject obj, string name)
            {
                ((TypedObjectObject)obj).AddProperty(name, null);
            }

            public override void AddToArray(ParseArray array)
            {
                ((TypedObjectArray)array).AddItem(null);
            }
        }
    }
}
