using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Text;

namespace Rekog.Data
{
    public class DataFile
    {
        private readonly IFileSystem _fileSystem;

        public DataFile(IFileSystem fileSystem, string path)
        {
            Path = path;
            _fileSystem = fileSystem;
        }

        public DataFile(IFileSystem fileSystem, string path1, string path2)
            : this(fileSystem, fileSystem.Path.Combine(path1, path2))
        {
        }

        public DataFile(IFileSystem fileSystem, string path1, string path2, string path3)
            : this(fileSystem, fileSystem.Path.Combine(path1, path2, path3))
        {
        }

        public DataFile(IFileSystem fileSystem, string path1, string path2, string path3, string path4)
            : this(fileSystem, fileSystem.Path.Combine(path1, path2, path3, path4))
        {
        }

        public string Path { get; }

        private static Encoding Encoding => Encoding.UTF8;

        public bool TryGetReader([MaybeNullWhen(false)] out StreamReader reader)
        {
            try
            {
                reader = GetReader();
                return true;
            }
            catch (Exception e) when (e is FileNotFoundException or DirectoryNotFoundException)
            {
                reader = null;
                return false;
            }
        }

        public StreamReader GetReader()
        {
            var stream = _fileSystem.FileStream.Create(Path, FileMode.Open, FileAccess.Read);
            var reader = new StreamReader(stream, Encoding);
            return reader;
        }

        public StreamWriter GetWriter()
        {
            _fileSystem.FileInfo.FromFileName(Path).Directory.Create();

            var stream = _fileSystem.FileStream.Create(Path, FileMode.Create, FileAccess.Write);
            var writer = new StreamWriter(stream, Encoding);
            return writer;
        }
    }
}
