using System.Collections.Generic;

namespace json
{
    public class JsonDictionary : Dictionary<string, object>
    {
        public string TypeIdentifier { get; set; }

        public void AddNumber (string name, double value)
        {
            this[name] = value;
        }

        public void AddString (string name, string value)
        {
            this[name] = value;
        }

        public void AddObject (string name, ParseObject value)
        {
            this[name] = value;
        }

        public void AddArray (string name, ParseArray value)
        {
            this[name] = value;
        }
    }
}
