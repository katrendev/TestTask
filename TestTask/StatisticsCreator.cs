using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    /// <summary>
    /// Implementation for interface IStatisticsCreator
    /// </summary>
    public class StatisticsCreator : IStatisticsCreator
    {
        public StatisticsCreator()
        { }

        internal class LetterPosition
        {
            internal LetterPosition() { }

            #region fields
            internal char Symbol { get; set; }

            internal int Position { get; set; }


            #endregion


            #region convert
            internal static IEnumerable<LetterPosition> ConvertTo(IEnumerable<char> sourceList)
            {
                if (sourceList == null) return null;

                var result = new List<LetterPosition>();
                var data = sourceList.ToList();

                Enumerable.Range(0, sourceList.Count())
                    .ToList()
                    .ForEach(x =>
                    {
                        var item = data[x];
                        result.Add(new LetterPosition() { Position = x, Symbol = item});
                    });

                return result;
            }

            internal static IEnumerable<char> ConvertTo(IEnumerable<LetterPosition> sourceList)
            {
                if (sourceList == null) return null;
                return sourceList.Select(x => x.Symbol);
            }

            #endregion
        }

        #region logics
        public IEnumerable<LetterStats> FillDoubleLetterStats(IReadOnlyStream sourceStream)
        {
            if (sourceStream == null)
                throw new Exception("Invalid parameter!");

            var result = new List<LetterStats>();
            var leftData = LetterPosition.ConvertTo(sourceStream.Items).ToList();
            var rightData = new List<LetterPosition>() { new LetterPosition() { Symbol = '\0', Position = 0 } };

            if (leftData.Any())
            {
                // Copying data from 1 -  N to rightData
                Enumerable.Range(1, leftData.Count() - 1)
                    .ToList()
                    .ForEach(x =>
                    {
                        var sourceItem = leftData[x];
                        rightData.Add(sourceItem);
                    });

                // Linking records
                var data = (from s in leftData.ToList()
                            from p in rightData.ToList()
                            where 
                                string.Compare(s.ToString(), p.ToString(), StringComparison.OrdinalIgnoreCase) >= 0
                                && s.Position + 1 == p.Position
                            // Gluing sumbols
                            select s.Symbol.ToString() + p.Symbol.ToString())
                            .ToList();

                // Creating result
                result = data
                    .GroupBy(x => x)
                    .Select(x => new LetterStats()
                    {
                        Letter = x.Key,
                        Count = x.Count()
                    }).ToList();
            }

            return result;
        }

        public IEnumerable<LetterStats> FillSingleLetterStats(IReadOnlyStream sourceStream)
        {
            if (sourceStream == null)
                throw new Exception("Invalid parameter!");

            var result = new List<LetterStats>();
            var data = sourceStream.Items;

            if (data.Any())
            {
                result = data.ToList()
                    .Select(x => x)
                    .GroupBy(x => x)
                    .Select(x => new LetterStats()
                    {
                        Letter = x.Key.ToString(),
                        Count = x.Count()
                    }).ToList();
            }

            return result;
        }

        /// <summary>
        /// Return IEnumerable<LetterStats> with excluded spectial chars
        /// </summary>
        /// <param name="sourceList"></param>
        /// <param name="exludedChars"></param>
        /// <returns></returns>
        public IEnumerable<LetterStats> RemoveCharStatsByType(IEnumerable<LetterStats> sourceList, IEnumerable<string> exludedChars)
        {
            if (exludedChars == null || sourceList == null)
                throw new Exception("Invalid parameter");

            if (!exludedChars.Any())
                return sourceList;

            var result = sourceList.Where(x => exludedChars.Any(y => !x.Letter.Contains(y))).ToList();
            return result;
        }

        /// <summary>
        /// Return IEnumerable<LetterStats> with excluded spacial types
        /// </summary>
        /// <param name="sourceList"></param>
        /// <param name="charType"></param>
        /// <returns></returns>
        public IEnumerable<LetterStats> RemoveCharStatsByType(IEnumerable<LetterStats> sourceList, CharType charType)
        {
            if ( sourceList == null)
                throw new Exception("Invalid parameter");

            var consonantsList = new List<string>() { "БВГДЖЗКЛМНПРСТФХЦЧЬЪ" };
            var vowelList = new List<string>() { "АЕЁИЙУОЭЮЯ" };

            var data = sourceList.ToList();

            var result = new List<LetterStats>();
            Enumerable.Range(0, sourceList.Count())
                .ToList()
                .ForEach(x =>
                {
                    var currentItem = data[x];
                    var currentChar = data[x].Letter;
                    var passedResult = false;

                    if (charType == CharType.Consonants && consonantsList.Contains(currentChar))
                        passedResult = true;

                    if (charType == CharType.Vowel && consonantsList.Contains(currentChar))
                        passedResult = true;

                    if (!passedResult)
                        result.Add(currentItem);
                });

            return result;
        }

        #endregion

        #region implementation IStatisticsCreator
        IEnumerable<LetterStats> IStatisticsCreator.FillDoubleLetterStats(IReadOnlyStream sourceStream)
        {
            return FillDoubleLetterStats(sourceStream);
        }

        IEnumerable<LetterStats> IStatisticsCreator.FillSingleLetterStats(IReadOnlyStream sourceStream)
        {
            return FillSingleLetterStats(sourceStream);
        }

        #endregion

       
    }
}
