﻿using Optional.Unsafe;
using RpgInterpreter.Lexer.LexingErrors;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.Lexer.InnerLexers;

public class SeparatorLexer : InnerLexer
{
    public override bool FirstCharacterMatches(char c) => "(){}[]:;,".Contains(c);

    public override Token Match(ICharSource source)
    {
        var c = source.Pop().ToNullable() ?? throw new UnexpectedEndOfInputException();

        return c switch
        {
            '(' => new OpenParen(),
            ')' => new CloseParen(),
            '[' => new OpenBracket(),
            ']' => new CloseBracket(),
            '{' => new OpenBrace(),
            '}' => new CloseBrace(),
            ':' => new Colon(),
            ';' => new Semicolon(),
            ',' => new Comma(),
            _ => throw new UnexpectedEndOfInputException() // FIXME
        };
    }
}