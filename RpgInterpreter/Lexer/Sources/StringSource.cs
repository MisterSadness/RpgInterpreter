namespace RpgInterpreter.Lexer.Sources
{
    public class StringSource : ICharSource
    {
        private readonly string _string;
        private int _position;

        public StringSource(string s)
        {
            _string = s;
            _position = 0;
        }

        public char? Peek()
        {
            if (_position < _string.Length)
                return _string[_position];
            return null;
        }

        public char? Pop()
        {
            if (_position < _string.Length)
            {
                var result = _string[_position];
                _position++;
                return result;
            }
            return null;
        }
    }
}