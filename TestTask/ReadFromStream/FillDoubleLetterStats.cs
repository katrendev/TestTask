using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    public class FillDoubleLetterStats:IReadLetterFromStream
    {
        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public IList<LetterStats> ReadFromStream(IReadOnlyStream stream)
        {
            var list = new List<LetterStats>();

            try
            {
                stream.ResetPositionToStart();

                var prevLetter = '\0';

                while (!stream.IsEof)
                {
                    char cNext = stream.ReadNextChar();
                    char c = char.ToLower(cNext);
                    if (char.IsLetter(c) && c == prevLetter)
                    {
                        var concat = string.Concat(cNext, prevLetter);
                        var pair = list.FirstOrDefault(x => x.Letter == concat);
                        if (pair == null)
                        {
                            pair = new LetterStats
                            {
                                Letter = concat,
                                Count = 0,
                            };
                            list.Add(pair);
                        }

                        LetterContext.IncStatistic(ref pair);
                    }

                    prevLetter = c;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
            finally
            {
                stream.Close();
            }
            return list;
        }
    }
}