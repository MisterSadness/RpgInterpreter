namespace RpgInterpreter;

public record Position(int Line, int Column)
{
    public string Formatted => $"line {Line}, column {Column}";
}