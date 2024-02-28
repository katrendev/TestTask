using System.Collections.Generic;
using TestTask.View;

namespace TestTask
{
    public class LetterStatisticsController : ILetterStatisticsController
    {
        private ILetterCountModel model;
        private ILetterCountView view;
        private IList<LetterStats> letterStats;

        public LetterStatisticsController(ILetterCountModel model, ILetterCountView view)
        {
            this.model = model;
            this.view = view;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="args">Полный путь до файла для чтения</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public void CalculateLetterStatistics(string args)
        {
            model.ReadStream(args);
            letterStats = model.GetLetterStats();
        }

        public void RemoveCharStatsByType(CharType charType)
        {
            model.RemoveCharStatsByType(letterStats, charType);
        }

        public void PrintStatistic()
        {
            view.PrintStatistic(letterStats);
        }
    }
}