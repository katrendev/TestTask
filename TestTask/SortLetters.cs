using System.Collections.Generic;

namespace TestTask
{
    /// <summary>
    /// Сортирует данные класса по алфавиту,
    /// равные буквы - с верхним регистром ставится перед буквой с нижним, например а,Б,б,Г,г,Д,д
    /// </summary>
    class SortLetters : IComparer<LetterStats>
    {
        public int Compare(LetterStats l1, LetterStats l2)
        {
            //Код букв в текущем регистре буквы
            int codL1 = (int)l1.Letter[0];
            int codL2 = (int)l2.Letter[0];

            //Код букв в верхнем регистре
            int upperLetter1 = (int)char.ToUpper(l1.Letter[0]);
            int upperLetter2 = (int)char.ToUpper(l2.Letter[0]);

            //Сравниваем код букв в одном регистре, в данном случае в верхнем
            //И если буквы равные, то сравнение происходит по коду текущего регистра буквы
            if (upperLetter1 < upperLetter2)
            {
                return -1;
            }
            else if (upperLetter1 > upperLetter2)
            {
                return 1;
            }
            else
            {
                if (codL1 < codL2) return -1;
                else if (codL1 > codL2) return 1;
                else return 0;
            }
        }
    }
}