namespace ForSerial.JsonObjects
{
    public interface JsonObjectVisitor
    {
        void Visit(JsonMap map);
        void Visit(JsonArray array);
        void Visit(JsonValue value);
    }
}