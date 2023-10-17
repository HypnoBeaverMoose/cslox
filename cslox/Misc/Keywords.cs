namespace Lox
{
    public static class Keywords
    {
        private static readonly Dictionary<string, TokenType> _keywords = new()
        {
            {"and", TokenType.AND},
            {"class", TokenType.CLASS},
            {"else", TokenType.ELSE},
            {"false", TokenType.FALSE},
            {"for", TokenType.FOR},
            {"fun", TokenType.FUN},
            {"if", TokenType.IF},
            {"nil", TokenType.NIL},
            {"or", TokenType.OR},
            {"print", TokenType.PRINT},
            {"return", TokenType.RETURN},
            {"super", TokenType.SUPER},
            {"this", TokenType.THIS},
            {"true", TokenType.TRUE},
            {"var", TokenType.VAR},
            {"while", TokenType.WHILE},
            {"break", TokenType.BREAK},

        };

        public static bool TryGetTokenType(string keyword, out TokenType tokenType)
        {
            return _keywords.TryGetValue(keyword, out tokenType);
        }
    }
}