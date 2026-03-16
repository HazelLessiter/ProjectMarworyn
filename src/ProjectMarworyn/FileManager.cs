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
                using (FileStream fileStream = new FileStream(_appSettings.FilePath.ToString(),
                    FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        var file = reader.ReadToEnd();

                        if (file == null)
                        {
                            return new List<Name>();
                        }
                        var names = JsonConvert.DeserializeObject<List<Name>>(file);

                        return names;
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception($"Failed to get Name file - {ex}, {ex?.Message}, {ex?.InnerException}, {ex?.StackTrace}");
            }
        }
    }
}