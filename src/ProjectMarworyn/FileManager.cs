using Newtonsoft.Json;
using ProjectMarworyn.Models;
using Microsoft.Extensions.Options;
using ProjectMarworyn.Configuration;

namespace ProjectMarworyn
{
    internal class FileManager : IFileManager
    {
        private readonly AppSettings _appSettings;

        public FileManager(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public List<Name> ReadNameFile()
        {
            try
            {
                var file = File.ReadAllText(_appSettings.FilePath);

                var names = JsonConvert.DeserializeObject<List<Name>>(file);

                return names;
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException($"Name file not found at path: {_appSettings.FilePath}", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException($"Invalid JSON format in name file: {_appSettings.FilePath}", ex);
            }
        }
    }
}