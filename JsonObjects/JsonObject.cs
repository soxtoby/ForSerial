using System.Collections.Generic;

namespace json.JsonObjects
{
    public class JsonObject : Dictionary<string, object>
    {
        public string TypeIdentifier { get; set; }
    }
}