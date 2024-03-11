namespace Lox
{
    public class ParsingException : Exception
    {
        public string where => _token.TokenType == TokenType.EOF ? " at end" : $"at '{_token.Lexeme}'";

        public string message => $"[line {_token.Line} ] Error {where} : {_message}";

        private Token _token;
        private string _message;

        public ParsingException()
        {
            _token = new Token();
            _message = "";
        }

        public ParsingException(Token token, string message)
        {
            _token = token;
            _message = message;
        }

        public ParsingException(string? message) : base(message)
        {
            _token = new Token();
            _message = "";
        }

        public ParsingException(string? message, Exception? innerException) : base(message, innerException)
        {
            _token = new Token();
            _message = "";
        }
    }
}