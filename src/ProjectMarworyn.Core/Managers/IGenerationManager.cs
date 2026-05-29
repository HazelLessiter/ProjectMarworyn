using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core.Managers
{
    internal interface IGenerationManager
    {
        Generation Initialise(List<Person> people);
        bool CheckForExtinction(List<Person> people);
    }
}