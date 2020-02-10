using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    public class DoubleLetterStatistics:Statistics
    {
        public DoubleLetterStatistics(string fileFullPath) : base(fileFullPath)
        {
            
        }

        protected sealed override void FillStatistics()
        {
            using (stream)
            {
                stream.ResetPositionToStart();
                var prevChar = new char();
                while (!stream.IsEof)
                {
                    var c = stream.ReadNextChar();
                    if (char.ToUpperInvariant(c) == char.ToUpperInvariant(prevChar))
                    {
                        var letter = new string(new[] {c, prevChar});

                        if (statistics.Any(x =>
                            string.Equals(x.Letter, letter, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            var letterStats = statistics.FirstOrDefault(x =>
                                string.Equals(x.Letter, letter, StringComparison.CurrentCultureIgnoreCase));
                            letterStats?.IncStatistic();
                        }
                        else
                        {
                            var letterStats = new LetterStats(letter);
                            statistics.Add(letterStats);
                        }
                    }

                    prevChar = c;
                }
            }
        }
        public override void PrintStatistic()
        {
            Console.WriteLine("----Регистронезависимая статистика вхождений подряд идущих символов---");
            base.PrintStatistic();
            Console.WriteLine("----------------------------------------------------------------------");
        }
    }
}
