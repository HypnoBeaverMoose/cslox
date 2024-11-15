namespace Lox
{
    public class ParsingException : Exception
    {
        public LoxError Error { get; }

        public ParsingException()
        {
        }

        public ParsingException(Token token, string message)
        {
            Error = new LoxError(token, message, LoxError.ErrorType.Parse);
        }

        public ParsingException(string? message) : base(message)
        {
        }

        public ParsingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}