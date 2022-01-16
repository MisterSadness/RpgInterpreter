using System.Collections.Immutable;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState(IImmutableQueue<Token> Queue);