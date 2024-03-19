namespace Lox
{
    public class Environment
    {
        private readonly Environment? _parent;
        private readonly Dictionary<string, object> values = new();

        public Environment? Parent => _parent;

        public Environment()
        {
            _parent = null;
        }

        public Environment(Environment parent)
        {
            _parent = parent;
        }

        public void Define(string name, object value)
        {
            values[name] = value;
        }

        public object Get(Token name)
        {
            if (values.TryGetValue(name.Lexeme, out object value))
            {
                return value;
            }

            if (_parent != null)
            {
                return _parent.Get(name);
            }

            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public void Put(Token token, object value)
        {
            if (values.ContainsKey(token.Lexeme))
            {
                values[token.Lexeme] = value;
            }
            else if (_parent != null)
            {
                _parent.Put(token, value);
            }
            else
            {
                throw new RuntimeException(token, $"Undefined variable '${token.Lexeme}'.");
            }
        }

        public object GetAt(Token name, int distance)
        {
            var ans = Ancestor(distance);

            if (ans.values.TryGetValue(name.Lexeme, out object value))
            {
                return value;
            }

            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public void PutAt(int distance, Token name, object value)
        {
            var ans = Ancestor(distance);

            if (ans.values.ContainsKey(name.Lexeme))
            {
                ans.values[name.Lexeme] = value;
            }
            else
            {
                throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
            }
        }

        private Environment Ancestor(int distance)
        {
            Environment? env = this;
            for (int i = 0; i < distance; i++)
            {
                env = env?._parent;
            }

            System.Diagnostics.Debug.Assert(env != null);

            return env;
        }
    }
}