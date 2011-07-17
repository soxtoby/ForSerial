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
        }
    }
}
