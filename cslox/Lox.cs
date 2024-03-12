using System.Collections;

namespace Lox
{
    public static class Lox
    {
        private static bool hadError;
        private static bool hadRuntimeError = false;
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
                    hadError = false;
                }
            } while (line != null);
        }

        private static void RunFile(string filename)
        {
            Run(System.IO.File.ReadAllText(filename));
            if (hadError)
            {
                System.Environment.Exit(65);
            }
            if (hadRuntimeError)
            {
                System.Environment.Exit(70);
            }
        }

        private static void Run(string text)
        {
            var errors = new List<LoxError>();
            var (tokens, scanErrors) = Scanner.Scan(text);
            errors.AddRange(scanErrors);

            if (REPLHelper.IsExpression(tokens))
            {
                REPLHelper.FixExpression(tokens);
            }

            var (statements, parserErrors) = Parser.Parse(tokens);
            errors.AddRange(parserErrors);

            if (parserErrors.Count == 0)
            {
                var (locals, resolveErrors) = _resolver.Resolve(statements);
                errors.AddRange(resolveErrors);

                if (!hadError && resolveErrors.Count == 0)
                {
                    if (REPLHelper.TryGetSingleExpression(statements, out Expr? expression))
                    {
                        Console.WriteLine(_interpreter.Evaluate(expression));
                    }
                    else
                    {
                        _interpreter.Interpret(statements, locals);
                    }
                }
            }

            foreach (var error in errors)
            {
                Console.Error.WriteLine(error.ToString());
            }
        }

        public static void RuntimeError(RuntimeException re)
        {
            Console.Error.WriteLine(re.Message + $"\n [line{re.Token.Value.Line}]");
            hadRuntimeError = true;
        }
    }

    public struct LoxError
    {
        public string Where => Token.TokenType == TokenType.NONE ? "" :
                    Token.TokenType == TokenType.EOF ? " at end" : $"at '{Token.Lexeme}'";

        public string ErrorText => $"[line {Line} ] Error {Where} : {Message}";

        public readonly Token Token;

        public readonly string Message;

        public readonly int Line;

        public LoxError(int line, string message)
        {
            Message = message;
            Token = new Token();
            Line = line;
        }

        public LoxError(Token token, string message)
        {
            Message = message;
            Token = token;
            Line = token.Line;
        }

        public override string ToString() => ErrorText;
    }
}