using System.Text;

namespace Lox
{
    public class RPNPrinter : Expr.Visitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string VisitBinary(Expr.Binary expr)
        {
            return RPN(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGrouping(Expr.Grouping expr)
        {
            return RPN("g", expr.Expression);
        }

        public string VisitLiteral(Expr.Literal expr)
        {
            return (expr?.Value.ToString()) ?? "nil";
        }

        public string VisitUnary(Expr.Unary expr)
        {
            return RPN(expr.Operator.Lexeme, expr.Right);
        }

        public string VisitTernary(Expr.Ternary expr)
        {
            return RPN(expr.Operator.Lexeme, expr.Condition, expr.Left, expr.Right);
        }

        public string VisitVariable(Expr.Variable expr)
        {
            return expr.Name.Lexeme;
        }

        public string VisitAssign(Expr.Assign expr)
        {
            return RPN("=", new Expr.Variable { Name = expr.Name }, expr.Value);
        }

        private string RPN(string name, params Expr[] exprs)
        {
            StringBuilder builder = new();
            foreach (var expr in exprs)
            {
                builder.Append(expr.Accept(this)).Append(' ');
            }

            builder.Append(name);
            return builder.ToString();
        }
    }
}