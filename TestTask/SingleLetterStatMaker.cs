using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
   internal class SingleLetterStatMaker : LetterStatMaker
    {
        public SingleLetterStatMaker(IReadOnlyStream stream) : base(stream) { }
        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        protected override IList<LetterStats> FillLetterStats()
        {
            Dictionary<char, LetterStats> letterStats = new Dictionary<char, LetterStats>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (!char.IsLetter(c)) continue;

                if (!letterStats.ContainsKey(c)) letterStats.Add(c, new LetterStats(c));
                IncStatistic(letterStats[c]);
            }
            return letterStats.Values.ToList();
        }
    }
}
