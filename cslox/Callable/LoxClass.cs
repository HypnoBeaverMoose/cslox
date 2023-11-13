
namespace Lox
{
    public class LoxClass : ILoxCallable
    {
        public int Arity => 0;

        public readonly string Name;

        public LoxClass(string name)
        {
            Name = name;
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