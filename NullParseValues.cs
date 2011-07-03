
namespace json
{
    public class NullParseObject : ParseObjectBase
    {
        private static NullParseObject instance;
        public static NullParseObject Instance
        {
            get { return instance ?? (instance = new NullParseObject()); }
        }

        protected NullParseObject()
        {
        }

        public override void AddNull(string name)
        {
        }

        public override void AddBoolean(string name, bool value)
        {
        }

        public override void AddNumber(string name, double value)
        {
        }

        public override void AddString(string name, string value)
        {
        }

        public override void AddObject(string name, ParseObject value)
        {
        }

        public override void AddArray(string name, ParseArray value)
        {
        }
    }

    public class NullParseArray : ParseArrayBase
    {
        private static NullParseArray instance;
        public static NullParseArray Instance
        {
            get { return instance ?? (instance = new NullParseArray()); }
        }

        protected NullParseArray()
        {
        }

        public override void AddNull()
        {
        }

        public override void AddBoolean(bool value)
        {
        }

        public override void AddNumber(double value)
        {
        }

        public override void AddString(string value)
        {
        }

        public override void AddObject(ParseObject value)
        {
        }

        public override void AddArray(ParseArray value)
        {
        }

        public override ParseObject AsObject()
        {
            return NullParseObject.Instance;
        }
    }
}
