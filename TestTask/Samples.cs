#if DEBUG
namespace TestTask
{
    internal class Samples
    {
        /// <summary>
        /// Пример текста для статистики вхождения каждой буквы.
        /// </summary>
        /// <returns>Пример текста.</returns>
        public static string Single()
        {
            return "Это простой текст.\nОн содержит различные буквы.\nТекст может содержать заглавные и строчные буквы.";
        }

        /// <summary>
        /// Пример текста для статистики вхождения парных буквы.
        /// </summary>
        /// <returns>Пример текста.</returns>
        public static string Double()
        {
            return "Это еще один пример текста.\nОн содержит пары символов, такие как FF, ББ, DD и т.д.\nТекст может содержать заглавные и строчные буквы.";
        }
    }
}
#endif