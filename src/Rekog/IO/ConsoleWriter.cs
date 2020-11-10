using System;
using System.CommandLine.IO;
using System.Text;
using System.Threading.Tasks;

namespace Rekog.IO
{
    public class ConsoleWriter : IDataWriter
    {
        private bool _disposed;

        public ConsoleWriter(IStandardStreamWriter writer)
        {
            Writer = writer;
        }

        protected IStandardStreamWriter Writer { get; }

        public Task WriteAsync(StringBuilder value)
        {
            Writer.Write(value.ToString());
            return Task.CompletedTask;
        }

        public Task WriteAsync(string value)
        {
            Writer.Write(value);
            return Task.CompletedTask;
        }

        public Task WriteLineAsync()
        {
            Writer.WriteLine();
            return Task.CompletedTask;
        }

        public Task WriteLineAsync(StringBuilder value)
        {
            Writer.WriteLine(value.ToString());
            return Task.CompletedTask;
        }

        public Task WriteLineAsync(string value)
        {
            Writer.WriteLine(value);
            return Task.CompletedTask;
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
                _disposed = true;
            }
        }
    }
}
