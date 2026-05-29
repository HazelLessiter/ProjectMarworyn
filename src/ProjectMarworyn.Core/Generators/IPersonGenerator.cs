using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core.Generators
{
    internal interface IPersonGenerator
    {
        List<Person> Initialise(List<InitialPerson> initialPeople, int worldSeed);
        (List<Person>, List<Person>) GenerateChildren(List<Pair> pairs, int worldSeed, int personId, List<Person> people);
    }
}