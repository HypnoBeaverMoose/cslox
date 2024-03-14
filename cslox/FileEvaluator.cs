namespace Lox
{
    public class FileEvaluator : EvaluatorBase
    {
        public FileEvaluator(Action<List<LoxError>> logErrors) : base(logErrors)
        {
        }

        protected override void Run(string text, List<LoxError> errors)
        {
            var tokens = Scanner.Scan(text, errors);
            if (errors.Count == 0)
            {
                var statements = Parser.Parse(tokens, errors);
                if (errors.Count == 0)
                {
                    var locals = _resolver.ResolveStatements(statements, errors);
                    if (errors.Count == 0)
                    {
                        _interpreter.Interpret(statements, locals, errors);
                    }
                }
            }
        }
    }
}