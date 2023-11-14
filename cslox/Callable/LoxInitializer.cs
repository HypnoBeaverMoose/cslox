
namespace Lox
{
    public class LoxInitializer : LoxFunctionBase
    {
        private readonly Token _this = new Token(TokenType.THIS, "this", "this", -1);

        public LoxInitializer(Stmt.Function function, Environment closure) : base(function, closure)
        {
        }

        public override object Call(Interpreter interpreter, List<object?> arguments)
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
            catch (ReturnException)
            {
                return _closure.GetAt(_this, 0);
            }

            return _closure.GetAt(_this, 0);
        }

        public override string ToString()
        {
            return $"<method {_function.Name.Lexeme}>";
        }

    }
}