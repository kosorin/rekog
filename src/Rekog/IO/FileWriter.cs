using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Rekog.IO
{
    public class FileWriter : IDataWriter
    {
        private bool _disposed;

        public FileWriter(Stream stream)
        {
            Writer = new StreamWriter(stream);
        }

        protected TextWriter Writer { get; }

        public Task WriteAsync(StringBuilder value)
        {
            return Writer.WriteAsync(value);
        }

        public Task WriteAsync(string value)
        {
            return Writer.WriteAsync(value);
        }

        public Task WriteLineAsync()
        {
            return Writer.WriteLineAsync();
        }

        public Task WriteLineAsync(StringBuilder value)
        {
            return Writer.WriteLineAsync(value);
        }

        public Task WriteLineAsync(string value)
        {
            return Writer.WriteLineAsync(value);
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
                    Writer?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
