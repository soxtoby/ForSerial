using System;

namespace ForSerial.Json
{
    [Serializable]
    public class ParseException : Exception
    {
        protected ParseException(string message, string tokenString, int line, int position)
            : base(message)
        {
            TokenString = tokenString;
            Line = line;
            Position = position;
        }

        public ParseException(ParseException innerException, string json)
            : this(GetWrappedExceptionMessage(innerException, json), innerException.TokenString, innerException.Line, innerException.Position, innerException)
        { }

        private static string GetWrappedExceptionMessage(ParseException innerException, string json)
        {
            return "{0}\r\nInner Exception: {1}".FormatWith(innerException.PrettyPrint(json), innerException);
        }

        private ParseException(string message, string tokenString, int line, int position, Exception innerException)
            : base(message, innerException)
        {
            TokenString = tokenString;
            Line = line;
            Position = position;
        }

        public string TokenString { get; private set; }
        public int Line { get; private set; }
        public int Position { get; private set; }

        private string PrettyPrint(string json)
        {
            return "{0} \"{1}\" at {2}:{3}.\r\n{4}\r\n{5}^".FormatWith(Message, TokenString, Line, Position, json.Split('\n')[Line], new string(' ', Position));
        }
    }
}