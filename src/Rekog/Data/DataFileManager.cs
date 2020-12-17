using System.IO;
using System.IO.Abstractions;
using System.Text;

namespace Rekog.Data
{
    public class DataFileManager
    {
        private readonly string _root;
        private readonly IFileSystem _fileSystem;

        public DataFileManager(string? root, IFileSystem fileSystem)
        {
            if (string.IsNullOrWhiteSpace(root))
            {
                root = ".";
            }
            _root = fileSystem.DirectoryInfo.FromDirectoryName(root).FullName;

            _fileSystem = fileSystem;
        }

        public StreamReader GetConfigReader()
        {
            var path = GetConfigPath();
            return GetReader(path);
        }

        public bool CorpusReportExists(string corpus, string alphabet)
        {
            var path = GetCorpusReportPath(corpus, alphabet);
            return _fileSystem.FileInfo.FromFileName(path).Exists;
        }

        public StreamReader GetCorpusReportReader(string corpus, string alphabet)
        {
            var path = GetCorpusReportPath(corpus, alphabet);
            return GetReader(path);
        }

        public StreamWriter GetCorpusReportWriter(string corpus, string alphabet)
        {
            var path = GetCorpusReportPath(corpus, alphabet);
            return GetWriter(path);
        }

        private StreamReader GetReader(string path)
        {
            var stream = _fileSystem.FileStream.Create(path, FileMode.Open, FileAccess.Read);
            var reader = new StreamReader(stream, Encoding.UTF8);
            return reader;
        }

        private StreamWriter GetWriter(string path)
        {
            var fileInfo = _fileSystem.FileInfo.FromFileName(path);
            fileInfo.Directory.Create();
            var stream = _fileSystem.FileStream.Create(path, FileMode.Create, FileAccess.Write);
            var writer = new StreamWriter(stream, Encoding.UTF8);
            return writer;
        }

        private string GetConfigPath() => GetPath(_root, "config.yml");

        private string GetCorpusReportPath(string corpus, string alphabet) => GetPath(_root, "corpus", corpus, alphabet + ".yml");

        private string GetPath(params string[] paths) => _fileSystem.Path.Combine(paths);
    }
}
