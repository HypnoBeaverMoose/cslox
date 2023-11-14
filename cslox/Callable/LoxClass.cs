
namespace Lox
{
    public class LoxClass : ILoxCallable
    {
        private const string _initMethodName = "init";

        public int Arity => TryGetMethod(_initMethodName, out LoxFunction? initMethod) ? (initMethod?.Arity ?? 0) : 0;

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
            var instance =  new LoxInstance(this);

            if(TryGetMethod(_initMethodName, out LoxFunction? initMethod))
            {
                initMethod?.Bind(instance).Call(interpreter, arguments);
            }

            return instance;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}