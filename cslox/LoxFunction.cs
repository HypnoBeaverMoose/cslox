
namespace Lox
{

    public class LoxFunction : ILoxCallable
    {
        public int Arity => _function.Parameters.Count;

        private readonly Stmt.Function _function;

        public LoxFunction(Stmt.Function function)
        {
            _function = function;
        }

        public object Call(Interpreter interpreter, List<object?> arguments)
        {
            var env = new Environment(interpreter.Globals);
            for (int i = 0; i < arguments.Count; i++)
            {
                env.Define(_function.Parameters[i].Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(_function.Body, env);
            }
            catch(Return ret)
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