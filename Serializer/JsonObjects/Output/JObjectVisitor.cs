namespace json.JsonObjects
{
    public interface JObjectVisitor
    {
        void Visit(JsonMap map);
        void Visit(JsonArray array);
        void Visit(JsonValue value);
    }
}