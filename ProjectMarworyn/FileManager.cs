using Newtonsoft.Json;
using ProjectMarworyn.Models;

namespace ProjectMarworyn
{
    internal class FileManager : IFileManager
    {
        public List<Name> ReadNameFile()
        {
            var file = File.ReadAllText("C:\\Workspace\\ProjectMarworyn\\ProjectMarworyn\\Configuration\\FileName.Json");

            var names = JsonConvert.DeserializeObject<List<Name>>(file);

            return names;
        }
    }
}
