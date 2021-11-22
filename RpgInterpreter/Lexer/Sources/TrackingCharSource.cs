using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgInterpreter.Lexer.Sources
{
    class TrackingCharSource : ICharSource
    {
        private readonly ICharSource _inner;

        public TrackingCharSource(ICharSource inner)
        {
            _inner = inner;
        }
        
        private int _line;
        private int _column;

        public Position Position => new(_line, _column);

        public char? Peek() => _inner.Peek();

        public char? Pop()
        {
            var c = _inner.Pop();

            if (c is '\n')
            {
                _line++;
                _column = 0;
            }
            else
            {
                _column++;
            }
            
            return c;
        }
    }
}
