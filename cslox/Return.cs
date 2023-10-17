namespace Lox
{
    public class Return : RuntimeException
    {
        public readonly object? Value;

        public Return(object? value) : base(null)
        {
            Value = value;
        }
    }
}