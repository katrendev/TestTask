using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestTask
{
    public class FillSingleLetterStats : IReadLetterFromStream
    {
        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public IList<LetterStats> ReadFromStream(IReadOnlyStream stream)
        {
            var list = new List<LetterStats>();

            try
            {
                stream.ResetPositionToStart();

                while (!stream.IsEof)
                {
                    char c = stream.ReadNextChar();
                    if (char.IsLetter(c))
                    {
                        var letter = list.FirstOrDefault(x => x.Letter == c.ToString());
                        if (letter == null)
                        {
                            letter = new LetterStats
                            {
                                Letter = c.ToString(),
                                Count = 0,
                            };
                            list.Add(letter);
                        }

                        LetterContext.IncStatistic(ref letter);
                    }
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