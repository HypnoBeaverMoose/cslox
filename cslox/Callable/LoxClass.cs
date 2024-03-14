
namespace Lox
{
    public class LoxClass : ILoxCallable
    {
        private const string _initMethodName = "init";

        public int Arity => TryGetMethod(_initMethodName, out LoxFunctionBase? initMethod) ? (initMethod?.Arity ?? 0) : 0;

        public readonly string Name;

        private readonly Dictionary<string, LoxFunctionBase> _methods;
        
        public readonly LoxClass Superclass;

        public LoxClass(string name, LoxClass superclass, Dictionary<string, LoxFunctionBase> methods)
        {
            Name = name;
            Superclass = superclass;
            _methods = new Dictionary<string, LoxFunctionBase>(methods);

        }

        public bool TryGetMethod(string name, out LoxFunctionBase? loxFunction)
        {
            return _methods.TryGetValue(name, out loxFunction);
        }

        public object Call(Interpreter interpreter, List<object?> arguments)
        {
            var instance =  new LoxInstance(this);

            if(TryGetMethod(_initMethodName, out LoxFunctionBase? initMethod))
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