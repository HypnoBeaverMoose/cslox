
namespace Lox;

public class Clock : ILoxCallable
{
    public int Arity => 0;

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        return (double)DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000.0;
    }
}
