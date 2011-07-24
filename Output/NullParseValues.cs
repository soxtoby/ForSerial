
namespace json
{
    /// <summary>
    /// Ignores all inputs. Do not derive from this type unless you need a Null Object
    /// with extra methods, or are using the class for testing.
    /// </summary>
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

        public override void AddToObject(ParseObject obj, string name)
        {
        }

        public override void AddToArray(ParseArray array)
        {
        }
    }

    /// <summary>
    /// Ignores all inputs. Do not derive from this type unless you need a Null Object
    /// with extra methods, or are using the class for testing.
    /// </summary>
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

        public override ParseObject AsObject()
        {
            return NullParseObject.Instance;
        }

        public override void AddToObject(ParseObject obj, string name)
        {
        }

        public override void AddToArray(ParseArray array)
        {
        }
    }
}
