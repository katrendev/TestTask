using System;
using System.IO;

namespace TestTask
{
    /// <summary>
    /// Простая фабрика для создания потоков для чтения
    /// </summary>
    internal class BaseReadOnlyStreamFactory : IReadOnlyStreamFactory
    {
        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <exception cref="ArgumentNullException">Выкидывается в случае передачи пустой строки</exception> 
        /// <exception cref="DirectoryNotFoundException">Выкидывается в случае передачи несуществующей директории</exception> 
        /// <exception cref="FileNotFoundException">Выкидывается в случае, если файл не существует</exception> 
        /// <exception cref="NullReferenceException">Выкидывается в случае, если не удалось создать итоговый объект</exception> 
        /// <returns>Поток для последующего чтения.</returns>
        public virtual IReadOnlyStream GetInputStream(string fileFullPath)
        {
            if (string.IsNullOrWhiteSpace(fileFullPath))
            {
                throw new ArgumentNullException("Передано null вместо названия файла");
            }

            if (!Directory.Exists(Path.GetDirectoryName(fileFullPath)))
            {
                throw new DirectoryNotFoundException($"Директории с файлом {fileFullPath} не существует");
            }

            if (!File.Exists(fileFullPath))
            {
                throw new FileNotFoundException($"Файла в пути {fileFullPath} не существует");
            }

            IReadOnlyStream stream = null;
            Stream dataStream = null;
            StreamReader reader = null;
            try
            {
                dataStream = new FileStream(fileFullPath, FileMode.Open);
                reader = new StreamReader(dataStream);
                stream = new ReadOnlyStream(dataStream, reader);
            }
            catch (Exception ex)
            {
                if (dataStream != null)
                {
                    dataStream.Dispose();
                }
                if (reader != null)
                {
                    reader.Dispose();
                }
                throw new NullReferenceException($"Ошибка, не удалось создать объект ReadOnlyStream\nТекст ошибки {ex.Message}");
            }

            return stream;
        }
    }
}
