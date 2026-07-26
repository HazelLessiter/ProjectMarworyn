using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core.Generators
{
    internal interface IPersonGenerator
    {
        List<Person> Initialise(List<InitialPerson> initialPeople, int worldSeed);
        List<Person> GenerateChildren(List<Pair> pairs, int worldSeed, int personId, DateTime currentTime);
    }
}