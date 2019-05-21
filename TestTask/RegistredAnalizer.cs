using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    /// <summary>
    /// Анализ текста по заданой коллекции строк с учетом регистра
    /// </summary>
    class RegistredAnalizer : IAnalizer
    {
        /// <summary>
        /// Метод анализирует количество вхождений коллекции строк в заданый текст c учетом регистра
        /// </summary>
        /// <param name="analizedText">Анализируемый текст</param>
        /// <param name="valueForSearchList">Коллекция строк для анализа</param>
        /// <returns>Коллекция (строка / колличество вхождений) в заданый текст</returns>
        public List<AnalizeDataModel> GetTextAnalize(string analizedText, List<string> valueForSearchList)
        {
            List<AnalizeDataModel> AnalizeResult = new List<AnalizeDataModel>();
            try
            {
                string AnalizeText = analizedText;                
                foreach (string item in valueForSearchList)
                {
                    string s = analizedText.Trim('&');
                    s = s.Replace(item, "&");
                    int count = s.Where(c => c=='&').Count();
                    if (count>0)
                    {
                        AnalizeDataModel AnalizeItem = new AnalizeDataModel { Count = count, SerchingText = item };
                        AnalizeResult.Add(AnalizeItem);
                    }
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return AnalizeResult;
        }
    }
}
