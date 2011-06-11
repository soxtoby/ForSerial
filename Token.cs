using System;

namespace json
{
    public enum TokenType
    {
        Word,
        Numeric,
        String,
        Symbol,
        EOF,
    }

    public struct Token
    {
        public TokenType TokenType { get; private set; }
        public string StringValue { get; private set; }
        public double NumericValue { get; private set; }
        public int Line { get; private set; }
        public int Position { get; private set; }

        public Token(TokenType type, int line, int position)
            : this()
        {
            TokenType = type;
            Line = line;
            Position = position;
        }

        public Token(double value, int line, int position)
            : this(TokenType.Numeric, line, position)
        {
            NumericValue = value;
        }

        public Token(string value, TokenType type, int line, int position)
            : this(type, line, position)
        {
            StringValue = value;
        }

        public override string ToString()
        {
            switch (TokenType)
            {
                case TokenType.Word:
                case TokenType.Symbol:
                    return StringValue;
                case TokenType.Numeric:
                    return NumericValue.ToString();
                case TokenType.String:
                    return "\"{0}\"".FormatWith(StringValue);
                case TokenType.EOF:
                    return "End Of File";
                default:
                    throw new NotImplementedException("No ToString implemented for TokenType " + TokenType.ToString());
            }
        }
    }
}
