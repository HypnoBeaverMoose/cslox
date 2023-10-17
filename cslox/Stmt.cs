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

            T VisitFunction(Function stmt);

            T VisitPrint(Print stmt);

            T VisitReturn(Return stmt);

            T VisitVar(Var stmt);

            T VisitWhile(While stmt);

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

        public class Function : Stmt
        {
            public Token Name;

            public List<Token> Parameters;

            public List<Stmt> Body;

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitFunction(this);
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

        public class Return : Stmt
        {
            public Token Keyword;

            public Expr Value;

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitReturn(this);
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

        public class While : Stmt
        {
            public Expr Condition;

            public Stmt Body;

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitWhile(this);
            }
        }

    }
}
