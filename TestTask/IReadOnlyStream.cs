using System;

namespace TestTask
{
    /// <summary>
    /// Интерфейс для работы с файлом в сильно урезаном виде.
    /// Умеет всего 2 вещи: прочитать символ и перемотать стрим на начало
    /// </summary>
    internal interface IReadOnlyStream : IDisposable
    {
        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ</returns>
        char ReadNextChar();

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало
        /// </summary>
        void ResetPositionToStart();

        /// <summary>
        /// Флаг окончания файла
        /// </summary>
        bool IsEof { get; }

    }
}
