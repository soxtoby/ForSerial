using System;
using System.Collections.Generic;
namespace json
{
    public class DictionaryParser
    {
        private IEnumerable<Token> tokens;

        private DictionaryParser(IEnumerable<Token> tokens)
        {
            this.tokens = tokens;
        }

        public static Dictionary<string, object> Parse(IEnumerable<Token> tokens)
        {
            DictionaryParser parser = new DictionaryParser(tokens);
            return parser.Parse();
        }

        private Dictionary<string, object> Parse()
        {
            throw new NotImplementedException();
        }
    }
}

