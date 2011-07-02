using System;

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

    internal class TestParseNumber : ParseNumber
    {
        public TestParseNumber(double value) : base(value) { }

        public override ParseObject AsObject()
        {
            throw new NotImplementedException();
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
            return new TestParseNumber(value);
        }

        public virtual ParseString CreateString(string value)
        {
            return new TestParseString(value);
        }

        public virtual ParseBoolean CreateBoolean(bool value)
        {
            throw new NotImplementedException();
        }

        public virtual ParseNull CreateNull()
        {
            throw new NotImplementedException();
        }

        public virtual ParseObject CreateReference(ParseObject parseObject)
        {
            throw new NotImplementedException();
        }
    }

    internal class SameReferenceTwice
    {
        public object One { get; set; }
        public object Two { get; set; }

        public SameReferenceTwice() { }

        public SameReferenceTwice(object obj)
        {
            One = Two = obj;
        }
    }

    internal class TwoReferencesTwice
    {
        public object One { get; set; }
        public object Two { get; set; }
        public object Three { get; set; }
        public object Four { get; set; }

        public TwoReferencesTwice() { }

        public TwoReferencesTwice(object odd, object even)
        {
            One = Three = odd;
            Two = Four = even;
        }
    }

    internal class WatchForReferenceBuilder : TestValueFactory
    {
        public ParseObject ReferencedObject { get; private set; }

        public override ParseObject CreateReference(ParseObject parseObject)
        {
            ReferencedObject = parseObject;
            return parseObject;
        }
    }
}