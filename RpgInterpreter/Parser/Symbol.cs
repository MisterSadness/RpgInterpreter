namespace RpgInterpreter.Parser;

public record Symbol;

public record ProductionEnd(Symbol Start) : Symbol;