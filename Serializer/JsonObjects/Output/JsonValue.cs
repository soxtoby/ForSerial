namespace json.JsonObjects
{
    public class JsonValue : JsonObject
    {
        public object Value { get; private set; }

        public JsonValue(object value)
        {
            Value = value;
        }

        public void Accept(JObjectVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}