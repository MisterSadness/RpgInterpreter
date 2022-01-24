using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Lexer;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Output;
using RpgInterpreter.Parser;
using RpgInterpreter.Parser.Grammar;
using RpgInterpreter.Runtime;

namespace RpgInterpreter
{
    public class RpgInterpreter
    {
        public RpgInterpreter(IOutput output) => Output = output;
        public RpgInterpreter() => Output = new ConsoleOutput();

        public void InterpretString(string sourceCodeString)
        {
            var errorLocator = new ErrorAreaPrinter(sourceCodeString);
            var handler = new RpgInterpreterExceptionHandler(errorLocator, Output);
            handler.RunAndHandle(() =>
            {
                var stringSource = new StringSource(sourceCodeString);
                var lexer = new TrackingRpgLexer();
                var lexed = lexer.Tokenize(stringSource);

                var parser = new CoolerParser();
                var parsed = parser.Parse(lexed);

                InterpretAst(parsed);
            });
        }

        public void InterpretAst(Root root)
        {
            var typeChecker = new TypeChecker.TypeChecker();
            var typeMap = typeChecker.AnalyzeTypes(root);

            var state = InterpreterState.Initial(typeMap) with { Output = Output };
            state.EvaluateRoot(root);
        }

        public IOutput Output { get; }
    }
}
