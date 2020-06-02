using System;

namespace TestTask
{
    /// <summary>
    /// Текстовые сообщения 
    /// </summary>
    class Title
    {
        public string MainTitle { get; private set; } = "Программа принимает на входе два пути до текстовых файлов и производит анализ переданных файлов согласно выбранных пользователем шаблонов.\n" +
                                                        "Например анализирует в первом файле кол-во вхождений каждой буквы(регистрозависимо): А, б, Б, Г и т.д.\n" +
                                                        "Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.\n" +
                                                        "По окончанию работы - выводит данную статистику на экран.";
        public string GeneralExampleMessage { get; private set; } = "Укажите путь до файла в формате [имя диска:]/[имя каталога]/[имя файла] и нажмите ENTER.\nПример ввода: C:\\user\\docs\\Letter.txt ";
        public static string FirstPathFileMessage { get; private set; } = "Путь к файлу №1: ";
        public static string SecondPathFileMessage { get; private set; } = "Путь к файлу №2: ";
        public static string ErrorMessage { get; private set; } = "Файл не найден. Повторите попытку";


        public Title()
        {
            Console.WriteLine(MainTitle);
            Console.WriteLine(GeneralExampleMessage);
        }
    }


}