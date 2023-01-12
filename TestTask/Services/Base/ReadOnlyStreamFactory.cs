using TestTask.Services.Interfaces;

namespace TestTask.Services.Base
{
    abstract public class ReadOnlyStreamFactory
    {
        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        abstract public IReadOnlyStream Create(string fileFullPath);
    }
}
