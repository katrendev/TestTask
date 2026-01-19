using System;

namespace TestTask
{
    /// <summary>
    /// Интерфейс для работы с потоком файла в сильно урезаном виде.
    /// Обеспечивает всего 2 задачи: прочитать символ и перемотать стрим на начало.
    /// </summary>
    internal interface IReadOnlyStream : IDisposable
    {
        /// <summary>
        /// Флаг окончания файла
        /// </summary>
        bool IsEof { get; }

        /// <summary>
        /// Читает следующий символ из потока.
        /// </summary>
        char ReadNextChar();

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        void ResetPositionToStart();
    }
}
