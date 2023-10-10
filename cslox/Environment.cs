namespace Lox
{
    public class Environment
    {
        private readonly Dictionary<string, object?> values = new();

        public void Define(string name, object? value)
        {
            values[name] = value;
        }

        public object? Get(Token name)
        {
            if(values.TryGetValue(name.Lexeme, out object? value))
            {
                return value;
            }
            
            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public void Put(Token token, object? value)
        {
            if(values.ContainsKey(token.Lexeme))
            {
                values[token.Lexeme] = value;
            }
            else
            {
                throw new RuntimeException(token, $"Undefined variable '${token.Lexeme}'.");
            }
        }
    }
}