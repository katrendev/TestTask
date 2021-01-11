using System;
using System.Collections.Generic;

namespace TestTask
{
    
    /// <summary>
    /// Reading all data from stream and stored in buffer
    /// </summary>
    public interface IReadOnlyStream: IDisposable
    {
        /// <summary>
        /// Get the all readed symbols exluded a special symbols
        /// </summary>
        IEnumerable<char> Items { get; set; }

        /// <summary>
        /// Reload data
        /// </summary>
        void Reload();

        /// <summary>
        /// Return the current name of file
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// The flag 
        /// </summary>
        bool IsReady { get; }
    }
}
