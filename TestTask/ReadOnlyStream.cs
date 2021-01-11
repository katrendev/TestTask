using System;
using System.Collections.Generic;
using System.IO;

namespace TestTask
{
    /// <summary>
    /// Implementation for interface IReadOnlyStream
    /// </summary>
    public class ReadOnlyStream : IReadOnlyStream
    {
        #region fields

        private string _localFileName = "";
        private IEnumerable<char> _buffer = null;
        private bool _isReady = false;

        #endregion

        #region ctor

        public ReadOnlyStream()
        { }

        public ReadOnlyStream(string fileFullPath)
        {
            if (!File.Exists(fileFullPath))
                throw new Exception($"File not found! See path: {fileFullPath}");

            _localFileName = fileFullPath;
            ReloadAsync();
        }

        #endregion

        #region implement IReadOnlyStream
        IEnumerable<char> IReadOnlyStream.Items { get => _buffer; set => _buffer = value; }

        string IReadOnlyStream.FileName => _localFileName;

        bool IReadOnlyStream.IsReady => _isReady;

        void IDisposable.Dispose()
        {
            // Clear buffer
            if (_buffer != null)
                _buffer = null;
        }

        void IReadOnlyStream.Reload()
        {
            ReloadAsync();
        }

        #endregion

        #region implement IDisposable

        public void Dispose()
        {
            if (_buffer != null)
                _buffer = null;
        }

        #endregion

        /// <summary>
        /// Reload data from stream to buffer 
        /// </summary>
        private async void ReloadAsync()
        {
            _isReady = false;
            if (!string.IsNullOrEmpty(_localFileName))
            {
                using (var reader = new StringReader(_localFileName))
                {
                    var data = await reader.ReadToEndAsync();
                    _buffer = data.ToCharArray();
                    _isReady = true;
                }
            }
            else
                throw new Exception("File not listed!");
        }
    }
}
