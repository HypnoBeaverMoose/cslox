
namespace Lox
{
    public class LoxFunction : LoxFunctionBase
    {
        public LoxFunction(Stmt.Function function, Environment closure) : base(function, closure)
        {
        }

        public override object Call(Interpreter interpreter, List<object> arguments)
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
    }
}