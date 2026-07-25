using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core.Managers
{
    internal class GenerationManager : IGenerationManager
    {
        public bool CheckForExtinction(List<Person> people)
        {
            return people == null ||
                people.Count < 2;
        }
    }
}