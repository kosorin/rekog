using Rekog.IO;
using System.Collections.Generic;
using System.IO.Abstractions;

namespace Rekog.Input.Configurations
{
    public class PathConfig : Input
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Path { get; set; }

        public string SearchPattern { get; set; }

        public bool RecurseSubdirectories { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public string[] GetPaths(IFileSystem fileSystem)
        {
            return PathHelper.GetPaths(fileSystem, Path, SearchPattern, RecurseSubdirectories);
        }

        protected override void FixSelf()
        {
            Path ??= string.Empty;
            SearchPattern ??= PathHelper.DefaultSearchPattern;
        }

        protected override IEnumerable<Input> CollectChildren()
        {
            yield break;
        }
    }
}
