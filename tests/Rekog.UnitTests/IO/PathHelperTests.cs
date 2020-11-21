using Rekog.IO;
using Shouldly;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Rekog.UnitTests.IO
{
    public class PathHelperTests
    {
        [Fact]
        public void GetPaths_File()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [@"T:\Test\A.txt"] = new MockFileData(string.Empty),
            });

            var paths = PathHelper.GetPaths(fileSystem, @"T:\Test\A.txt", null, false);

            paths.ShouldBe(new[] { @"T:\Test\A.txt" }, ignoreOrder: true);
        }

        [Fact]
        public void GetPaths_RelativeFile()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [@"T:\Test\A.txt"] = new MockFileData(string.Empty),
            }, @"T:\Test\");

            var paths = PathHelper.GetPaths(fileSystem, "A.txt", null, false);

            paths.ShouldBe(new[] { @"T:\Test\A.txt" }, ignoreOrder: true);
        }

        [Fact]
        public void GetPaths_RelativeDirectory()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [@"T:\ROOT.txt"] = new MockFileData(string.Empty),
                [@"T:\Test\A.txt"] = new MockFileData(string.Empty),
                [@"T:\Test\B.txt"] = new MockFileData(string.Empty),
                [@"T:\Test\C.gif"] = new MockFileData(string.Empty),
                [@"T:\Test\Sub1\D.txt"] = new MockFileData(string.Empty),
            }, @"T:\Test\");

            var paths = PathHelper.GetPaths(fileSystem, ".", null, false);

            paths.ShouldBe(new[] { @"T:\Test\A.txt", @"T:\Test\B.txt", @"T:\Test\C.gif" }, ignoreOrder: true);
        }

        [Fact]
        public void GetPaths_Directory_DefaultSearchPattern()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [@"T:\ROOT.txt"] = new MockFileData(string.Empty),
                [@"T:\Test\A.txt"] = new MockFileData(string.Empty),
                [@"T:\Test\B.txt"] = new MockFileData(string.Empty),
                [@"T:\Test\C.gif"] = new MockFileData(string.Empty),
                [@"T:\Test\Sub1\D.txt"] = new MockFileData(string.Empty),
            });

            var paths = PathHelper.GetPaths(fileSystem, @"T:\Test", null, false);

            paths.ShouldBe(new[] { @"T:\Test\A.txt", @"T:\Test\B.txt", @"T:\Test\C.gif" }, ignoreOrder: true);
        }

        [Fact]
        public void GetPaths_Directory_TxtSearchPattern()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [@"T:\ROOT.txt"] = new MockFileData(string.Empty),
                [@"T:\Test\A.txt"] = new MockFileData(string.Empty),
                [@"T:\Test\B.txt"] = new MockFileData(string.Empty),
                [@"T:\Test\C.gif"] = new MockFileData(string.Empty),
                [@"T:\Test\Sub1\D.txt"] = new MockFileData(string.Empty),
            });

            var paths = PathHelper.GetPaths(fileSystem, @"T:\Test", "*.txt", false);

            paths.ShouldBe(new[] { @"T:\Test\A.txt", @"T:\Test\B.txt" }, ignoreOrder: true);
        }

        [Fact]
        public void GetPaths_Directory_RecurseSubdirectories()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [@"T:\ROOT.txt"] = new MockFileData(string.Empty),
                [@"T:\Test\A.txt"] = new MockFileData(string.Empty),
                [@"T:\Test\B.txt"] = new MockFileData(string.Empty),
                [@"T:\Test\C.gif"] = new MockFileData(string.Empty),
                [@"T:\Test\Sub1\D.txt"] = new MockFileData(string.Empty),
                [@"T:\Test\Sub1\E.txt"] = new MockFileData(string.Empty),
                [@"T:\Test\Sub2\"] = new MockDirectoryData(),
                [@"T:\Test\Sub3\F.gif"] = new MockFileData(string.Empty),
                [@"T:\Test\Sub3\Sub1\G.gif"] = new MockFileData(string.Empty),
            });

            var paths = PathHelper.GetPaths(fileSystem, @"T:\Test", PathHelper.DefaultSearchPattern, true);

            paths.ShouldBe(new[] { @"T:\Test\A.txt", @"T:\Test\B.txt", @"T:\Test\C.gif", @"T:\Test\Sub1\D.txt", @"T:\Test\Sub1\E.txt", @"T:\Test\Sub3\F.gif", @"T:\Test\Sub3\Sub1\G.gif" }, ignoreOrder: true);
        }
    }
}
