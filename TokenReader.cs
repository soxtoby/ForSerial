using System;
using System.IO;
using System.Text;

namespace json
{
    public class TokenReader
    {
        public TokenReader(TextReader reader)
        {
            _reader = reader;
            CurrentLine = 0;
            CurrentPosition = 0;
        }

        TextReader _reader;
        char _nextChar;

        internal int CurrentLine { get; set; }
        internal int CurrentPosition { get; set; }

        /// <summary>
        /// The next character to be read.
        /// </summary>
        internal char NextChar {
            get
            {
                if (_nextChar == char.MinValue)
                {
                    _nextChar = (char)_reader.Peek();
                }
                return _nextChar;
            }
        }

        internal char LastChar { get; set; }

        /// <summary>
        /// Reads the next character and returns it.
        /// </summary>
        internal char DiscardNextChar()
        {
            _nextChar = char.MinValue;
            LastChar = (char)_reader.Read();
            
            if (LastChar == '\n')
            {
                CurrentLine++;
                CurrentPosition = 0;
            }

            else if (LastChar == '\r')
            {
                CurrentPosition++;
            }
            else
            {
                CurrentPosition++;
            }
            
            return LastChar;
        }

        StringBuilder _tokenValue = new StringBuilder();
        int _tokenLine;
        int _tokenPosition;

        /// <summary>
        /// Reads the next character and stores it in the current token.
        /// </summary>
        internal void KeepNextChar()
        {
            if (_tokenValue.Length == 0)
            {
                _tokenLine = CurrentLine;
                _tokenPosition = CurrentPosition;
            }

            _tokenValue.Append(DiscardNextChar());
        }

        /// <summary>
        /// Returns a token of the specified type, using the currently stored token text, line & position,
        /// and clears the buffer, ready for the next token.
        /// </summary>
        internal Token ExtractToken(TokenType type)
        {
            Token token = new Token(_tokenValue.ToString(), type, _tokenLine, _tokenPosition);
            _tokenValue.Length = 0;
            return token;
        }

        /// <summary>
        /// True if the next character to be read is past the end of the file.
        /// </summary>
        internal bool EndOfFile {
            get { return _reader.Peek() == -1; }
        }
    }
}