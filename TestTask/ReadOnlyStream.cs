using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _localStreamReader;

        /// <summary>
        /// Конструктор класса. 
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            // TODO : Заменить на создание реального стрима для чтения файла!
            try
            {
                _localStreamReader = new StreamReader(fileFullPath, Encoding.UTF8);
            }
            catch
            {
                _localStreamReader = null;
            }
        }


        /// <summary>
        /// Функция для считывания всех символов из потока
        /// </summary>
        /// <returns>Считанные символы либо null, в случае если поток не был создан по каким-либо причинам</returns>
        public char[] ReadAllChar()
        {
            if (_localStreamReader != null)
            {
                string allChars = _localStreamReader.ReadToEnd();
                StreamClose();
                return allChars.ToCharArray();
            }
            return null;
        }

        /// <summary>
        /// Функция для гарантированного закрытия и освобождения ресурсов, связанных с потоком
        /// </summary>
        public void StreamClose()
        {
            _localStreamReader.Close();
            _localStreamReader.Dispose();
        }
    }
}
