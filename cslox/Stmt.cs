namespace Lox
{
    public abstract class Stmt
    {

        public abstract T Accept<T>(Visitor<T> visitor);

        public interface Visitor<T>
        {
            T VisitExpression(Expression stmt);

            T VisitPrint(Print stmt);
        }

        public class Expression : Stmt
        {
            public Expr Expr;

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitExpression(this);
            }
        }

        public class Print : Stmt
        {
            public Expr Expr;

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitPrint(this);
            }
        }

    }
}
