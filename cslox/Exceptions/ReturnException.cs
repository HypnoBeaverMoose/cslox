namespace Lox
{
    public class ReturnException : RuntimeException
    {
        public readonly object Value;

        public ReturnException(object value) : base(null)
        {
            Value = value;
        }
    }
}