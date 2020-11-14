using Rekog.IO;
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
            var path = @"T:\Test\A.txt";

            var paths = PathHelper.GetPaths(fileSystem, path, null);

            Assert.Equal(new[] { path }, paths);
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
            var path = @"T:\Test";

            var paths = PathHelper.GetPaths(fileSystem, path, null);

            Assert.Equal(new[] { @"T:\Test\A.txt", @"T:\Test\B.txt", @"T:\Test\C.gif" }, paths);
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
            var path = @"T:\Test";

            var paths = PathHelper.GetPaths(fileSystem, path, "*.txt");

            Assert.Equal(new[] { @"T:\Test\A.txt", @"T:\Test\B.txt" }, paths);
        }

        [Fact]
        public void GetPaths_Directory_AllSubdirectories()
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
            var path = @"T:\Test";

            var paths = PathHelper.GetPaths(fileSystem, path, PathHelper.SearchOptionPrefix + PathHelper.DefaultSearchPattern);

            Assert.Equal(new[] { @"T:\Test\A.txt", @"T:\Test\B.txt", @"T:\Test\C.gif", @"T:\Test\Sub1\D.txt", @"T:\Test\Sub1\E.txt", @"T:\Test\Sub3\F.gif", @"T:\Test\Sub3\Sub1\G.gif" }, paths);
        }
    }
}
