﻿namespace TestTask
{
    /// <summary>
    /// Интерфейс для работы с файлом в сильно урезаном виде.
    /// Умеет всего 2 вещи: прочитать символ, и перемотать стрим на начало.
    /// </summary>
    public interface IReadOnlyStream
    {
        char ReadNextChar();

        void ResetPositionToStart();

        bool IsEof { get; }
        void CloseStream();
    }
}
