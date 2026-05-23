using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core
{
    public interface IGenerationManager
    {
        Generation Initialise(List<Person> people);
        bool CheckForExtinction(List<Person> people);
    }
}