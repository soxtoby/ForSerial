namespace json
{
    internal class TestParseObject : ParseObjectBase
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

    internal class TestParseArray : ParseArrayBase
    {
        public override void AddNull()
        {
            throw new System.NotImplementedException();
        }

        public override void AddBoolean(bool value)
        {
            throw new System.NotImplementedException();
        }

        public override void AddNumber(double value)
        {
            throw new System.NotImplementedException();
        }

        public override void AddString(string value)
        {
            throw new System.NotImplementedException();
        }

        public override void AddObject(ParseObject value)
        {
            throw new System.NotImplementedException();
        }

        public override void AddArray(ParseArray value)
        {
            throw new System.NotImplementedException();
        }

        public override ParseObject AsObject()
        {
            throw new System.NotImplementedException();
        }
    }

    internal class TestParseString : ParseString
    {
        public TestParseString(string value) : base(value) { }

        public override ParseObject AsObject()
        {
            TestParseObject obj = new TestParseObject();
            obj.AddString("value", value);
            return obj;
        }
    }

    internal class TestValueFactory : ParseValueFactory
    {
        public virtual ParseObject CreateObject()
        {
            return new TestParseObject();
        }

        public virtual ParseArray CreateArray()
        {
            return new TestParseArray();
        }

        public virtual ParseNumber CreateNumber(double value)
        {
            throw new System.NotImplementedException();
        }

        public virtual ParseString CreateString(string value)
        {
            return new TestParseString(value);
        }

        public virtual ParseBoolean CreateBoolean(bool value)
        {
            throw new System.NotImplementedException();
        }

        public virtual ParseNull CreateNull()
        {
            throw new System.NotImplementedException();
        }
    }
}