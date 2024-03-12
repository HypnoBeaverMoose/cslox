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

            if (REPLHelper.IsExpression(tokens))
            {
                REPLHelper.FixExpression(tokens);
            }

            var (statements, errors) = Parser.Parse(tokens);

            if (!hadError && errors.Count == 0)
            {
                var resolver = new Resolver(_interpreter);
                resolver.Resolve(statements);

                if (!hadError)
                {
                    if (REPLHelper.TryGetSingleExpression(statements, out Expr? expression))
                    {
                        Console.WriteLine(_interpreter.Evaluate(expression));
                    }
                    else
                    {
                        _interpreter.Interpret(statements);
                    }
                }
            }
            else if(errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    Console.Error.WriteLine(error.ToString());
                }
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
            Console.Error.WriteLine($"[line {line} ] Error {where} : {message}");
            hadError = true;
        }

        public static void RuntimeError(RuntimeException re)
        {
            Console.Error.WriteLine(re.Message + $"\n [line{re.Token.Value.Line}]");
            hadRuntimeError = true;
        }
    }

    public struct LoxError
    {
        public string Where => Token.TokenType == TokenType.EOF ? " at end" : $"at '{Token.Lexeme}'";

        public string ErrorText => $"[line {Token.Line} ] Error {Where} : {Message}";

        public readonly Token Token;

        public readonly string Message;

        public LoxError(string message, Token token)
        {
            Message = message;
            Token = token;
        }

        public override string ToString() => ErrorText;
    }
}