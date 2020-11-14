using System;
using System.Threading.Tasks;

namespace Rekog.IO
{
    public interface IDataReader : IDisposable
    {
        Task<string?> ReadLineAsync();
    }
}
