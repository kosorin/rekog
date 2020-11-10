using System;

namespace Rekog.IO
{
    public abstract class DataWriter : IDisposable
    {
        private bool _disposed;

        protected DataWriter(IDataWriter writer)
        {
            Writer = writer;
        }

        protected IDataWriter Writer { get; }

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
                    if (Writer is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
                _disposed = true;
            }
        }
    }
}
