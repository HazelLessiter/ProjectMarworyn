using ProjectMarworyn.Models;

namespace ProjectMarworyn
{
    internal interface IGenerationManager
    {
        Generation Initialise(List<Name> names);
        bool CheckForExtinction(List<Person> people);
    }
}