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

    internal class CustomCreateObject : TestParseObject
    {
        public override ParseObject CreateObject(string name, ParseValueFactory valueFactory)
        {
            ((CustomCreateValueFactory)valueFactory).ObjectsCreatedFromProperties++;
            return base.CreateObject(name, valueFactory);
        }

        public override ParseArray CreateArray(string name, ParseValueFactory valueFactory)
        {
            ((CustomCreateValueFactory)valueFactory).ArraysCreatedFromProperties++;
            return base.CreateArray(name, valueFactory);
        }
    }

    internal class CustomCreateArray : TestParseArray
    {
        public override ParseObject CreateObject(ParseValueFactory valueFactory)
        {
            ((CustomCreateValueFactory)valueFactory).ObjectsCreatedFromArrays++;
            return base.CreateObject(valueFactory);
        }

        public override ParseArray CreateArray(ParseValueFactory valueFactory)
        {
            ((CustomCreateValueFactory)valueFactory).ArraysCreatedFromArrays++;
            return base.CreateArray(valueFactory);
        }
    }

    internal class CustomCreateValueFactory : TestValueFactory
    {
        public int ObjectsCreatedFromProperties { get; set; }
        public int ArraysCreatedFromProperties { get; set; }
        public int ObjectsCreatedFromArrays { get; set; }
        public int ArraysCreatedFromArrays { get; set; }

        public override ParseObject CreateObject()
        {
            return new CustomCreateObject();
        }

        public override ParseArray CreateArray()
        {
            return new CustomCreateArray();
        }
    }
}