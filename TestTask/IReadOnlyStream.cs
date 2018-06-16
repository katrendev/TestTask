using System;

namespace TestTask
{
    /// <summary>
    ///     Интерфейс для работы с файлом в сильно урезаном виде.
    ///     Умеет всего 2 вещи: прочитать символ, и перемотать стрим на начало.
    /// </summary>
    public interface IReadOnlyStream : IDisposable
    {
        bool IsEof { get; }

        char ReadNextChar();

        void ResetPositionToStart();

        void Close();
    }
}