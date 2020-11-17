using System;

namespace Rekog.IO
{
    public abstract class ReaderBase : IDisposable
    {
        private bool _disposed;

        protected ReaderBase(IDataReader reader)
        {
            Reader = reader;
        }

        protected IDataReader Reader { get; }

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
                    if (Reader is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
                _disposed = true;
            }
        }
    }
}
