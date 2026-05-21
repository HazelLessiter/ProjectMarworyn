using ProjectMarworyn.Models;

namespace ProjectMarworyn.Generators
{
    internal interface IPersonGenerator
    {
        List<Person> Initialise(List<InitialPerson> initialPeople, int worldSeed);
        (List<Person>, List<Person>) GenerateChildren(List<Pair> pairs, int worldSeed, int personId, List<Person> people);
    }
}