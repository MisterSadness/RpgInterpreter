using System.Text;

namespace RpgInterpreter.ExceptionHandler
{
    public class ErrorAreaPrinter : IDisposable
    {
        public ErrorAreaPrinter(string sourceCode) => SourceCode = new StringReader(sourceCode);
        public TextReader SourceCode { get; }

        public string FindErrorSurroundings(IPointPositionedException exception)
        {
            var (targetLine, targetColumn) = exception.Position;
            var currentLine = -1;
            var delta = 1;

            var output = new StringBuilder();

            while (SourceCode.ReadLine() is { } lineString)
            {
                currentLine++;

                if (Math.Abs(currentLine - targetLine) <= delta)
                {
                    output.AppendLine(lineString);
                }

                if (currentLine == targetLine)
                {
                    output.Append(new string(' ', targetColumn));
                    output.AppendLine("^");
                }
            }

            return output.ToString();
        }

        public void Dispose()
        {
            SourceCode.Dispose();
        }
    }
}
