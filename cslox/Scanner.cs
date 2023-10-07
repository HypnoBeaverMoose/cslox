namespace Lox
{
    public static class Scanner
    {
        private static string _source = "";

        private static readonly List<Token> _tokens = new();

        private static bool IsAtEnd => _current >= _source.Length;
        private static int _start;
        private static int _current;
        private static int _line;

        public static IReadOnlyList<Token> Tokens => _tokens;

        public static List<Token> Scan(string text)
        {
            _line = 0;
            _source = text;
            _start = _current = 0;
            _tokens.Clear();
            while (!IsAtEnd)
            {
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.EOF, "", literal: null, _line));

            return new List<Token>(_tokens);
        }

        private static void ScanToken()
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
                    _line++;
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
                        Lox.Error(_line, $"Unexpected character {c}");
                    }
                    break;
            }
        }

        private static void BlockComment()
        {
            while (!(Peek(0) == '*' && Peek(1) == '/'))
            {
                if (Advance() == '\n')
                {
                    _line++;
                }
            }

            Advance();
            Advance();
        }

        private static bool IsDigit(char c)
        {
            return char.IsDigit(c);
        }

        private static bool IsLetterOrUnderscore(char c)
        {
            return char.IsAsciiLetter(c) || c == '_';
        }

        private static bool IsAlphanumeric(char c)
        {
            return IsLetterOrUnderscore(c) || IsDigit(c);
        }

        private static void Identifier()
        {
            while (IsAlphanumeric(Peek()))
            {
                Advance();
            }

            var value = _source.Substring(_start, _current);
            if (Keywords.TryGetTokenType(value, out TokenType tokenType))
            {
                AddToken(tokenType, value);
            }
            else
            {
                AddToken(TokenType.IDENTIFIER);
            }
        }

        private static void SlashComment()
        {
            while (Peek() != '\n' && !IsAtEnd)
            {
                Advance();
            }
        }

        private static void Number()
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

            AddToken(TokenType.NUMBER, _source.SubstringFromTo(_start, _current));
        }

        private static void String()
        {
            while (Peek() != '"' && !IsAtEnd)
            {
                if (Advance() == '\n')
                {
                    _line++;
                }
            }

            if (IsAtEnd)
            {
                Lox.Error(_line, "Unterminated string.");
                return;
            }

            Advance();

            var value = _source.SubstringFromTo(_start + 1, _current - 1);
            AddToken(TokenType.STRING, value);
        }

        private static char Peek(int ahead = 0)
        {
            return _current + ahead >= _source.Length ? '\0' : _source[_current + ahead];
        }

        private static void AddToken(TokenType tokenType)
        {
            AddToken(tokenType, null);
        }

        private static void AddToken(TokenType tokenType, object? literal)
        {
            var lexeme = _source.SubstringFromTo(_start, _current);
            _tokens.Add(new Token(tokenType, lexeme, literal, _line));
        }

        private static char Advance()
        {
            return _source[_current++];
        }

        private static bool Match(char expected)
        {
            if (!IsAtEnd)
            {
                if (_source[_current] != expected)
                {
                    return false;
                }

                _current++;
                return true;
            }
            return false;
        }
    }
}