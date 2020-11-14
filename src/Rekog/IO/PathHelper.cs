using System.IO;
using System.IO.Abstractions;

namespace Rekog.IO
{
    public static class PathHelper
    {
        public static string SearchOptionPrefix { get; } = "./";

        public static string DefaultSearchPattern { get; } = "*";

        public static string[] GetPaths(IFileSystem fileSystem, string path, string? searchPattern)
        {
            var attributes = fileSystem.File.GetAttributes(path);
            if (attributes.HasFlag(FileAttributes.Directory))
            {
                searchPattern = !string.IsNullOrWhiteSpace(searchPattern) ? searchPattern : DefaultSearchPattern;

                var searchOption = SearchOption.TopDirectoryOnly;
                if (searchPattern.StartsWith(SearchOptionPrefix))
                {
                    searchPattern = searchPattern[SearchOptionPrefix.Length..];
                    searchOption = SearchOption.AllDirectories;
                }

                return fileSystem.Directory.GetFiles(path, searchPattern, searchOption);
            }
            else
            {
                return new[] { path };
            }
        }
    }
}
