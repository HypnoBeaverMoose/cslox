namespace Lox
{
    public class Token
    {
        public readonly TokenType TokenType;

        public readonly string Lexeme;

        public object Literal;

        public readonly int Line;

        public Token(TokenType type, string lexeme, object literal, int line)
        {
            TokenType = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
        }

        public override string ToString()
        {
            return $"{TokenType} {Lexeme}, {Literal}";
        }
    }
}