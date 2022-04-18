namespace TestTask.Stats
{
    /// <summary>
    /// Класс сборщика статистики вхождения букв в поток (регистрозависимо)
    /// </summary>
    public class SimpleLetterStatsCollector : LetterStatsCollector
    {
        public static readonly SimpleLetterStatsCollector Instance = new SimpleLetterStatsCollector();

        private SimpleLetterStatsCollector()
        {
        }

        protected override bool IsCharAllowed(char ch)
        {
            return char.IsLetter(ch);
        }
    }
}