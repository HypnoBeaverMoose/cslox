namespace Lox
{
    public class Resolver : Expr.Visitor<object?>, Stmt.Visitor<object?>
    {
        private readonly Interpreter _interpreter;

        private FunctionType _currentFunction = FunctionType.NONE;

        private int _nestedLoops = 0;

        private readonly List<Dictionary<Token, VariableState>> _scopes = new();

        public Resolver(Interpreter interpreter)
        {
            _interpreter = interpreter;
        }

        public void Resolve(IReadOnlyList<Stmt> stmts)
        {
            foreach (var stmt in stmts)
            {
                Resolve(stmt);
            }
        }

        public object? VisitBlock(Stmt.Block stmt)
        {
            using (new ScopeBlock(this))
            {
                Resolve(stmt.Statements);
            }

            return null;
        }

        public object? VisitVar(Stmt.Var stmt)
        {
            Declare(stmt.Name);
            if (stmt.Initializer != null)
            {
                Resolve(stmt.Initializer);
            }
            Define(stmt.Name);

            return null;
        }

        public object? VisitFunction(Stmt.Function stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);

            ResolveFunction(stmt, FunctionType.FUNCTION);
            return null;
        }

        public object? VisitExpression(Stmt.Expression stmt)
        {
            Resolve(stmt.Expr);
            return null;
        }

        public object? VisitIf(Stmt.If stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.ThenBranch);
            if (stmt.ElseBranch != null)
            {
                Resolve(stmt.ElseBranch);
            }

            return null;
        }

        public object? VisitPrint(Stmt.Print stmt)
        {
            Resolve(stmt.Expr);
            return null;
        }

        public object? VisitReturn(Stmt.Return stmt)
        {
            if (_currentFunction == FunctionType.NONE)
            {
                Lox.Error(stmt.Keyword, "Can't return from top level code");
            }

            if (stmt.Value != null)
            {
                Resolve(stmt.Value);
            }

            return null;
        }

        public object? VisitWhile(Stmt.While stmt)
        {
            Resolve(stmt.Condition);
            using (new LoopBlock(this))
            {
                Resolve(stmt.Body);
            }
            return null;
        }

        public object? VisitBinary(Expr.Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        public object? VisitCall(Expr.Call expr)
        {
            Resolve(expr.Callee);
            foreach (var arg in expr.Arguments)
            {
                Resolve(arg);
            }
            return null;
        }

        public object? VisitBreak(Stmt.Break stmt)
        {
            if (_nestedLoops == 0)
            {
                Lox.Error(stmt.Keyword, "Expect 'break;' statements only inside loops");
            }
            return null;
        }

        public object? VisitGrouping(Expr.Grouping expr)
        {
            Resolve(expr.Expression);
            return null;
        }

        public object? VisitLiteral(Expr.Literal expr)
        {
            return null;
        }

        public object? VisitLogical(Expr.Logical expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        public object? VisitTernary(Expr.Ternary expr)
        {
            Resolve(expr.Condition);
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        public object? VisitUnary(Expr.Unary expr)
        {
            Resolve(expr.Right);
            return null;
        }

        public object? VisitVariable(Expr.Variable expr)
        {
            if (_scopes.Count > 0 && _scopes[^1].TryGetValue(expr.Name, out VariableState val) && val == VariableState.DECLARED)
            {
                Lox.Error(expr.Name, "Can't read local variable in it's own initializer");
            }

            ResolveLocal(expr, expr.Name);

            return null;
        }

        public object? VisitAssign(Expr.Assign expr)
        {
            Resolve(expr.Value);
            ResolveLocal(expr, expr.Name);
            return null;
        }

        private void ResolveLocal(Expr expr, Token name)
        {
            for (int i = _scopes.Count - 1; i >= 0; i--)
            {
                if (_scopes[i].ContainsKey(name))
                {
                    _interpreter.Resolve(expr, _scopes.Count - 1 - i);
                    _scopes[i][name] = VariableState.USED;
                }
            }
        }

        private void ResolveFunction(Stmt.Function stmt, FunctionType type)
        {

            using (new FunctionBlock(this, type))
            {
                using (new ScopeBlock(this))
                {
                    foreach (var param in stmt.Parameters)
                    {
                        Declare(param);
                        Define(param);
                    }
                    Resolve(stmt.Body);
                }
            }
        }

        private void Declare(Token name)
        {
            if (_scopes.Count > 0)
            {
                if (!_scopes[^1].TryAdd(name, VariableState.DECLARED))
                {
                    Lox.Error(name, "Already a variable with this name in this scope.");
                }
            }
        }

        private void Define(Token name)
        {
            if (_scopes.Count > 0)
            {
                _scopes[^1][name] = VariableState.DEFINED;
            }
        }

        private void BeginScope()
        {
            _scopes.Add(new Dictionary<Token, VariableState>());
        }

        private void EndScope()
        {
            foreach (var kvp in _scopes[^1])
            {
                if (kvp.Value != VariableState.USED)
                {
                    Lox.Error(kvp.Key, "Unused variable detected");
                }
            }
            _scopes.RemoveAt(_scopes.Count - 1);
        }

        private void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

        public object? VisitClass(Stmt.Class stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);
            return null;
        }

        public object? VisitGet(Expr.Get expr)
        {
            Resolve(expr.Obj);
            return null;
        }

        public object? VisitSet(Expr.Set expr)
        {
            Resolve(expr.Value);
            Resolve(expr.Obj);
            return null;
        }

        private struct ScopeBlock : IDisposable
        {
            private Resolver _resolver;

            public ScopeBlock(Resolver resolver)
            {
                _resolver = resolver;
                _resolver.BeginScope();
            }

            public void Dispose()
            {
                _resolver.EndScope();
            }
        }

        private struct FunctionBlock : IDisposable
        {
            private Resolver _resolver;
            private FunctionType _functionCache;

            public FunctionBlock(Resolver resolver, FunctionType type)
            {
                _resolver = resolver;
                _functionCache = _resolver._currentFunction;
                _resolver._currentFunction = type;
            }

            public void Dispose()
            {
                _resolver._currentFunction = _functionCache;
            }
        }

        private struct LoopBlock : IDisposable
        {
            private Resolver _resolver;

            public LoopBlock(Resolver resolver)
            {
                _resolver = resolver;
                _resolver._nestedLoops++;
            }

            public void Dispose()
            {
                _resolver._nestedLoops--;
            }
        }

        private enum FunctionType
        {
            NONE,
            FUNCTION
        }

        private enum VariableState
        {
            NONE,
            DECLARED,
            DEFINED,
            USED
        }
    }
}
