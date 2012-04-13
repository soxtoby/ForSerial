namespace ForSerial.JsonObjects
{
    public class JsonValue : JsonObject
    {
        public object Value { get; private set; }

        public JsonValue(object value)
        {
            Value = value;
        }

        void JsonObject.Accept(JsonObjectVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}