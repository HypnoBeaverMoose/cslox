namespace Lox
{
    public class Interpreter : Expr.Visitor<object?>, Stmt.Visitor<object?>
    {
        public readonly Environment Globals = new();

        private Dictionary<Expr, int> _locals = new();
        private Environment _environment;

        public Interpreter()
        {
            _environment = Globals;
            Globals.Define("clock", new Clock());
        }

        public void Interpret(IReadOnlyList<Stmt> statements, Dictionary<Expr, int> locals, List<LoxError> errors)
        {
            _locals.Clear();
            foreach (var local in locals)
            {
                _locals.Add(local.Key, local.Value);
            }

            try
            {
                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (RuntimeException re)
            {
                errors.Add(new LoxError(re.Token, re.Message, LoxError.ErrorType.Runtime));
            }
        }

        private void Execute(Stmt statement)
        {
            statement.Accept(this);
        }

        public void ExecuteBlock(IReadOnlyList<Stmt> statements, Environment environment)
        {
            var previous = _environment;
            try
            {
                _environment = environment;
                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                _environment = previous;
            }
        }

        public object? VisitLogical(Expr.Logical expr)
        {
            var left = Evaluate(expr.Left);
            if (expr.Operator.TokenType == TokenType.OR && IsTruthy(left))
            {
                return left;
            }
            else if (expr.Operator.TokenType == TokenType.AND && !IsTruthy(left))
            {
                return left;
            }

            return Evaluate(expr.Right);
        }

        public object? VisitIf(Stmt.If stmt)
        {
            if (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.ThenBranch);
            }
            else if (stmt.ElseBranch != null)
            {
                Execute(stmt.ElseBranch);
            }
            return null;
        }

        public object? VisitFunction(Stmt.Function stmt)
        {
            var function = LoxFunctionBase.Create(stmt, _environment);
            _environment.Define(stmt.Name.Lexeme, function);
            return null;
        }

        public object? VisitBlock(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.Statements, new Environment(_environment));
            return null;
        }

        public object? VisitVar(Stmt.Var stmt)
        {
            object? value = null;
            if (stmt.Initializer != null)
            {
                value = Evaluate(stmt.Initializer);
            }
            _environment.Define(stmt.Name.Lexeme, value);

            return null;
        }


        public object? VisitExpression(Stmt.Expression stmt)
        {
            Evaluate(stmt.Expr);
            return null;
        }

        public object? VisitPrint(Stmt.Print stmt)
        {
            var result = Evaluate(stmt.Expr);
            Console.WriteLine(result?.ToString());
            return null;
        }

        public object? VisitWhile(Stmt.While stmt)
        {
            while (IsTruthy(Evaluate(stmt.Condition)))
            {
                try
                {
                    Execute(stmt.Body);
                }
                catch (BreakException)
                {
                    break;
                }
            }
            return null;
        }

        public object? VisitLiteral(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object? VisitVariable(Expr.Variable expr)
        {
            return LookUpVariable(expr.Name, expr);
        }

        public object? VisitGrouping(Expr.Grouping expr)
        {
            return Evaluate(expr);
        }

        public object? VisitUnary(Expr.Unary expr)
        {
            var result = Evaluate(expr);
            switch (expr.Operator.TokenType)
            {
                case TokenType.MINUS:
                    return -TryCast<double>(expr.Operator, result);
                case TokenType.BANG:
                    return !IsTruthy(result);
                default:
                    throw new NotImplementedException();
            }
        }

        public object? VisitBinary(Expr.Binary expr)
        {
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            var leftNum = left as double?;
            var rightNum = right as double?;
            var leftString = left as string;
            var rightString = right as string;


            var bothAreStrings = leftString != null && rightString != null;
            var bothAreNumbers = leftNum != null && rightNum != null;

            switch (expr.Operator.TokenType)
            {
                case TokenType.GREATER:
                    if (bothAreNumbers)
                    {
                        return leftNum > rightNum;
                    }
                    else if (bothAreStrings)
                    {
                        return string.Compare(leftString, rightString) == 1;
                    }
                    throw new RuntimeException(expr.Operator, "Operands must be numbers or strings");
                case TokenType.LESS:
                    if (bothAreNumbers)
                    {
                        return leftNum < rightNum;
                    }
                    else if (bothAreStrings)
                    {
                        return string.Compare(leftString, rightString) == -1;
                    }
                    throw new RuntimeException(expr.Operator, "Operands must be numbers or strings");
                case TokenType.GREATER_EQUAL:
                    if (bothAreNumbers)
                    {
                        return leftNum >= rightNum;
                    }
                    else if (bothAreStrings)
                    {
                        return string.Compare(leftString, rightString) > -1;
                    }
                    throw new RuntimeException(expr.Operator, "Operands must be numbers or strings");
                case TokenType.LESS_EQUAL:
                    if (bothAreNumbers)
                    {
                        return leftNum <= rightNum;
                    }
                    else if (bothAreStrings)
                    {
                        return string.Compare(leftString, rightString) < 1;
                    }
                    throw new RuntimeException(expr.Operator, "Operands must be numbers or strings");
                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);
                case TokenType.MINUS:
                    return TryCast<double>(expr.Operator, left) - TryCast<double>(expr.Operator, right);
                case TokenType.PLUS:
                    if (bothAreNumbers)
                    {
                        return leftNum + rightNum;
                    }
                    else if (bothAreStrings)
                    {
                        return string.Concat(leftString, rightString);
                    }
                    else if (leftString != null || rightString != null)
                    {
                        return string.Concat(left, right);
                    }
                    throw new RuntimeException(expr.Operator, "Operands must be numbers or strings");
                case TokenType.SLASH:
                    var divResult = TryCast<double>(expr.Operator, left) / TryCast<double>(expr.Operator, right);
                    if (double.IsInfinity(divResult))
                    {
                        throw new RuntimeException(expr.Operator, "Division by zero");
                    }
                    return divResult;
                case TokenType.STAR:
                    return TryCast<double>(expr.Operator, left) * TryCast<double>(expr.Operator, right); ;
                case TokenType.COMMA:
                    return right;
                default:
                    return null;
            }
        }

        public object? VisitTernary(Expr.Ternary expr)
        {
            var condition = IsTruthy(Evaluate(expr.Condition));
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            return condition ? left : right;
        }

        public object? VisitCall(Expr.Call expr)
        {
            var callee = Evaluate(expr.Callee);
            if (callee is not ILoxCallable loxCallable)
            {
                throw new RuntimeException(expr.Paren, "Only functions and classes can be called");
            }

            var arguments = expr.Arguments.Select(a => Evaluate(a)).ToList();
            if (arguments.Count != loxCallable.Arity)
            {
                throw new RuntimeException(expr.Paren, $"Expected {loxCallable.Arity} arguments, but got {arguments.Count}.");
            }

            return loxCallable?.Call(this, arguments);
        }

        public object? VisitAssign(Expr.Assign expr)
        {
            var value = Evaluate(expr.Value);

            if (_locals.TryGetValue(expr, out int distance))
            {
                _environment.PutAt(distance, expr.Name, value);
            }
            else
            {
                Globals.Put(expr.Name, value);
            }
            return value;
        }

        public object? Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private bool IsTruthy(object? obj)
        {
            return obj is bool truthVal ? truthVal : false;
        }

        private bool IsEqual(object? left, object? right)
        {
            return left == null ? right == null : left.Equals(right);
        }

        private T TryCast<T>(Token op, object? operand)
        {
            if (operand is T result)
            {
                return result;
            }
            else
            {
                throw new RuntimeException(op, $"Operand must be{typeof(T).Name}");
            }
        }

        public object? VisitReturn(Stmt.Return stmt)
        {
            var value = stmt.Value == null ? stmt.Value : Evaluate(stmt.Value);

            throw new ReturnException(value);
        }

        public object? VisitBreak(Stmt.Break stmt)
        {
            throw new BreakException();
        }

        private object? LookUpVariable(Token name, Expr expr)
        {
            if (_locals.TryGetValue(expr, out int distance))
            {
                return _environment.GetAt(name, distance);
            }
            else
            {
                return Globals.Get(name);
            }
        }

        public object? VisitClass(Stmt.Class stmt)
        {
            _environment.Define(stmt.Name.Lexeme, null);

            var loxClass = new LoxClass(stmt.Name.Lexeme,
                            stmt.Methods.ToDictionary(m => m.Name.Lexeme,
                                                        m => LoxFunctionBase.Create(m, _environment)));

            _environment.Put(stmt.Name, loxClass);
            return null;
        }

        public object? VisitGet(Expr.Get expr)
        {
            var obj = Evaluate(expr.Obj);
            if (obj is LoxInstance loxInstance)
            {
                return loxInstance.Get(expr.Name);
            }

            throw new RuntimeException(expr.Name, "Only instances have properties.");
        }

        public object? VisitSet(Expr.Set expr)
        {
            var target = Evaluate(expr.Obj);

            if (target is LoxInstance loxInstance)
            {
                var value = Evaluate(expr.Value);
                loxInstance.Set(expr.Name, value);
                return value;
            }

            throw new RuntimeException(expr.Name, "Only instances have fields.");
        }

        public object? VisitThis(Expr.This expr)
        {
            return LookUpVariable(expr.Keyword, expr);
        }
    }
}