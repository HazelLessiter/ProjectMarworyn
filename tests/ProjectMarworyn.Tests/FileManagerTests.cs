using Microsoft.Extensions.Options;
using ProjectMarworyn.Configuration;

namespace ProjectMarworyn.Tests
{
    public class FileManagerTests : IDisposable
    {
        private readonly List<string> _tempFiles = [];

        public void Dispose()
        {
            foreach (var file in _tempFiles)
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }

        [Fact]
        public void ReadInitialPersonFile_WithValidFile_ReturnsNames()
        {
            var json = "[{\"FullName\":\"JaneDoe\",\"Prefix\":\"Jane\",\"Suffix\":\"Doe\",\"Gender\":0},{\"FullName\":\"JohnSmith\",\"Prefix\":\"John\",\"Suffix\":\"Smith\",\"Gender\":1}]";
            var tempFile = CreateTempFile(json);

            var options = Options.Create(new AppSettings { InitialPeopleFilePath = tempFile });
            var fileManager = new FileManager(options);

            var result = fileManager.ReadInitialPersonFile();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("JaneDoe", result[0].FullName);
        }

        [Fact]
        public void ReadInitialPersonFile_FileNotFound_ThrowsFileNotFoundException()
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");

            var options = Options.Create(new AppSettings { InitialPeopleFilePath = path });
            var fileManager = new FileManager(options);

            var ex = Assert.Throws<FileNotFoundException>(() => fileManager.ReadInitialPersonFile());

            Assert.Contains(path, ex.Message);
        }

        [Fact]
        public void ReadInitialPersonFile_InvalidJson_ThrowsInvalidDataException()
        {
            var tempFile = CreateTempFile("this is not valid json");

            var options = Options.Create(new AppSettings { InitialPeopleFilePath = tempFile });
            var fileManager = new FileManager(options);

            var ex = Assert.Throws<InvalidDataException>(() => fileManager.ReadInitialPersonFile());

            Assert.Contains(tempFile, ex.Message);
        }

        private string CreateTempFile(string content)
        {
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
            File.WriteAllText(tempFile, content);
            _tempFiles.Add(tempFile);
            return tempFile;
        }
    }
}