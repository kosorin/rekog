using System;
using System.Text;
using System.Threading.Tasks;

namespace Rekog.IO
{
    public interface IDataWriter : IDisposable
    {
        Task WriteAsync(StringBuilder value);

        Task WriteAsync(string value);

        Task WriteLineAsync();

        Task WriteLineAsync(StringBuilder value);

        Task WriteLineAsync(string value);
    }
}
