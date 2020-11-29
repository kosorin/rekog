using System.IO;
using System.IO.Abstractions;
using System.Text;

namespace Rekog.Core.Corpora
{
    public record CorpusFile(string Path, Encoding Encoding)
    {
        public static Encoding DefaultEncoding { get; } = Encoding.UTF8;

        public StreamReader Open(IFileSystem fileSystem)
        {
            return new StreamReader(fileSystem.FileStream.Create(Path, FileMode.Open, FileAccess.Read), Encoding);
        }
    }
}
