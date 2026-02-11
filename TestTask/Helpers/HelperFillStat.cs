using System.Collections.Generic;

namespace TestTask.Helpers
{
    public static class HelperFillStat
    {
        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();
            Dictionary<string, LetterStats> letterCountsMap = new Dictionary<string, LetterStats>();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый. (+)
                FillSingleStatMap(c, letterCountsMap);
            }
            return new List<LetterStats>(letterCountsMap.Values);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();
            Dictionary<string, LetterStats> pairCountsMap = new Dictionary<string, LetterStats>();
            char prevC = '\0';
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый. (+)
                FillDoubleStatMap(prevC, c, pairCountsMap);
                prevC = c;
            }
            return new List<LetterStats>(pairCountsMap.Values);
        }

        /// <summary>
        /// Метод добавляет в статистику по одиночным буквам, прочитанный символ.
        /// </summary>
        /// <param name="c">Символ для добаления в статистику.</param>
        /// <param name="map">Словарь статистики.</param>
        private static void FillSingleStatMap(char c, Dictionary<string, LetterStats> map)
        {
            if (IsGoodSingleChar(c))
            {
                FillStatMap(BuildSingleKey(c), map);
            }
        }

        /// <summary>
        /// Метод добавляет в статистику по парным буквам, прочитанную пару символов.
        /// </summary>
        /// <param name="c">Первый символ для добаления в статистику.</param>
        /// <param name="c">Второй символ для добаления в статистику.</param>
        /// <param name="map">Словарь статистики.</param>
        private static void FillDoubleStatMap(char c1, char c2, Dictionary<string, LetterStats> map)
        {
            if (IsGoodDoubleChar(c1, c2))
            {
                FillStatMap(BuildDoubleKey(c1, c2), map);
            }
        }

        /// <summary>
        /// Метод строит ключ для одиночной статистики.
        /// </summary>
        /// <param name="c">Символ для построения ключа.</param>
        /// <returns>Ключ для одиночной статистики.</returns>
        private static string BuildSingleKey(char c) => c.ToString();

        /// <summary>
        /// Метод строит ключ для парной статистики.
        /// </summary>
        /// <param name="c1">Первый символ для построения ключа.</param>
        /// <param name="c2">Второй символ для построения ключа.</param>
        /// <returns>Ключ для парной статистики.</returns>
        private static string BuildDoubleKey(char c1, char c2) => $"{c1}{c2}".ToUpper();

        /// <summary>
        /// Метод проверяет символ на принадлежность к букве.
        /// </summary>
        /// <param name="c">Символ для проверки.</param>
        /// <returns>Был ли символ буквой.</returns>
        private static bool IsGoodSingleChar(char c) =>
            char.IsLetter(c);

        /// <summary>
        /// Метод проверяет пару символов на принадлежность к буквам и на равенство букв.
        /// </summary>
        /// <param name="c1">Первый символ для проверки.</param>
        /// <param name="c2">Второй символ для проверки.</param>
        /// <returns>Является ли пара символов парой букв и одинаковы ли они.</returns>
        private static bool IsGoodDoubleChar(char c1, char c2) =>
            IsLetterBoth(c1, c2) && IsEquallyBoth(c1, c2);

        /// <summary>
        /// Метод проверяет пару символов на принадлежность к буквам.
        /// </summary>
        /// <param name="c1">Первый символ для проверки.</param>
        /// <param name="c2">Второй символ для проверки.</param>
        /// <returns>Является ли пара символов парой букв.</returns>
        private static bool IsLetterBoth(char c1, char c2) =>
            char.IsLetter(c1) && char.IsLetter(c2);

        /// <summary>
        /// Метод проверяет пару символов на равенство без учета регистра.
        /// </summary>
        /// <param name="c1">Первый символ для проверки.</param>
        /// <param name="c2">Второй символ для проверки.</param>
        /// <returns>Одинаковы ли символы.</returns>
        private static bool IsEquallyBoth(char c1, char c2) =>
            char.ToUpperInvariant(c1) == char.ToUpperInvariant(c2);

        /// <summary>
        /// Метод увеличивает статистику по существующему ключу или созает новую запись.
        /// </summary>
        /// <param name="key">Ключ для проверки.</param>
        /// <param name="map">Словарь со статистикой.</param>
        private static void FillStatMap(string key, Dictionary<string, LetterStats> map)
        {
            if (map.TryGetValue(key, out LetterStats existingStat))
            {
                IncStatistic(ref existingStat);
                map[key] = existingStat;
            }
            else
            {
                map[key] = CreateLetterStats(key);
            }
        }

        /// <summary>
        /// Метод создает статистику для впервые встретившегося ключа.
        /// </summary>
        /// <param name="key">Ключ статистики.</param>
        /// <returns>Статистика.</returns>
        private static LetterStats CreateLetterStats(string key) =>
            new LetterStats
            {
                Letter = key,
                Count = 1
            };

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(ref LetterStats letterStats)
        {
            letterStats.Count++;
        }
    }
}
