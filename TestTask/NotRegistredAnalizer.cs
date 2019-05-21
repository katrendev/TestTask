using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    /// <summary>
    /// Анализ текста по заданной коллекции строк без учета регистра
    /// </summary>
    class NotRegistredAnalizer : IAnalizer
    {
        /// <summary>
        /// Метод анализирует колличество вхождений коллекции строк в заданный текст без учета регистра
        /// </summary>
        /// <param name="analizedText">Анализируемый текст</param>
        /// <param name="valueForSearchList">Коллекция строк для анализа</param>
        /// <returns>Коллекция (строка / колличество вхождений) в заданый текст</returns>
        public List<AnalizeDataModel> GetTextAnalize(string analizedText, List<string> valueForSearchList)
        {
            List<AnalizeDataModel> AnalizeResult = new List<AnalizeDataModel>();
            try
            {
                string analizedTextToUpper = analizedText.ToUpper();
                foreach (string item in valueForSearchList)
                {
                    string s = analizedTextToUpper.Trim('&');
                    s = s.Replace(item, "&");
                    int count = s.Where(c => c =='&').Count();
                    if (count>0)
                    {
                        AnalizeDataModel AmalizeItem = new AnalizeDataModel { Count = count, SerchingText = item };
                        AnalizeResult.Add(AmalizeItem);
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
