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

        public List<InitialPerson> ReadInitialPersonFile()
        {
            try
            {
                var file = File.ReadAllText(_appSettings.InitialPeopleFilePath);

                var initialPeople = JsonConvert.DeserializeObject<List<InitialPerson>>(file);

                return initialPeople;
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException($"Name file not found at path: {_appSettings.InitialPeopleFilePath}",
                    ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException($"Invalid JSON format in Name file: {_appSettings.InitialPeopleFilePath}",
                    ex);
            }
        }

        public List<SeedWord> ReadSeedWordFile()
        {
            try
            {
                var file = File.ReadAllText(_appSettings.SeedWordFilePath);

                var seedWords = JsonConvert.DeserializeObject<List<SeedWord>>(file);

                return seedWords;
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException($"SeedWord file not found at path: {_appSettings.SeedWordFilePath}",
                    ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException($"Invalid JSON format in SeedWord file: {_appSettings.SeedWordFilePath}",
                    ex);
            }
        }
    }
}