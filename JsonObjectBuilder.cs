using System.Collections.Generic;

namespace json
{
    public class JsonObjectBuilder : ParseValueFactory
    {
        public ParseObject CreateObject()
        {
            throw new System.NotImplementedException();
        }

        public ParseArray CreateArray()
        {
            throw new System.NotImplementedException();
        }

        public ParseNumber CreateNumber(double value)
        {
            throw new System.NotImplementedException();
        }

        public ParseString CreateString(string value)
        {
            throw new System.NotImplementedException();
        }

        public ParseBoolean CreateBoolean(bool value)
        {
            throw new System.NotImplementedException();
        }

        public ParseNull CreateNull()
        {
            throw new System.NotImplementedException();
        }
    }

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
