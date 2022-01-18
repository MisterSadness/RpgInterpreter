﻿using RpgInterpreter.Lexer.InnerLexers;
using RpgInterpreter.Lexer.LexingErrors;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.Lexer;

public class Lexer
{
    private readonly IEnumerable<InnerLexer> _inner;

    public Lexer(IEnumerable<InnerLexer> inner) => _inner = inner;

    public virtual IEnumerable<Token> Tokenize(ICharSource source)
    {
        var c = source.Peek();
        while (c.HasValue)
        {
            var possible = _inner.SingleOrDefault(i => c.Exists(i.FirstCharacterMatches))
                           ?? throw new UnexpectedInputException();

            var matched = possible.Match(source);
            if (matched is not Whitespace)
            {
                yield return possible.Match(source);
            }

            c = source.Peek();
        }

        yield return new EndOfInput();
        yield return new LexingFinished();
    }
}