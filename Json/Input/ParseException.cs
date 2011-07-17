using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace json.Json
{
    /// <summary>
    /// CVSBAD Document me :)
    /// </summary>
    [Serializable]
    public class ParseException : Exception
    {

        #region Constructors and Serialization

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

        public ParseException(string message, Token token) : this(message, token.ToString(), token.Line, token.Position) { }

        private ParseException(string message, string tokenString, int line, int position, Exception innerException)
            : base(message, innerException)
        {
            TokenString = tokenString;
            Line = line;
            Position = position;
        }

        protected ParseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // extract all our custom properties here
            TokenString = info.GetString("TokenString");
            Line = info.GetInt16("Line");
            Position = info.GetInt16("Position");
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            // add all our custom properties here
            info.AddValue("TokenString", TokenString);
            info.AddValue("Line", Line);
            info.AddValue("Position", Position);
        }

        #endregion

        #region Public Properties

        public string TokenString { get; set; }
        public int Line { get; set; }
        public int Position { get; set; }

        #endregion

        private string PrettyPrint(string json)
        {
            return "{0} \"{1}\" at {2}:{3}.\r\n{4}\r\n{5}^".FormatWith(Message, TokenString, Line, Position, json.Split('\n')[Line], new string(' ', Position));
        }
    }
}