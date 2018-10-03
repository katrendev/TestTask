using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
   internal class DoubleLetterStatMaker : LetterStatMaker
    {
        public DoubleLetterStatMaker(IReadOnlyStream stream) : base(stream) { }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>        
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        protected override IList<LetterStats> FillLetterStats()
        {
            Dictionary<char, LetterStats> letterStats = new Dictionary<char, LetterStats>();
            stream.ResetPositionToStart();
            char? prev = null;

            while (!stream.IsEof)
            {
                var c = Char.ToUpper(stream.ReadNextChar());
                if (!char.IsLetter(c))
                {
                    prev = null;
                    continue;
                }
                if (prev == c) // найдена пара одинаковых букв
                {
                    if (!letterStats.ContainsKey(c)) letterStats.Add(c, new LetterStats(c));
                    IncStatistic(letterStats[c]);
                    prev = null;
                }
                else prev = c;
            }
            return letterStats.Values.ToList();
        }
    }
}
