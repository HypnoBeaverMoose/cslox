namespace Lox
{
    public readonly struct LoxError
    {
        public enum ErrorType { Parse, Runtime }

        public string Where => Token?.TokenType == TokenType.NONE ? "" :
                    Token?.TokenType == TokenType.EOF ? " at end" : $"at '{Token?.Lexeme}'";

        public string ErrorText => $"[line {Line} ] Error {Where} : {Message}";

        public readonly Token? Token;

        public readonly string Message;

        public readonly int Line;

        public readonly ErrorType Type;

        public LoxError(int line, string message)
        {
            Message = message;
            Token = new Token();
            Line = line;
            Type = ErrorType.Parse;
        }

        public LoxError(Token? token, string message, ErrorType errorType)
        {
            Message = message;
            Token = token;
            Line = token?.Line ?? 0;
            Type = errorType;
        }

        public override string ToString() => ErrorText;
    }
}