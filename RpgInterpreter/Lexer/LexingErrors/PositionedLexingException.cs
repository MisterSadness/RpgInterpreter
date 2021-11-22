using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgInterpreter.Lexer.Sources;

namespace RpgInterpreter.Lexer.LexingErrors
{
    class PositionedLexingException : LexingException
    {
        public PositionedLexingException(LexingException inner, Position position)
        {
            Inner = inner;
            Position = position;
        }

        public LexingException Inner { get; }
        public Position Position { get; }
    }
}
