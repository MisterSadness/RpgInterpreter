using System;
using System.IO;

namespace RpgInterpreter.Lexer.Sources
{
    public class FileSource : ICharSource, IDisposable
    {
        private readonly StreamReader _reader;

        public FileSource(string path)
        {
            _reader = new StreamReader(path);
        }

        public virtual char? Peek()
        {
            var result = _reader.Peek();
            return result == -1 ? null : (char)result;
        }

        public virtual char? Pop()
        {
            var result = _reader.Read();
            return result == -1 ? null : (char) result;
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}