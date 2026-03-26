using ProjectMarworyn.Models;

namespace ProjectMarworyn.Generators
{
    internal interface IPersonGenerator
    {
        List<Person> Initialise(List<Name> names, int worldSeed);
        (List<Person>, List<Person>) GenerateChildren(List<Pair> pairs, int worldSeed, int personId, List<Person> people);
    }
}