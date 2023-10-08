using System.Runtime.InteropServices;
using System.Text;

namespace Lox
{
    public class Interpreter : Expr.Visitor<object>
    {
        public object? Interpret(Expr expression)
        {
            try
            {
                return Evaluate(expression);
            }
            catch (RuntimeException re)
            {
                Lox.RuntimeError(re);
            }
            return null;

        }

        public object VisitLiteral(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object VisitGrouping(Expr.Grouping expr)
        {
            return Evaluate(expr);
        }

        public object VisitUnary(Expr.Unary expr)
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

        public object VisitBinary(Expr.Binary expr)
        {
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            switch (expr.Operator.TokenType)
            {
                case TokenType.GREATER:
                    return TryCast<double>(expr.Operator, left) > TryCast<double>(expr.Operator, right);
                case TokenType.LESS:
                    return TryCast<double>(expr.Operator, left) < TryCast<double>(expr.Operator, right);
                case TokenType.GREATER_EQUAL:
                    return TryCast<double>(expr.Operator, left) >= TryCast<double>(expr.Operator, right);
                case TokenType.LESS_EQUAL:
                    return TryCast<double>(expr.Operator, left) <= TryCast<double>(expr.Operator, right);
                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);
                case TokenType.MINUS:
                    return TryCast<double>(expr.Operator, left) - TryCast<double>(expr.Operator, right);
                case TokenType.PLUS:
                    if (left is double leftNum && right is double rightNum)
                    {
                        return leftNum + rightNum;
                    }
                    else if (left is string && right is string)
                    {
                        return string.Concat(left, right);
                    }
                    throw new RuntimeException(expr.Operator, "Operands must be numbers or strings");
                case TokenType.SLASH:
                    return (double)left / (double)right;
                case TokenType.STAR:
                    return (double)left * (double)right;
                default:
                    return null;
            }
        }

        public object VisitTernary(Expr.Ternary expr)
        {
            var condition = IsTruthy(Evaluate(expr.Condition));
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            return condition ? left : right;
        }

        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private bool IsTruthy(object obj)
        {
            return obj is bool truthVal ? truthVal : false;
        }

        private bool IsEqual(object left, object right)
        {
            return left == null ? right == null : left.Equals(right);
        }

        private T TryCast<T>(Token op, object operand)
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