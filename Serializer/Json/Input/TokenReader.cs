using System.Text;

namespace json.Json
{
    public class TokenReader
    {
        private readonly string json;
        private int position = -1;

        public TokenReader(string json)
        {
            this.json = json;
            GetNextChar();
        }

        private int currentLine;
        internal int CurrentLine { get { return currentLine; } }

        private int currentPosition;
        internal int CurrentPosition { get { return currentPosition; } }

        /// <summary>
        /// The next character to be read.
        /// </summary>
        internal char CurrentChar;

        /// <summary>
        /// Reads the next character and returns it.
        /// </summary>
        internal void MoveNext()
        {
            char lastChar = CurrentChar;
            GetNextChar();

            if (lastChar == '\n')
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
            CurrentChar = EndOfFile
                ? char.MinValue
                : json[position];
        }

        private readonly StringBuilder tokenValue = new StringBuilder();

        /// <summary>
        /// Reads the next character and stores it in the current token.
        /// </summary>
        internal void KeepNextChar()
        {
            tokenValue.Append(CurrentChar);
            MoveNext();
        }

        internal string ExtractToken()
        {
            string token = tokenValue.ToString();
            tokenValue.Length = 0;
            return token;
        }

        internal bool EndOfFile;
    }
}