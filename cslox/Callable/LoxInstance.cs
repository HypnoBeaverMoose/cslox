namespace Lox
{
    public class LoxInstance
    {
        private readonly LoxClass _class;

        private readonly Dictionary<string, object?> _properties = new();

        public LoxInstance(LoxClass loxClass)
        {
            _class = loxClass;
        }

        public object? Get(Token name)
        {
            if(_properties.TryGetValue(name.Lexeme, out object? val))
            {
                return val;
            }
            else
            {
                throw new RuntimeException(name, $"Undefined property '{name.Lexeme}'.");
            }
        }

        public override string ToString()
        {
            return $"{_class.Name} instance";
        }
    }
}