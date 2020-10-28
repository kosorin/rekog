using System;
using System.IO;

namespace Rekog.Persistence
{
    public abstract class PersistenceWriter : IDisposable
    {
        private bool _disposed;

        public PersistenceWriter(TextWriter writer)
        {
            Writer = writer;
        }

        protected TextWriter Writer { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }
}
