using Newtonsoft.Json;
using ProjectMarworyn.Models;

namespace ProjectMarworyn
{
    internal class FileManager : IFileManager
    {
        public List<Name> ReadNameFile()
        {
            var file = File.ReadAllText("C:\\Workspace\\ProjectMarworyn\\src\\ProjectMarworyn\\Configuration\\FileName.Json");//TODO: Fix hardcoded path

            var names = JsonConvert.DeserializeObject<List<Name>>(file);

            return names;
        }
    }
}