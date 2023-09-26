using System.Text.RegularExpressions;

namespace Lox
{
    public class Scanner
    {
        private readonly Dictionary<string, TokenType> keywords = new()
        {
            {"and", TokenType.AND},
            {"class", TokenType.CLASS},
            {"else", TokenType.ELSE},
            {"false", TokenType.FALSE},
            {"for", TokenType.FOR},
            {"fun", TokenType.FUN},
            {"if", TokenType.IF},
            {"nil", TokenType.NIL},
            {"or", TokenType.OR},
            {"print", TokenType.PRINT},
            {"return", TokenType.RETURN},
            {"super", TokenType.SUPER},
            {"this", TokenType.THIS},
            {"true", TokenType.TRUE},
            {"var", TokenType.VAR},
            {"while", TokenType.WHILE},
        };

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
                case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
                case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
                case '<': AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
                case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
                //TODO: Separate method here?
                case '/':
                    if (Match('/'))
                    {
                        SlashComment();
                    }
                    else if (Match('*'))
                    {
                        BlockComment();
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                //TODO: Can we combine these?
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    line++;
                    break;
                case '"': String(); break;
                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsLetterOrUnderscore(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Lox.Error(line, $"Unexpected character {c}");
                    }
                    break;
            }
        }

        private void BlockComment()
        {
            while (!(Peek(0) == '*' && Peek(1) == '/'))
            {
                if (Advance() == '\n')
                {
                    line++;
                }
            }

            Advance();
            Advance();
        }

        private bool IsDigit(char c)
        {
            return char.IsDigit(c);
        }

        private bool IsLetterOrUnderscore(char c)
        {
            return char.IsAsciiLetter(c) || c == '_';
        }

        private bool IsAlphanumeric(char c)
        {
            return IsLetterOrUnderscore(c) || IsDigit(c);
        }

        private void Identifier()
        {
            while (IsAlphanumeric(Peek()))
            {
                Advance();
            }

            var value = source.SubstringFromTo(start, current);
            if (keywords.TryGetValue(value, out TokenType tokenType))
            {
                AddToken(tokenType, value);
            }
            else
            {
                AddToken(TokenType.IDENTIFIER);
            }
        }

        private void SlashComment()
        {
            while (Peek() != '\n' && !IsAtEnd)
            {
                Advance();
            }
        }

        private void Number()
        {
            while (IsDigit(Peek()))
            {
                Advance();
            }

            if (Peek() == '.' && IsDigit(Peek(1)))
            {
                Advance();

                while (IsDigit(Peek()))
                {
                    Advance();
                }
            }

            AddToken(TokenType.NUMBER, source.SubstringFromTo(start, current));
        }

        private void String()
        {
            while (Peek() != '"' && !IsAtEnd)
            {
                if (Advance() == '\n')
                {
                    line++;
                }
            }

            if (IsAtEnd)
            {
                Lox.Error(line, "Unterminated string.");
                return;
            }

            Advance();

            var value = source.SubstringFromTo(start + 1, current - 1);
            AddToken(TokenType.STRING, value);
        }

        private char Peek(int ahead = 0)
        {
            return current + ahead >= source.Length ? '\0' : source[current + ahead];
        }

        private void AddToken(TokenType tokenType)
        {
            AddToken(tokenType, null);
        }

        private void AddToken(TokenType tokenType, object? literal)
        {
            var lexeme = source.SubstringFromTo(start, current);
            tokens.Add(new Token(tokenType, lexeme, literal, line));
        }

        private char Advance()
        {
            return source[current++];
        }

        private bool Match(char expected)
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