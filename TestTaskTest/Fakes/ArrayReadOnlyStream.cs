using System.IO;
using TestTask;

namespace TestTaskTest
{
    /// <summary>
    /// Симулирует чтение из файла с помощью массива
    /// </summary>
    public class ArrayReadOnlyStream : IReadOnlyStream
    {
        private char[] _text;
        private bool _isEOF = false;
        private int _position = 0;

        public bool IsEof 
        {
            get => _isEOF;
            private set => _isEOF = value;
        }

        public ArrayReadOnlyStream(char[] text)
        {
            _text = text;
            _position = 0;
            IsEof = _position == _text.Length;
        }

        public char ReadNextChar()
        {
            if (IsEof)
            {
                throw new EndOfStreamException("Попытка чтения файла после достижения границы");
            }

            char symbol = _text[_position++];
            if (_position == _text.Length)
            {
                _isEOF = true;
            }

            return symbol;
        }

        public void ResetPositionToStart()
        {
            _position = 0;
        }

        public void Dispose()
        {

        }
    }
}
