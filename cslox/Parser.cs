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

        public List<Stmt> Parse()
        {
            var statements = new List<Stmt>();
            while (!_isAtEnd)
            {
                statements.Add(Declaration());
            }

            return statements;
        }
        private Stmt Declaration()
        {
            try
            {
                if (Match(TokenType.VAR))
                {
                    return VariableDeclaration();
                }
                else
                {
                    return Statement();
                }
            }
            catch (ParsingException)
            {
                Synchronize();
                return null;
            }
        }

        private Stmt VariableDeclaration()
        {
            var name = Consume(TokenType.IDENTIFIER, "Expect variable name");

            Expr? initializer = Match(TokenType.EQUAL) ? Expression() : null;

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration");

            return new Stmt.Var { Name = name, Initializer = initializer };
        }

        private Stmt Statement()
        {
            if (Match(TokenType.PRINT))
            {
                return PrintStatement();
            }
            else if (Match(TokenType.LEFT_BRACE))
            {
                return BlockStatement();
            }
            else if (Match(TokenType.IF))
            {
                return IfStatement();
            }
            else if (Match(TokenType.WHILE))
            {
                return WhileStatement();
            }
            else
            {
                return ExpressionStatement();
            }
        }

        private Stmt WhileStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'");
            var condition = Expression();

            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition");
            var body = Statement();

            return new Stmt.While { Condition = condition, Body = body };
        }

        private Stmt BlockStatement()
        {
            var statements = new List<Stmt>();

            while (!(Check(TokenType.RIGHT_BRACE) || _isAtEnd))
            {
                statements.Add(Declaration());
            }
            Consume(TokenType.RIGHT_BRACE, "Expect '}' after block");

            return new Stmt.Block { Statements = statements };
        }
        private Stmt IfStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '('");
            var condition = Expression();

            Consume(TokenType.RIGHT_PAREN, "Expect ')'");

            var thenBranch = Statement();
            Stmt? elseBranch = null;

            if (Match(TokenType.ELSE))
            {
                elseBranch = Statement();
            }

            return new Stmt.If { Condition = condition, ThenBranch = thenBranch, ElseBranch = elseBranch };
        }

        private Stmt ExpressionStatement()
        {
            var expression = Expression();
            Consume(TokenType.SEMICOLON, "; expected");
            return new Stmt.Expression { Expr = expression };
        }

        private Stmt PrintStatement()
        {
            var expression = Expression();
            Consume(TokenType.SEMICOLON, "; expected");
            return new Stmt.Print { Expr = expression };
        }

        private Expr Expression()
        {
            return Assignment();
        }

        private Expr Assignment()
        {
            var expr = Comma();

            if (Match(TokenType.EQUAL))
            {
                var token = Previous();
                var value = Assignment();

                if (expr is Expr.Variable variable)
                {
                    var name = variable.Name;
                    return new Expr.Assign { Name = name, Value = value };
                }
                Error(token, "Invalid assignment target");
            }

            return expr;
        }

        private Expr Comma()
        {
            var expr = Ternary();
            while (Match(TokenType.COMMA))
            {
                var token = Previous();
                var right = Ternary();
                expr = new Expr.Binary { Left = expr, Operator = token, Right = right };
            }
            return expr;
        }

        private Expr Ternary()
        {
            var expr = LogicalOr();
            if (Match(TokenType.QUESTION))
            {
                var token = Previous();
                var left = Ternary();

                Consume(TokenType.COLON, "Expected ':'");

                var right = Ternary();
                expr = new Expr.Ternary { Condition = expr, Operator = token, Left = left, Right = right };
            }
            return expr;
        }

        private Expr LogicalOr()
        {
            var expr = LogicalAnd();
            while (Match(TokenType.OR))
            {
                var token = Previous();
                var right = LogicalAnd();
                expr = new Expr.Logical { Left = expr, Operator = token, Right = right };
            }
            return expr;
        }

        private Expr LogicalAnd()
        {
            var expr = Equality();
            while (Match(TokenType.AND))
            {
                var token = Previous();
                var right = Equality();
                expr = new Expr.Logical { Left = expr, Operator = token, Right = right };
            }
            return expr;
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
            if (Match(TokenType.FALSE, TokenType.TRUE, TokenType.NIL, TokenType.NUMBER, TokenType.STRING, TokenType.IDENTIFIER))
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
                    case TokenType.IDENTIFIER:
                        return new Expr.Variable { Name = token };
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

        private void Synchronize()
        {
            Advance();
            while (!_isAtEnd)
            {
                if (Previous().TokenType == TokenType.SEMICOLON)
                {
                    return;
                }

                switch (Peek().TokenType)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                Advance();
            }
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
        }
    }
}