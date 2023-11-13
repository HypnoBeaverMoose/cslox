
namespace Lox
{
    public class LoxClass : ILoxCallable
    {
        public int Arity => 0;

        public readonly string Name;

        private readonly Dictionary<string, LoxFunction> _methods;

        public LoxClass(string name, Dictionary<string, LoxFunction> methods)
        {
            Name = name;
            _methods = new Dictionary<string, LoxFunction>(methods);
        }

        public bool TryGetMethod(string name, out LoxFunction? loxFunction)
        {
            return _methods.TryGetValue(name, out loxFunction);
        }

        public object Call(Interpreter interpreter, List<object?> arguments)
        {
            return new LoxInstance(this);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}