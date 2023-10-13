using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    public class LettersSequence
    {
        /// <summary>
        /// Индекс, куда пойдет следующий символ.
        /// </summary>
        private int _iterator = 0;
        public LettersSequence(int length) { 
            if (length <= 0)
                throw new ArgumentOutOfRangeException("Парамметр length должен быть больше чем 0");
            Letters = new char?[length];
            Clear();
        }

        /// <summary>
        /// Массив, хранящий последовательность символов
        /// </summary>
        public char?[] Letters { get; private set; }

        /// <summary>
        /// Добавляет в последовательность новый символ,
        /// если в хранящем массиве закончилось место,
        /// начинаем записывать в начало.
        /// </summary>
        /// <param name="c"></param>
        public void Add(char c)
        {
            Letters[_iterator] = c;
            _iterator = (_iterator + 1) % Letters.Length;
        }

        /// <summary>
        /// Каждый элемент инициализируется null.
        /// </summary>
        public void Clear()
        {
            _iterator = 0;
            for(int i = 0; i < Letters.Length; i++)
            {
                Letters[i] = null;
            }
        }

        /// <summary>
        /// Если вся последовательность состоит из одинаковых символов, возвращает этот символ.
        /// Если нет, возвращает null.
        /// </summary>
        /// <returns>Повторяющийся символ, или null</returns>
        public char? IsAllSame()
        {
            char? ans = Letters[0];
            for (int i = 1; i < Letters.Length; i++)
            {
                if (Letters[0] != Letters[i] || Letters[i] == null)
                {
                    ans = null;
                    break;
                }
            }
            return ans;
        }
    }
}
