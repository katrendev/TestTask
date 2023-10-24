using System;

namespace TestTask
{
    internal interface IReadOnlyStream : IDisposable
    {
        char ReadNextChar();

        void ResetPositionToStart();

        bool IsEndOfStream { get; }
    }
}
