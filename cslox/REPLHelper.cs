namespace Lox
{
    public static class REPLHelper
    {
        private static HashSet<TokenType> _statementTokens = new() 
        {
            TokenType.LEFT_BRACE,
            TokenType.RIGHT_BRACE,
            TokenType.SEMICOLON,
            TokenType.VAR, 
            TokenType.WHILE, 
            TokenType.IF, 
            TokenType.FOR
        };

        public static bool IsExpression(IReadOnlyList<Token> tokens)
        {
            return _statementTokens.Intersect(tokens.Select(t => t.TokenType)).Count() == 0;
        }
    }
}