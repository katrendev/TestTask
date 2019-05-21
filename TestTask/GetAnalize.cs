using System;
using System.Collections.Generic;

namespace TestTask
{
    /// <summary>
    /// Формирование анализа
    /// </summary>
    public static class GetAnalize
    {
       /// <summary>
       /// Метод для формирования и вывода анализа текста
       /// </summary>
       /// <param name="analizer">Объект с логикой анализа текста</param>
       /// <param name="analizedText">Текст для анализа</param>
       /// <param name="vluesForAnalize">Коллекция строк для анализа</param>
       /// <param name="title">Заголовок данных анализа</param>
       public static void Print(IAnalizer analizer, string analizedText, List<string> vluesForAnalize, string title)
        {
            try
            {
                List<string> VluesForAnalize = vluesForAnalize;
                List<AnalizeDataModel> analizList = analizer.GetTextAnalize(analizedText, vluesForAnalize);
                Console.WriteLine("-------------------------------------------------------------");
                Console.WriteLine(title);
                Console.WriteLine("-------------------------------------------------------------");
                foreach (var item in analizList)
                {
                    Console.WriteLine($"Колличество вхождений '{item.SerchingText}' в анализируемом тексте = {item.Count}");
                }
                Console.WriteLine("-------------------------------------------------------------");
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            
        }
    }    
}
