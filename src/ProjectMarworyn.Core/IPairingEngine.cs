using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core
{
    internal interface IPairingEngine
    {
        (List<Pair>, List<Person>) GeneratePairs(List<Person> people, List<Pair> pairs, int worldSeed, Random dice);
    }
}