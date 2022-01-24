using System.Text;

namespace RpgInterpreter.ExceptionHandler;

public class ErrorAreaPrinter
{
    public ErrorAreaPrinter(string sourceCode) => SourceCode = sourceCode;
    private string SourceCode { get; }

    public string FindErrorSurroundings(IPositionedException exception)
    {
        return exception switch
        {
            IPointPositionedException e => FindErrorSurroundings(e),
            IIntervalPositionedException e => FindErrorSurroundings(e),
            _ => ""
        };
    }

    public string FindErrorSurroundings(IIntervalPositionedException exception)
    {
        using var reader = new StringReader(SourceCode);
        var (startLine, startColumn) = exception.Start;
        var (endLine, endColumn) = exception.End;
        var currentLine = -1;
        var delta = 1;

        var output = new StringBuilder();
        output.AppendLine($"At: {exception.Start.Formatted}");

        while (reader.ReadLine() is { } lineString && currentLine < endLine + delta)
        {
            currentLine++;

            if (startLine - delta <= currentLine && currentLine <= endLine + delta)
            {
                output.AppendLine(lineString);
            }

            if (startLine < currentLine && currentLine < endLine)
            {
                output.Append(new string('^', lineString.Length));
                output.AppendLine("^");
            }

            if (currentLine == startLine && currentLine == endLine)
            {
                output.Append(new string(' ', startColumn));
                output.AppendLine(new string('^', endColumn - startColumn + 1));
            }
            else if (currentLine == startLine)
            {
                output.Append(new string(' ', startColumn));
                output.AppendLine(new string('^', lineString.Length - startColumn + 1));
            }
            else if (currentLine == endLine)
            {
                output.AppendLine(new string('^', endColumn + 1));
            }
        }

        return output.ToString();
    }

    public string FindErrorSurroundings(IPointPositionedException exception)
    {
        using var reader = new StringReader(SourceCode);
        var (targetLine, targetColumn) = exception.Position;
        var currentLine = -1;
        var delta = 1;

        var output = new StringBuilder();
        output.AppendLine($"At: {exception.Position.Formatted}");

        while (reader.ReadLine() is { } lineString && currentLine < targetLine + delta)
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
}