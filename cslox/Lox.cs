namespace Lox
{
    public static class Lox
    {
        private readonly static REPL _repl = new(PrintErrors);
        private readonly static FileEvaluator _eval = new(PrintErrors);

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

        private static void RunFile(string filename)
        {
            var result = _eval.Run(System.IO.File.ReadAllText(filename));

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

        private static void RunPrompt()
        {
            string? line;
            do
            {
                Console.Write(">");
                line = Console.ReadLine();
                if (line != null)
                {
                    _repl.Run(line);
                }
            } while (line != null);
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