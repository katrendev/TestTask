using System;

namespace TestTask
{
    /// <summary>
    /// Интерфейс для работы с файлом в сильно урезаном виде.
    /// Умеет всего 2 вещи: прочитать символ, и перемотать стрим на начало.
    /// </summary>
    internal interface IReadOnlyStream : IDisposable
    {
        /// <summary>
        /// Читает следующий символ из потока.
        /// </summary>
        /// <returns>Считанный символ.</returns>
        char ReadNextChar();

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        void ResetPositionToStart();

        /// <summary>
        /// Флаг, указывающий на окончание файла.
        /// </summary>
        bool IsEof { get; }
    }
}
