namespace TestTask
{
    /// <summary>
    /// Интерфейс фабрики классов для создания потоков для чтения
    /// </summary>
    internal interface IReadOnlyStreamFactory
    {
        IReadOnlyStream GetInputStream(string fileFullPath);
    }
}
