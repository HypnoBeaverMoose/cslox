namespace Lox
{
    public class LoxInstance
    {
        private readonly LoxClass _class;

        public LoxInstance(LoxClass loxClass)
        {
            _class = loxClass;
        }

        public override string ToString()
        {
            return $"{_class.Name} instance";
        }
    }
}