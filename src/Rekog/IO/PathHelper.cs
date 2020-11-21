using System.IO;
using System.IO.Abstractions;

namespace Rekog.IO
{
    public static class PathHelper
    {
        public static string DefaultSearchPattern { get; } = "*";

        public static string[] GetPaths(IFileSystem fileSystem, string path)
        {
            return GetPaths(fileSystem, path, DefaultSearchPattern, false);
        }

        public static string[] GetPaths(IFileSystem fileSystem, string path, string searchPattern, bool recurseSubdirectories)
        {
            var attributes = fileSystem.File.GetAttributes(path);
            if (attributes.HasFlag(FileAttributes.Directory))
            {
                var searchOption = recurseSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                return fileSystem.Directory.GetFiles(path, searchPattern, searchOption);
            }
            else
            {
                return new[] { fileSystem.Path.GetFullPath(path) };
            }
        }
    }
}
