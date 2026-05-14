using ProjectMarworyn.Models;

namespace ProjectMarworyn
{
    internal interface IGenerationManager
    {
        Generation Initialise(List<Person> people);
        bool CheckForExtinction(List<Person> people);
    }
}