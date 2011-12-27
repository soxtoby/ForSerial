namespace json.Objects
{
    public partial class TypedObjectBuilder
    {
        private class TypedObjectBoolean : ParseBoolean
        {
            private TypedObjectBoolean(bool value) : base(value) { }

            private static TypedObjectBoolean trueValue;
            public static TypedObjectBoolean True
            {
                get { return trueValue = trueValue ?? new TypedObjectBoolean(true); }
            }

            private static TypedObjectBoolean falseValue;
            public static TypedObjectBoolean False
            {
                get { return falseValue = falseValue ?? new TypedObjectBoolean(false); }
            }

            public override ParseObject AsObject()
            {
                return new TypedObjectObject(value);
            }

            public override void AddToObject(ParseObject obj, string name)
            {
                ((TypedObjectObject)obj).AddProperty(name, new TypedObjectPrimitive(value));
            }

            public override void AddToArray(ParseArray array)
            {
                ((TypedObjectArray)array).AddItem(value);
            }
        }
    }
}
