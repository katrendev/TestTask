using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    /// <summary>
    /// The interface for creating a different static for IReadOnlyStream
    /// </summary>
    public interface IStatisticsCreator
    {
        /// <summary>
        /// Creating statistics with register
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <returns></returns>
        public IEnumerable<LetterStats> FillSingleLetterStats(IReadOnlyStream sourceStream);

        /// <summary>
        /// Creating statistics for pair sumbols
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <returns></returns>
        public IEnumerable<LetterStats> FillDoubleLetterStats(IReadOnlyStream sourceStream);
    }
}
