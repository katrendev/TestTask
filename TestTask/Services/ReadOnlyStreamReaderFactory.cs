using TestTask.Services.Base;
using TestTask.Services.Interfaces;

namespace TestTask.Services
{
    public class ReadOnlyStreamReaderFactory : ReadOnlyStreamFactory
    {
        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        public override IReadOnlyStream Create(string fileFullPath)
        {
            return new ReadOnlyStreamReader(fileFullPath);
        }
    }
}   
