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

        public static void FixExpression(List<Token> tokens)
        {
            int line = tokens.Count >= 2 ? tokens[tokens.Count - 2].Line : 0;
            tokens.Insert(tokens.Count - 1, new Token(TokenType.SEMICOLON, ";", null, line));
        }

        public static bool TryGetSingleExpression(IReadOnlyList<Stmt> statements, out Expr? expression)
        {
            if (statements.Count == 1 && statements[0] is Stmt.Expression expr)
            {
                expression = expr.Expr;
                return true;
            }
            expression = null;
            return false;
        }
    }
}