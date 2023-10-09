using System.Collections;

namespace Lox
{
    public static class Lox
    {
        private static bool hadError;
        private static bool hadRuntimeError = false;
        private static Interpreter _interpreter = new();

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
            var tokens = Scanner.Scan(text);
            var statements = new Parser(tokens).Parse();

            if (!hadError)
            {
                _interpreter.Interpret(statements);
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        public static void Error(Token token, string message)
        {
            var where = token.TokenType == TokenType.EOF ? " at end" : $"at '{token.Lexeme}'";
            Report(token.Line, where, message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line} ] Error {where} + : {message}");
            hadError = true;
        }

        internal static void RuntimeError(Interpreter.RuntimeException re)
        {
            Console.Error.WriteLine(re.Message + $"\n [line{re.Token.Line}]");
            hadRuntimeError = true;
        }
    }
}