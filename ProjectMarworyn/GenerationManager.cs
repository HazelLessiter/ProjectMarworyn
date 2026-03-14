using ProjectMarworyn.Models;

namespace ProjectMarworyn
{
    internal class GenerationManager : IGenerationManager
    {
        public Generation Initialise(List<Name> names)
        {
            return new Generation()
            {
                Iteration = 0,
                Names = names
            };
        }
    }
}