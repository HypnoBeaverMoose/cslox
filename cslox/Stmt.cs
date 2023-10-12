namespace Lox
{
    public abstract class Stmt
    {

        public abstract T Accept<T>(Visitor<T> visitor);

        public interface Visitor<T>
        {
            T VisitIf(If stmt);

            T VisitBlock(Block stmt);

            T VisitExpression(Expression stmt);

            T VisitPrint(Print stmt);

            T VisitVar(Var stmt);

        }

        public class If : Stmt
        {
            public Expr Condition;

            public Stmt ThenBranch;

            public Stmt? ElseBranch;

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitIf(this);
            }
        }

        public class Block : Stmt
        {
            public List<Stmt> Statements;

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitBlock(this);
            }
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

        public class Var : Stmt
        {
            public Token Name;

            public Expr Initializer;

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitVar(this);
            }
        }

    }
}
