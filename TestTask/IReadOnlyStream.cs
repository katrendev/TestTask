using System;

namespace TestTask
{
    public interface IReadOnlyStream : IDisposable
    {
        bool IsEof { get; }

        char ReadNextChar();

        void ResetPositionToStart();

        void Close();
    }
}