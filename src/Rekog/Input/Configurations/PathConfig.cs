using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

namespace Rekog.Input.Configurations
{
    public class PathConfig : Input
    {
        public static string DefaultSearchPattern { get; } = "*";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Path { get; set; }

        public string SearchPattern { get; set; }

        public bool RecurseSubdirectories { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public string[] GetPaths(IFileSystem fileSystem)
        {
            var attributes = fileSystem.File.GetAttributes(Path);
            if (attributes.HasFlag(FileAttributes.Directory))
            {
                var searchOption = RecurseSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                return fileSystem.Directory.GetFiles(Path, SearchPattern, searchOption);
            }
            else
            {
                return new[] { fileSystem.Path.GetFullPath(Path) };
            }
        }

        protected override void FixSelf()
        {
            Path ??= string.Empty;
            SearchPattern ??= string.Empty;
        }

        protected override IEnumerable<Input> CollectChildren()
        {
            yield break;
        }
    }
}
