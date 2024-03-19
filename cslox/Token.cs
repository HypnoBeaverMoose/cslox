namespace Lox
{
    public struct Token : IEquatable<Token>
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

        public override bool Equals(object obj)
        {
            if (obj is Token token)
            {
                return this.Equals(token);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Lexeme.GetHashCode() *
                    this.TokenType.GetHashCode();
        }

        public bool Equals(Token other)
        {
            return this.Lexeme == other.Lexeme &&
                    this.TokenType == other.TokenType;
        }
    }
}