using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core.Managers
{
    internal interface IGenerationManager
    {
        bool CheckForExtinction(List<Person> people);
    }
}