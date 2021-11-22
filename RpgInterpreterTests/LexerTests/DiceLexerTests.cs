﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RpgInterpreter.Lexer;
using RpgInterpreter.Lexer.InnerLexers;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Tokens;

namespace RpgInterpreterTests.LexerTests
{
    class DiceLexerTests
    {
        private readonly DiceOrNaturalLexer _lexer = new ();

        [TestCaseSource(nameof(_dice))]
        public void UppercaseWordTest(SingleTestData data)
        {
            var result = _lexer.Match(data.Source);

            Assert.That(result, Is.EqualTo(data.Output));
        }

        private static SingleTestData[] _dice = {
            new ("123", new NaturalLiteral(123)),
            new ("2d4", new DiceLiteral(2, 4)),
            new ("12731231", new NaturalLiteral(12731231)),
            new ("12d413", new DiceLiteral(12, 413)),
            new ("1d413", new DiceLiteral(1, 413)),
            new ("121d4", new DiceLiteral(121, 4)),
        };
    }
}