using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    public class SingleLetterStatistics:Statistics
    {
        public SingleLetterStatistics(string fileFullPath):base(fileFullPath)
        {
            
        }

        protected sealed override void FillStatistics()
        {
            using (stream)
            {
                stream.ResetPositionToStart();
                while (!stream.IsEof)
                {
                    var c = stream.ReadNextChar();
                   
                        var letter = c.ToString();

                        if (statistics.Any(x =>x.Letter==letter))
                        {
                            var letterStats = statistics.FirstOrDefault(x => x.Letter == letter);
                            letterStats?.IncStatistic();
                        }
                        else
                        {
                            var letterStats = new LetterStats(letter);
                            statistics.Add(letterStats);
                        }
                    
                }
            }
        }

        public override void PrintStatistic()
        {
            Console.WriteLine("----Регистрозависимая статистика вхождений символов---");
            base.PrintStatistic();
            Console.WriteLine("------------------------------------------------------");
        }
    }
}
