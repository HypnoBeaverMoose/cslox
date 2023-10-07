using System.Runtime.Serialization;

namespace Lox
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _current = 0;
        private bool _isAtEnd => Peek().TokenType == TokenType.EOF;

        public Parser(List<Token> tokens)
        {
            _tokens = new List<Token>(tokens);
        }

        public Expr? Parse()
        {
            try
            {
                return Expression();
            }
            catch (ParsingException)
            {
                return null;
            }
        }

        private Expr Expression()
        {
            return Equality();
        }

        private Expr Equality()
        {
            var expr = Comparison();
            while (Match(TokenType.BANG_EQUAL, TokenType.BANG))
            {
                var token = Previous();
                var right = Comparison();
                expr = new Expr.Binary { Left = expr, Operator = token, Right = right };
            }
            return expr;
        }

        private Expr Comparison()
        {
            var expr = Term();
            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                var token = Previous();
                var right = Term();
                expr = new Expr.Binary { Left = expr, Operator = token, Right = right };
            }
            return expr;
        }

        private Expr Term()
        {
            var expr = Factor();
            while (Match(TokenType.MINUS, TokenType.PLUS))
            {
                var token = Previous();
                var right = Factor();
                expr = new Expr.Binary { Left = expr, Operator = token, Right = right };
            }
            return expr;
        }

        private Expr Factor()
        {
            var expr = Unary();
            while (Match(TokenType.SLASH, TokenType.STAR))
            {
                var token = Previous();
                var right = Unary();
                expr = new Expr.Binary { Left = expr, Operator = token, Right = right };
            }
            return expr;
        }

        private Expr Unary()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                var token = Previous();
                var right = Unary();
                return new Expr.Unary { Operator = token, Right = right };
            }

            return Primary();
        }

        private Expr Primary()
        {
            if (Match(TokenType.FALSE, TokenType.TRUE, TokenType.NIL, TokenType.NUMBER, TokenType.STRING))
            {
                var token = Previous();
                switch (token.TokenType)
                {
                    case TokenType.FALSE:
                        return new Expr.Literal { Value = false };
                    case TokenType.TRUE:
                        return new Expr.Literal { Value = true };
                    case TokenType.NIL:
                        return new Expr.Literal { Value = null };
                    case TokenType.NUMBER:
                    case TokenType.STRING:
                        return new Expr.Literal { Value = token.Literal };
                }
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                var expression = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression");
                return new Expr.Grouping { Expression = expression };
            }

            throw Error(Peek(), "Expect expression");
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type))
            {
                return Advance();
            }

            throw Error(Peek(), message);
        }

        private Exception Error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParsingException();
        }

        //TODO: Rename to MatchAny, Return (bool, Token)
        public bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private bool Check(TokenType tokenType)
        {
            if (!_isAtEnd)
            {
                return Peek().TokenType == tokenType;
            }

            return false;
        }

        private Token Advance()
        {
            if (!_isAtEnd)
            {
                _current++;
            }

            return Previous();
        }

        private Token Peek() => _tokens[_current];

        private Token Previous() => _tokens[_current - 1];

        [Serializable]
        public class ParsingException : Exception
        {
            public ParsingException()
            {
            }

            public ParsingException(string? message) : base(message)
            {
            }

            public ParsingException(string? message, Exception? innerException) : base(message, innerException)
            {
            }

            protected ParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }
}