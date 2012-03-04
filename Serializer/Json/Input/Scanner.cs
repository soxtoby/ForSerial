using System.Collections.Generic;
using System.IO;

namespace json.Json
{
    public class Scanner
    {
        private readonly TokenReader reader;

        private Scanner(TextReader reader)
        {
            this.reader = new TokenReader(reader);
        }

        public static IEnumerable<Token> Scan(string text)
        {
            try
            {
                return TryScan(text);
            }
            catch (ParseException e)
            {
                throw new ParseException(e, text);
            }
        }

        private static IEnumerable<Token> TryScan(string text)
        {
            TextReader reader = new StringReader(text);
            Scanner scanner = new Scanner(reader);
            return scanner.Scan();
        }

        private IEnumerable<Token> Scan()
        {
            while (!reader.EndOfFile)
            {
                if (char.IsWhiteSpace(reader.NextChar))
                {
                    reader.MoveNext();
                }
                else if (char.IsLetter(reader.NextChar) || reader.NextChar == '_')
                {
                    yield return ScanWord();
                }
                else if (char.IsNumber(reader.NextChar))
                {
                    yield return ScanNumber();
                }
                else if (reader.NextChar == '"')
                {
                    yield return ScanString();
                }
                else
                {
                    reader.KeepNextChar();
                    switch (reader.LastChar)
                    {
                        case '{':
                        case '}':
                        case ',':
                        case ':':
                        case '[':
                        case ']':
                            yield return reader.ExtractToken(TokenType.Symbol);
                            break;

                        default:
                            throw new UnexpectedCharacter(reader.LastChar, reader.CurrentLine, reader.CurrentPosition - 1);
                    }
                }
            }

            yield return new Token(TokenType.EOF, reader.CurrentLine, reader.CurrentPosition);
        }

        private Token ScanWord()
        {
            do
            {
                AssertNotEndOfFile();
                reader.KeepNextChar();

            } while (char.IsLetterOrDigit(reader.NextChar) || reader.NextChar == '_');

            return reader.ExtractToken(TokenType.Word);
        }

        private Token ScanNumber()
        {
            bool decimalSeparator = false;
            do
            {
                reader.KeepNextChar();
            } while (!reader.EndOfFile && (char.IsNumber(reader.NextChar) || !decimalSeparator && (decimalSeparator = reader.NextChar == '.')));

            Token stringToken = reader.ExtractToken(TokenType.Numeric);
            return new Token(double.Parse(stringToken.StringValue), stringToken.Line, stringToken.Position);
        }

        private Token ScanString()
        {
            // Don't keep opening char
            reader.MoveNext();

            while (reader.NextChar != '"')
            {
                AssertNotEndOfFile();

                if (reader.NextChar == '\\')
                {
                    // Skip '\' and keep next char
                    reader.MoveNext();
                }

                reader.KeepNextChar();
            }

            // Don't keep closing char
            reader.MoveNext();

            return reader.ExtractToken(TokenType.String);
        }

        private void AssertNotEndOfFile()
        {
            if (reader.EndOfFile)
                throw new UnexpectedEndOfFile(reader.CurrentLine, reader.CurrentPosition);
        }

        public class UnexpectedEndOfFile : ParseException
        {
            public UnexpectedEndOfFile(int line, int position) : base("Unexpected end of file.", string.Empty, line, position) { }
        }

        public class UnexpectedCharacter : ParseException
        {
            public UnexpectedCharacter(char character, int line, int position) : base("Unexpected character.", character.ToString(), line, position) { }
        }
    }
}
