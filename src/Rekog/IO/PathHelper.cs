using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rekog.IO
{
    public static class PathHelper
    {
        public static string DefaultSearchPattern { get; } = string.Empty;

        public static string[] GetPaths(IFileSystem fileSystem, string path)
        {
            return GetPaths(fileSystem, path, DefaultSearchPattern, false);
        }

        public static string[] GetPaths(IFileSystem fileSystem, string path, string searchPattern)
        {
            return GetPaths(fileSystem, path, searchPattern, false);
        }

        public static string[] GetPaths(IFileSystem fileSystem, string path, bool recurseSubdirectories)
        {
            return GetPaths(fileSystem, path, DefaultSearchPattern, recurseSubdirectories);
        }

        public static string[] GetPaths(IFileSystem fileSystem, string path, string searchPattern, bool recurseSubdirectories)
        {
            var attributes = fileSystem.File.GetAttributes(path);
            if (attributes.HasFlag(FileAttributes.Directory))
            {
                return fileSystem.Directory
                    .EnumerateFiles(path, "*", new EnumerationOptions
                    {
                        RecurseSubdirectories = recurseSubdirectories,
                    })
                    .Where(x => string.IsNullOrEmpty(searchPattern) || Regex.IsMatch(x, searchPattern, RegexOptions.CultureInvariant))
                    .ToArray();
            }
            else
            {
                return new[] { fileSystem.Path.GetFullPath(path) };
            }
        }
    }
}
