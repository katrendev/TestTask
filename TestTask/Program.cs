using System;
using System.Collections.Generic;

namespace TestTask
{
    class Program
    {
        static void Main(string[] args)
        {        
            Console.WriteLine("Введите полный путь к файлу 1");
            string path1 = Console.ReadLine();

            Console.WriteLine("Введите полный путь к файлу 2");
            string path2 = Console.ReadLine();

            IAnalizer RegistredAnalizer = new RegistredAnalizer();
            IAnalizer NotRegistredAnalizer = new NotRegistredAnalizer();

            string textFromFile1 = FileStreamToString.GetTextFromeFile(path1);
            string textFromFile2 = FileStreamToString.GetTextFromeFile(path2);          
            
            GetAnalize.Print(RegistredAnalizer, textFromFile1, CreateSingleCharList(), $"Результат анализа файла (с учетом регистра) {path1}");
            GetAnalize.Print(NotRegistredAnalizer, textFromFile2, CreateDoubleCharList(), $"Результат анализа файла (без учета регистра) {path2}");

            Console.WriteLine("Все операции завершены!!!\n Для выхода из программы нажмите любую клавишу...");
            Console.ReadKey();
        }

        /// <summary>
        /// Метод гениритует коллекцию символов отсортированного 
        /// русского и английского алфавита в верхнем регистре.
        /// </summary>
        /// <returns>List<string></returns>
        private static List<string> CreateDoubleCharList()
        {
            List<string> singlregList = new List<string>();
            for (char c = 'А'; c <= 'Я'; c++)
            {
                singlregList.Add(c.ToString()+c.ToString());                
            }
            for (char c = 'A'; c <= 'Z'; c++)
            {
                singlregList.Add(c.ToString()+c.ToString());                
            }
            return singlregList;
        }
        /// <summary>
        /// Метод гениритует коллекцию пар символов отсортированного 
        /// русского и английского алфавита в верхнем регистре. 
        /// </summary>
        /// <returns>List<String></returns>
        private static List<string> CreateSingleCharList()
        {
            List<string> singlregList = new List<string>();
            for (char c = 'А'; c <= 'Я'; c++)
            {
                singlregList.Add(c.ToString());
                singlregList.Add(Char.ToLower(c).ToString());
            }
            for (char c = 'A'; c <= 'Z'; c++)
            {
                singlregList.Add(c.ToString());
                singlregList.Add(Char.ToLower(c).ToString());
            }
            return singlregList;
        }
    }
}
