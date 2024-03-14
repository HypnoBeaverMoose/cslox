namespace Lox
{
    public class REPL : EvaluatorBase
    {
        public REPL(Action<List<LoxError>> logErrors) : base(logErrors)
        {
        }

        protected override void Run(string text, List<LoxError> errors)
        {
            var tokens = Scanner.Scan(text, errors);
            if (errors.Count == 0)
            {
                var expression = new List<Stmt>() { Parser.ParseExpression(tokens, errors) };
                if (errors.Count == 0)
                {
                    var locals = _resolver.ResolveStatements(expression, errors);
                    if (errors.Count == 0)
                    {
                        _interpreter.Interpret(expression, locals, errors);
                    }
                }
            }
        }
    }
}