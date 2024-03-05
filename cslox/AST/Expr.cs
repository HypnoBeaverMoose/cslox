namespace Lox
{
    public abstract class Expr
    {
        public abstract T Accept<T>(Visitor<T> visitor);

        public interface Visitor<T>
        {
            T VisitAssign(Assign expr);

            T VisitTernary(Ternary expr);

            T VisitBinary(Binary expr);

            T VisitCall(Call expr);

            T VisitGet(Get expr);

            T VisitGrouping(Grouping expr);

            T VisitLiteral(Literal expr);

            T VisitLogical(Logical expr);

            T VisitSet(Set expr);

            T VisitThis(This expr);

            T VisitUnary(Unary expr);

            T VisitVariable(Variable expr);


        }


        public class Assign : Expr
        {
            public Token Name;

            public Expr Value;



            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitAssign(this);
            }
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


        public class Call : Expr
        {
            public Expr Callee;

            public Token Paren;

            public List<Expr> Arguments;



            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitCall(this);
            }
        }


        public class Get : Expr
        {
            public Expr Obj;

            public Token Name;



            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitGet(this);
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


        public class Logical : Expr
        {
            public Expr Left;

            public Token Operator;

            public Expr Right;



            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitLogical(this);
            }
        }


        public class Set : Expr
        {
            public Expr Obj;

            public Token Name;

            public Expr Value;



            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitSet(this);
            }
        }


        public class This : Expr
        {
            public Token Keyword;



            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitThis(this);
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


        public class Variable : Expr
        {
            public Token Name;



            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitVariable(this);
            }
        }


    }
}