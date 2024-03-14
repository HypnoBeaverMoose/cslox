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
                var expression = Parser.ParseExpression(tokens, errors);
                if (errors.Count == 0)
                {
                    Console.WriteLine(_interpreter.Evaluate(expression));
                }
                else
                {
                    errors.Clear();
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
}