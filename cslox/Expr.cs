namespace Lox
{
    public abstract class Expr
    {

        public abstract T Accept<T>(Visitor<T> visitor);

        public interface Visitor<T>
        {
            T VisitTernary(Ternary expr);

            T VisitBinary(Binary expr);

            T VisitGrouping(Grouping expr);

            T VisitLiteral(Literal expr);

            T VisitUnary(Unary expr);

        }

        public class Ternary : Expr
        {
            public Expr Condition;

            public Token Operator;

            public Expr Left;

            public Expr Right;

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitTernary(this);
            }
        }

        public class Binary : Expr
        {
            public Expr Left;

            public Token Operator;

            public Expr Right;

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitBinary(this);
            }
        }

        public class Grouping : Expr
        {
            public Expr Expression;

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitGrouping(this);
            }
        }

        public class Literal : Expr
        {
            public object? Value;

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitLiteral(this);
            }
        }

        public class Unary : Expr
        {
            public Token Operator;

            public Expr Right;

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitUnary(this);
            }
        }

    }
}
