namespace json
{
    public interface ParseValue
    {
        void AddToObject(ParseObject obj, string name);
        void AddToArray(ParseArray array);
        /// <summary>
        /// Converts the current value into a <see cref="ParseObject"/>, so parser output is consistent.
        /// </summary>
        ParseObject AsObject();
    }

    public interface ParseObject : ParseValue
    {
        /// <summary>
        /// Sets the object's type.
        /// </summary>
        /// <returns>
        /// True if the object was pre-built and the parser should skip populating it.
        /// </returns>
        bool SetType(string typeIdentifier, Parser parser);
        void AddNull(string name);
        void AddBoolean(string name, bool value);
        void AddNumber(string name, double value);
        void AddString(string name, string value);
        void AddObject(string name, ParseObject value);
        void AddArray(string name, ParseArray value);
    }

    public interface Parser
    {
        ParseObject ParseSubObject(ParseValueFactory subParseValueFactory);
    }

    public abstract class ParseObjectBase : ParseObject
    {
        public void AddToObject(ParseObject obj, string name)
        {
            obj.AddObject(name, this);
        }

        public void AddToArray(ParseArray array)
        {
            array.AddObject(this);
        }

        public ParseObject AsObject()
        {
            return this;
        }

        public virtual bool SetType(string typeIdentifier, Parser parser) { return false; }

        public abstract void AddNull(string name);

        public abstract void AddBoolean(string name, bool value);

        public abstract void AddNumber(string name, double value);

        public abstract void AddString(string name, string value);

        public abstract void AddObject(string name, ParseObject value);

        public abstract void AddArray(string name, ParseArray value);
    }

    public interface ParseArray : ParseValue
    {
        void AddNull();
        void AddBoolean(bool value);
        void AddNumber(double value);
        void AddString(string value);
        void AddObject(ParseObject value);
        void AddArray(ParseArray value);
    }

    public abstract class ParseArrayBase : ParseArray
    {
        public void AddToObject(ParseObject obj, string name)
        {
            obj.AddArray(name, this);
        }

        public void AddToArray(ParseArray array)
        {
            array.AddArray(this);
        }

        public abstract void AddNull();

        public abstract void AddBoolean(bool value);

        public abstract void AddNumber(double value);

        public abstract void AddString(string value);

        public abstract void AddObject(ParseObject value);

        public abstract void AddArray(ParseArray value);

        public abstract ParseObject AsObject();
    }

    public abstract class ParseNumber : ParseValue
    {
        protected readonly double value;

        protected ParseNumber(double value)
        {
            this.value = value;
        }

        public void AddToObject(ParseObject obj, string name)
        {
            obj.AddNumber(name, value);
        }

        public void AddToArray(ParseArray array)
        {
            array.AddNumber(value);
        }

        public abstract ParseObject AsObject();
    }

    public abstract class ParseString : ParseValue
    {
        protected readonly string value;

        protected ParseString(string value)
        {
            this.value = value;
        }

        public void AddToObject(ParseObject obj, string name)
        {
            obj.AddString(name, value);
        }

        public void AddToArray(ParseArray array)
        {
            array.AddString(value);
        }

        public abstract ParseObject AsObject();
    }

    public abstract class ParseBoolean : ParseValue
    {
        protected readonly bool value;

        protected ParseBoolean(bool value)
        {
            this.value = value;
        }

        public void AddToObject(ParseObject obj, string name)
        {
            obj.AddBoolean(name, value);
        }

        public void AddToArray(ParseArray array)
        {
            array.AddBoolean(value);
        }

        public abstract ParseObject AsObject();
    }

    public abstract class ParseNull : ParseValue
    {
        public void AddToObject(ParseObject obj, string name)
        {
            obj.AddNull(name);
        }

        public void AddToArray(ParseArray array)
        {
            array.AddNull();
        }

        public abstract ParseObject AsObject();
    }

    public interface ParseValueFactory
    {
        ParseObject CreateObject();
        ParseArray CreateArray();
        ParseNumber CreateNumber(double value);
        ParseString CreateString(string value);
        ParseBoolean CreateBoolean(bool value);
        ParseNull CreateNull();
        ParseObject CreateReference(ParseObject parseObject);
    }
}

