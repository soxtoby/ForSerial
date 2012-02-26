using System;

namespace json.Objects.TypeDefinitions
{
    public class DateTimeDefinition : TypeDefinition
    {
        private static readonly DateTime BaseDate = new DateTime(1970, 1, 1);

        private DateTimeDefinition(Type type) : base(type) { }

        internal static DateTimeDefinition CreateDateTimeDefinition(Type type)
        {
            return type == typeof(DateTime)
                ? new DateTimeDefinition(type)
                : null;
        }

        public override void ReadObject(object input, ObjectReader reader, Writer writer)
        {
            DateTime? dateTime = input as DateTime?;
            writer.Write(dateTime == null
                ? (object)null
                : (dateTime.Value.ToUniversalTime() - BaseDate).TotalMilliseconds);
        }

        public override ObjectValue CreateValue(object value)
        {
            DateTime dateTime;
            double? number = value as double?;
            if (number.HasValue)
                dateTime = BaseDate.AddMilliseconds(number.Value).ToLocalTime();
            else
                dateTime = (DateTime)value;

            return new DefaultObjectValue(dateTime);
        }
    }
}