
namespace json
{
    public class NullParseObject : ParseObjectBase
    {
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
            return new NullParseObject();
        }
    }
}
