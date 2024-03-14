namespace Lox
{
    public enum RunResult { Success, ParseError, RuntimeError }

    public abstract class EvaluatorBase
    {
        protected Interpreter _interpreter = new();
        protected Resolver _resolver = new();

        protected Action<List<LoxError>> _logErrors;

        public EvaluatorBase(Action<List<LoxError>> logErrors)
        {
            _logErrors = logErrors;
        }

        public RunResult Run(string text)
        {
            var errors = new List<LoxError>();

            Run(text, errors);

            _logErrors(errors);

            return errors.Count > 0 ?
            errors.Any(e => e.Type == LoxError.ErrorType.Runtime) ?
                            RunResult.RuntimeError : RunResult.ParseError : RunResult.Success;
        }

        protected abstract void Run(string text, List<LoxError> errors);
    }
}