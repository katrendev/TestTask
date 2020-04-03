namespace TestTask
{
    /// <summary>
    /// Интерфейс для работы с файлом в сильно урезаном виде.
    /// Умеет следующее: прочитать символ, перемотать поток на начало и закрыть поток.
    /// </summary>
    internal interface IReadOnlyStream
    {
        char ReadNextChar();

        void ResetPositionToStart();

        void Close();

        bool IsEof { get; }
    }
}