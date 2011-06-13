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

        public ParseException() : base() { }

        public ParseException(string message, string tokenString, int line, int position)
            : base(message)
        {
            TokenString = tokenString;
            Line = line;
            Position = position;
        }

        public ParseException(string message, Token token) : this(message, token.ToString(), token.Line, token.Position) { }

        public ParseException(string message, string tokenString, int line, int position, Exception innerException)
            : base(message, innerException)
        {
            TokenString = tokenString;
            Line = line;
            Position = position;
        }

        public ParseException(string message, Token token, Exception innerException) : this(message, token.ToString(), token.Line, token.Position, innerException) { }


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

        public string PrettyPrint(string mdx)
        {
            return "{0} \"{1}\" at {2}:{3}.\r\n{4}\r\n{5}^".FormatWith(Message, TokenString, Line, Position, mdx.Split('\n')[Line], new string(' ', Position));
        }
    }
}