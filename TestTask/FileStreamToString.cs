using System;
using System.IO;

namespace TestTask
{
    /// <summary>
    /// Чтение данных из файла
    /// </summary>
    public static class FileStreamToString
    {
        /// <summary>
        /// Метод для чтения данных из файла и передачи в строку
        /// </summary>
        /// <param name="filePath">Полный путь к файлу</param>
        /// <returns></returns>
        public static string GetTextFromeFile(string filePath)
        {
            string textFromFile =null;
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    textFromFile = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);                
            }

            return textFromFile;
        }
        
    }
}
