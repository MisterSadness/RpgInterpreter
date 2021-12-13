namespace RpgInterpreter.Parser;

public record ParsingResult;

public record Success : ParsingResult;

public record Failure : ParsingResult;