using System;
using System.Text;
using System.Xml;

namespace ForSerial.Objects.TypeDefinitions
{
    internal class DateTimeDefinition : TypeDefinition
    {
        private const char Zero = '0';
        private const char Dash = '-';
        private const char DateTimeSeparator = 'T';
        private const char Colon = ':';
        private const char DecimalPoint = '.';
        private const char TimeZoneDesignator = 'Z';
        private static readonly DateTime BaseDate = new DateTime(1970, 1, 1);

        private DateTimeDefinition() : base(typeof(DateTime)) { }

        public static readonly DateTimeDefinition Instance = new DateTimeDefinition();

        public override void Read(object input, ObjectReader reader, Writer writer, bool requestTypeIdentification)
        {
            DateTime dt = (DateTime)input;
            writer.Write(FormatAsIso8601(dt.ToUniversalTime()));
        }

        /// <summary>
        /// Format from http://www.w3.org/TR/NOTE-datetime
        /// </summary>
        private static string FormatAsIso8601(DateTime dt)
        {
            StringBuilder sb = new StringBuilder(28);

            int year = dt.Year;
            if (year < 1000)
                sb.Append(Zero);
            if (year < 100)
                sb.Append(Zero);
            if (year < 10)
                sb.Append(Zero);
            sb.Append(year);    // YYYY

            sb.Append(Dash);

            int month = dt.Month;
            if (month < 10)
                sb.Append(Zero);
            sb.Append(month);   // MM

            sb.Append(Dash);

            int day = dt.Day;
            if (day < 10)
                sb.Append(Zero);
            sb.Append(day);     // DD

            sb.Append(DateTimeSeparator);

            int hour = dt.Hour;
            if (hour < 10)
                sb.Append(Zero);
            sb.Append(hour);    // hh

            sb.Append(Colon);

            int minute = dt.Minute;
            if (minute < 10)
                sb.Append(Zero);
            sb.Append(minute);  // mm

            sb.Append(Colon);

            int second = dt.Second;
            if (second < 10)
                sb.Append(Zero);
            sb.Append(second);  // ss

            sb.Append(DecimalPoint);

            int millisecond = dt.Millisecond;
            if (millisecond < 100)
                sb.Append(Zero);
            if (millisecond < 10)
                sb.Append(Zero);
            sb.Append(millisecond); // s
            sb.Append(TimeZoneDesignator);

            return sb.ToString();
        }

        public override ObjectOutput CreateValue(object value)
        {
            DateTime dateTime;
            string dateString = value as string;
            if (dateString != null)
            {
                dateTime = XmlConvert.ToDateTime(dateString, XmlDateTimeSerializationMode.RoundtripKind);
            }
            else
            {
                double? number = value as double?;
                if (number.HasValue)
                    dateTime = BaseDate.AddMilliseconds(number.Value).ToLocalTime();
                else
                    dateTime = (DateTime)value;
            }

            return new DefaultObjectValue(dateTime);
        }
    }
}
