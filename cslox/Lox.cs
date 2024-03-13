namespace Lox
{
    public static class Lox
    {
        public enum RunResult { Success, ParseError, RuntimeError }

        private static Interpreter _interpreter = new();

        private static Resolver _resolver = new();

        public static void Main(string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    RunPrompt();
                    break;
                case 1:
                    RunFile(args[0]);
                    break;
                default:
                    Console.WriteLine("Usage: cslox [script]");
                    System.Environment.Exit(64);
                    break;
            }
        }

        private static void RunPrompt()
        {
            string? line = null;
            do
            {
                Console.Write(">");
                line = Console.ReadLine();
                if (line != null)
                {
                    Run(line);
                }
            } while (line != null);
        }

        private static void RunFile(string filename)
        {
            var result = Run(System.IO.File.ReadAllText(filename));

            switch (result)
            {
                case RunResult.ParseError:
                    System.Environment.Exit(65);
                    break;
                case RunResult.RuntimeError:
                    System.Environment.Exit(70);
                    break;
            }
        }

        private static RunResult Run(string text)
        {
            var errors = new List<LoxError>();
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

            PrintErrors(errors);

            return errors.Count > 0 ?
            errors.Any(e => e.Type == LoxError.ErrorType.Runtime) ?
                            RunResult.RuntimeError : RunResult.ParseError : RunResult.Success;
        }

        private static void PrintErrors(List<LoxError> errors)
        {
            foreach (var error in errors)
            {
                Console.Error.WriteLine(error.ToString());
            }
        }
    }
}