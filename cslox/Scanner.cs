using System.Text.RegularExpressions;

namespace Lox
{
    public class Scanner
    {
        private readonly string source;

        private readonly List<Token> tokens = new();

        private bool IsAtEnd => current >= source.Length;
        private int start = 0;
        private int current = 0;
        private int line = 1;

        public IReadOnlyList<Token> Tokens => tokens;

        public Scanner(string text)
        {
            this.source = text;
        }

        public IReadOnlyList<Token> ScanTokens()
        {
            int line = 0;

            while (!IsAtEnd)
            {
                start = current;
                ScanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", literal: null, line));

            return tokens;
        }

        private void ScanToken()
        {
            char c = Advance();

            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                case '!': AddToken(TryMatch('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
                case '>': AddToken(TryMatch('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
                case '<': AddToken(TryMatch('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
                case '=': AddToken(TryMatch('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
                case '/':
                    if (TryMatch('/'))
                    {
                        while(Peek() != '\n' && !IsAtEnd)
                        {
                            Advance();
                        }
                    }
                    break;
                default: Lox.Error(line, $"Unexpected character {c}"); break;
            }
        }

        private char Peek()
        {
            return ' ';
        }

        private void AddToken(TokenType tokenType)
        {
            AddToken(tokenType, null);
        }

        private void AddToken(TokenType tokenType, object literal)
        {
            var lexeme = source.Substring(start, current);
            tokens.Add(new Token(tokenType, lexeme, literal, line));
        }

        private char Advance()
        {
            return source[current++];
        }

        private bool TryMatch(char expected)
        {
            if (!IsAtEnd)
            {
                if (source[current] != expected)
                {
                    return false;
                }

                current++;
                return true;
            }
            return false;
        }
    }
}