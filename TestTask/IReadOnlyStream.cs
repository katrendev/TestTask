using System;
namespace TestTask
{
    /// <summary>
    /// Интерфейс для работы с файлом.
    /// Умеет прочитать символ, перемотать стрим на начало, а также закрыть поток.
    /// </summary>
    internal interface IReadOnlyStream : IDisposable
    {
    
        char ReadNextChar();

        void ResetPositionToStart();

        bool IsEof { get; }
        
        void Close();
    }
}
