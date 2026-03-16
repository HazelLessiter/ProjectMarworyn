using ProjectMarworyn.Models;

namespace ProjectMarworyn
{
    internal interface IGenerationManager
    {
        Generation Initialise(List<Name> names);
        public Generation GenerateChildren(Generation generation);
    }
}