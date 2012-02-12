using System.Collections.Generic;
using System.Linq;
using json.Json;
using NUnit.Framework;

namespace json.Tests.Json
{
    [TestFixture]
    public class ScannerTests
    {
        [Test]
        public void Empty()
        {
            AssertTokens(string.Empty);
        }

        [Test]
        public void OpenBrace()
        {
            AssertTokens("{", Symbol("{"));
        }

        [Test]
        public void CloseBrace()
        {
            AssertTokens("}", Symbol("}"));
        }

        [Test]
        public void Colon()
        {
            AssertTokens(":", Symbol(":"));
        }

        [Test]
        public void Comma()
        {
            AssertTokens(",", Symbol(","));
        }

        [Test]
        public void OpenBracket()
        {
            AssertTokens("[", Symbol("["));
        }

        [Test]
        public void CloseBracket()
        {
            AssertTokens("]", Symbol("]"));
        }

        [Test]
        public void SingleDigitNumber()
        {
            AssertTokens("5", Number(5));
        }

        [Test]
        public void MultiDigitNumber()
        {
            AssertTokens("12", Number(12));
        }

        [Test]
        public void DecimalNumber()
        {
            AssertTokens("1.2", Number(1.2));
        }

        [Test]
        public void True()
        {
            AssertTokens("true", Word("true"));
        }

        [Test]
        public void False()
        {
            AssertTokens("false", Word("false"));
        }

        [Test]
        public void Null()
        {
            AssertTokens("null", Word("null"));
        }

        [Test]
        public void EmptyString()
        {
            AssertTokens("\"\"", String(string.Empty));
        }

        [Test]
        public void AsciiString()
        {
            AssertTokens("\"foo\"", String("foo"));
        }

        [Test]
        public void EscapedQuotationMark()
        {
            AssertTokens("\"\\\"\"", String("\""));
        }

        [Test]
        public void EscapedBackslash()
        {
            AssertTokens("\"\\\\\"", String("\\"));
        }

        [Test]
        [ExpectedException(typeof(Scanner.UnexpectedEndOfFile))]
        public void UnterminatedString()
        {
            Scanner.Scan("\"").ToList();
        }

        [Test]
        [ExpectedException(typeof(Scanner.UnexpectedCharacter))]
        public void UnknownSymbol()
        {
            Scanner.Scan("-").ToList();
        }



        private static void AssertTokens(string text, params Token[] tokens)
        {
            List<Token> actual = Scanner.Scan(text).ToList();
            List<Token> expected = tokens.Append(Eof).ToList();

            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].TokenType, actual[i].TokenType);
                Assert.AreEqual(expected[i].StringValue, actual[i].StringValue);
                Assert.AreEqual(expected[i].NumericValue, actual[i].NumericValue, double.Epsilon);
            }
        }

        private static Token Symbol(string value)
        {
            return new Token(value, TokenType.Symbol, 0, 0);
        }

        private static Token Number(double value)
        {
            return new Token(value, 0, 0);
        }

        private static Token String(string value)
        {
            return new Token(value, TokenType.String, 0, 0);
        }

        private static Token Word(string value)
        {
            return new Token(value, TokenType.Word, 0, 0);
        }

        private static Token Eof
        {
            get { return new Token(TokenType.EOF, 0, 0); }
        }
    }
}
