using System;
using System.IO;
using System.Threading.Tasks;

namespace Rekog.IO
{
    public class DataReader : IDataReader
    {
        private bool _disposed;

        public DataReader(Stream stream)
        {
            Reader = new StreamReader(stream);
        }

        protected TextReader Reader { get; }

        public Task<string?> ReadLineAsync()
        {
            return Reader.ReadLineAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Reader?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
