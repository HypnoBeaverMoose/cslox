
namespace Lox
{
    public abstract class LoxFunctionBase : ILoxCallable
    {
        public int Arity => _function.Parameters.Count;

        protected readonly Stmt.Function _function;
        protected  readonly Environment _closure;

        public LoxFunctionBase(Stmt.Function function, Environment closure)
        {
            _function = function;
            _closure = closure;
        }

        public LoxFunction Bind(LoxInstance instance)
        {
            var env = new Environment(_closure);
            env.Define("this", instance);
            return new LoxFunction(_function, env);
        }

        public abstract object Call(Interpreter interpreter, List<object> arguments);

        public override string ToString()
        {
            return $"<fn {_function.Name.Lexeme}>";
        }

        public static LoxFunctionBase Create(Stmt.Function function, Environment closure)
        {
            if(function.Name.Lexeme == "init")
            {
                return new LoxInitializer(function, closure);
            }
            else
            {
                return new LoxFunction(function, closure);
            }
        }
    }
}