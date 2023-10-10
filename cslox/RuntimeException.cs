namespace Lox
{
    public class RuntimeException : Exception
    {
        public readonly Token Token;

        public RuntimeException(Token token)
        {
            Token = token;
        }

        public RuntimeException(Token token, string? message) : base(message)
        {
            Token = token;
        }

        public RuntimeException(Token token, string? message, Exception? innerException) : base(message, innerException)
        {
            Token = token;
        }
    }
}