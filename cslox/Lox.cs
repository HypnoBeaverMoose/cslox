using System.Collections;

namespace Lox
{
    public static class Lox
    {
        private static bool hadError;

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
                System.Environment.Exit(64);
            }
        }

        private static void Run(string text)
        {
            var tokens = Scanner.Scan(text);
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line} ] Error {where} + : {message}");
            hadError = true;
        }
    }
}