using System;
using System.Collections.Generic;
using System.Linq;

namespace ForSerial.Objects
{
    internal class WriteException : Exception
    {
        public WriteException(string typeName, string propertyName, Exception innerException)
            : base(BuildMessage(typeName, propertyName, innerException), GetInnerException(innerException))
        {
            stack = GetStack(typeName, propertyName, innerException);
        }

        private readonly IEnumerable<string> stack;

        private static string BuildMessage(string typeName, string propertyName, Exception innerException)
        {
            string stackText = GetStack(typeName, propertyName, innerException)
                .Join(Environment.NewLine);
            return "{1}{0}At: {2}".FormatWith(Environment.NewLine, GetInnerException(innerException).Message, stackText);
        }

        private static IEnumerable<string> GetStack(string typeName, string propertyName, Exception innerException)
        {
            string currentMember = typeName;
            if (propertyName.IsNotNullOrEmpty())
                currentMember += "." + propertyName;

            WriteException writeException = innerException as WriteException;
            return writeException == null
                ? (IEnumerable<string>)new[] { currentMember }
                : writeException.stack.Append(currentMember).ToList();
        }

        private static Exception GetInnerException(Exception exception)
        {
            WriteException writeException = exception as WriteException;
            return writeException == null
                ? exception
                : writeException.InnerException;
        }
    }
}