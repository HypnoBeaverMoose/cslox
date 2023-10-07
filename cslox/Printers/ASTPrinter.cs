using System.Text;

namespace Lox
{
    public class ASTPrinter : Expr.Visitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string VisitBinary(Expr.Binary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGrouping(Expr.Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteral(Expr.Literal expr)
        {
            return (expr?.Value.ToString()) ?? "nil";
        }

        public string VisitUnary(Expr.Unary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Right);
        }

        public string VisitTernary(Expr.Ternary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Condition, expr.Left, expr.Right);
        }

        private string Parenthesize(string name, params Expr[] exprs)
        {
            StringBuilder builder = new();
            builder.Append('(').Append(name);
            foreach (var expr in exprs)
            {
                builder.Append(' ').Append(expr.Accept(this));
            }
            builder.Append(')');

            return builder.ToString();
        }
    }
}