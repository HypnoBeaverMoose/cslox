
namespace Lox
{
    public class LoxFunction : ILoxCallable
    {
        public int Arity => _function.Parameters.Count;

        private readonly Stmt.Function _function;
        private readonly Environment _closure;

        public LoxFunction(Stmt.Function function, Environment closure)
        {
            _function = function;
            _closure = closure;
        }

        public object Call(Interpreter interpreter, List<object?> arguments)
        {
            var env = new Environment(_closure);
            for (int i = 0; i < arguments.Count; i++)
            {
                env.Define(_function.Parameters[i].Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(_function.Body, env);
            }
            catch(ReturnException ret)
            {
                return ret.Value;
            }

            return null;
        }

        public override string ToString()
        {
            return $"<fn {_function.Name.Lexeme}>";
        }
    }
}