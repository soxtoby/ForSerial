using System;
using System.Collections.Generic;

namespace ForSerial.Objects.TypeDefinitions
{
    internal class EnumDefinition : TypeDefinition
    {
        private const char Comma = ',';
        private const char Space = ' ';

        private readonly Dictionary<string, int> nameMap = new Dictionary<string, int>();

        private EnumDefinition(Type type)
            : base(type)
        {
            string[] names = Enum.GetNames(type);
            Array values = Enum.GetValues(type);

            for (int i = 0; i < names.Length; i++)
                nameMap[names[i]] = Convert.ToInt32(values.GetValue(i));
        }

        internal static EnumDefinition CreateEnumDefinition(Type type)
        {
            return type.CanBeCastTo(typeof(Enum))
                ? new EnumDefinition(type)
                : null;
        }

        public override void Read(object input, ObjectReader reader, Writer writer, PartialOptions optionsOverride)
        {
            if ((optionsOverride.EnumSerialization ?? reader.Options.EnumSerialization) == EnumSerialization.AsString)
                writer.Write(input.ToString());
            else
                writer.Write((int)input);
        }

        public override ObjectOutput CreateValue(object value)
        {
            string strValue = value as string;
            if (strValue != null)
            {
                int enumValue = 0;
                int from = 0;
                for (int to = 0; to < strValue.Length - 2; to++)
                {
                    if (strValue[to] == Comma && strValue[to + 1] == Space)
                    {
                        enumValue |= nameMap[strValue.Substring(from, to - from)];
                        from = to + 2;
                        to += 1;
                    }
                }
                enumValue += nameMap[strValue.Substring(from)];
                return new DefaultObjectValue(Enum.ToObject(Type, enumValue));
            }

            return new DefaultObjectValue(Enum.ToObject(Type, ConvertToCorrectType(value)));
        }
    }
}
