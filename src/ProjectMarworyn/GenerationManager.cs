using ProjectMarworyn.Models;

namespace ProjectMarworyn
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