using System.IO;
using System.Text;

namespace json.Json
{
    public class TokenReader
    {
        private readonly string json;
        private int position = -1;

        public TokenReader(TextReader reader)
        {
            json = reader.ReadToEnd();
            GetNextChar();
        }

        private int currentLine;
        internal int CurrentLine { get { return currentLine; } }

        private int currentPosition;
        internal int CurrentPosition { get { return currentPosition; } }

        /// <summary>
        /// The next character to be read.
        /// </summary>
        internal char NextChar;

        internal char LastChar;

        /// <summary>
        /// Reads the next character and returns it.
        /// </summary>
        internal void MoveNext()
        {
            LastChar = NextChar;
            GetNextChar();

            if (LastChar == '\n')
            {
                currentLine++;
                currentPosition = 0;
            }
            else
            {
                currentPosition++;
            }
        }

        private void GetNextChar()
        {
            position++;
            EndOfFile = position >= json.Length;
            NextChar = EndOfFile
                ? char.MinValue
                : json[position];
        }

        readonly StringBuilder tokenValue = new StringBuilder();
        int tokenLine;
        int tokenPosition;

        /// <summary>
        /// Reads the next character and stores it in the current token.
        /// </summary>
        internal void KeepNextChar()
        {
            if (tokenValue.Length == 0)
            {
                tokenLine = CurrentLine;
                tokenPosition = CurrentPosition;
            }

            MoveNext();
            tokenValue.Append(LastChar);
        }

        /// <summary>
        /// Returns a token of the specified type, using the currently stored token text, line & position,
        /// and clears the buffer, ready for the next token.
        /// </summary>
        internal Token ExtractToken(TokenType type)
        {
            Token token = new Token(tokenValue.ToString(), type, tokenLine, tokenPosition);
            tokenValue.Length = 0;
            return token;
        }

        /// <summary>
        /// True if the next character to be read is past the end of the file.
        /// </summary>
        internal bool EndOfFile;
    }
}