using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core
{
    internal class GenerationManager : IGenerationManager
    {
        public Generation Initialise(List<Person> people)
        {
            return new Generation()
            {
                Iteration = 0,
                People = people
            };
        }

        public bool CheckForExtinction(List<Person> people)
        {
            return people == null ||
                people.Count < 2;
        }
    }
}