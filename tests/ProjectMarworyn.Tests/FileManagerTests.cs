using Microsoft.Extensions.Options;
using ProjectMarworyn.Configuration;

namespace ProjectMarworyn.Tests
{
    public class FileManagerTests
    {
        [Fact]
        public void ReadNameFile_WithValidFile_ReturnsNames()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");

            var json = "[{\"FullName\":\"JaneDoe\",\"Prefix\":\"Jane\",\"Suffix\":\"Doe\",\"Gender\":0},{\"FullName\":\"JohnSmith\",\"Prefix\":\"John\",\"Suffix\":\"Smith\",\"Gender\":1}]";

            File.WriteAllText(tempFile, json);

            var options = Options.Create(new AppSettings { FilePath = tempFile });
            var fileManager = new FileManager(options);

            var result = fileManager.ReadNameFile();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("JaneDoe", result[0].FullName);

            // cleanup
            File.Delete(tempFile);
        }

        [Fact]
        public void ReadNameFile_FileNotFound_ThrowsExceptionWithFriendlyMessage()
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");

            var options = Options.Create(new AppSettings { FilePath = path });
            var fileManager = new FileManager(options);

            var ex = Assert.Throws<Exception>(() => fileManager.ReadNameFile());

            Assert.Contains("Failed to get Name file", ex.Message);
        }
    }
}