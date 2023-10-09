using System.Runtime.InteropServices;
using System.Text;

namespace Lox
{
    public class Interpreter : Expr.Visitor<object?>, Stmt.Visitor<object?>
    {
        public void Interpret(IReadOnlyList<Stmt> statements)
        {
            try
            {
                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (RuntimeException re)
            {
                Lox.RuntimeError(re);
            }
        }

        private void Execute(Stmt statement)
        {
            statement.Accept(this);
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

        public object? VisitLiteral(Expr.Literal expr)
        {
            return expr.Value;
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

        private object? Evaluate(Expr expr)
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

        public class RuntimeException : Exception
        {
            public readonly Token Token;

            public RuntimeException(Token token)
            {
                Token = token;
            }

            public RuntimeException(Token token, string? message) : base(message)
            {
                Token = token;
            }

            public RuntimeException(Token token, string? message, Exception? innerException) : base(message, innerException)
            {
                Token = token;
            }
        }
    }
}