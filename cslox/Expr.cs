namespace Lox
{
    public abstract class Expr
    {
        public class Binary : Expr
        {
            public Expr Left;
            public Token Operator;
            public Expr Right;
        }

        public class Grouping : Expr
        {
            public Expr Expression;
        }

        public class Literal : Expr
        {
            public Object Value;
        }

        public class Unary : Expr
        {
            public Token Operator;
            public Expr Right;
        }

    }
}
