using Newtonsoft.Json;
using ProjectMarworyn.Models;
using Microsoft.Extensions.Options;
using ProjectMarworyn.Configuration;

namespace ProjectMarworyn
{
    internal class FileManager : IFileManager
    {
        private readonly Configuration.AppSettings _appSettings;

        public FileManager(IOptions<Configuration.AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public List<Name> ReadNameFile()
        {
            var file = File.ReadAllText(_appSettings.FilePath);

            var names = JsonConvert.DeserializeObject<List<Name>>(file);

            return names;
        }
    }
}