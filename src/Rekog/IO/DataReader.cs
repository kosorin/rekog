using System;
using System.IO;

namespace Rekog.IO
{
    public abstract class DataReader : IDisposable
    {
        private bool _disposed;

        protected DataReader(string path)
        {
            Reader = new StreamReader(path);
        }

        protected StreamReader Reader { get; }

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
