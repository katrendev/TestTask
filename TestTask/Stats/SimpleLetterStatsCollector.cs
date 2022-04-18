namespace TestTask.Stats
{
    /// <summary>
    /// Класс сборщика статистики вхождения букв в поток (регистрозависимо)
    /// </summary>
    public class SimpleLetterStatsCollector : LetterStatsCollector
    {
        public static readonly LetterStatsCollector Instance = new LetterStatsCollector();
        
        private SimpleLetterStatsCollector()
        {
        }
        
        protected override bool IsCharAllowed(char ch) => char.IsLetter(ch);
    }
}