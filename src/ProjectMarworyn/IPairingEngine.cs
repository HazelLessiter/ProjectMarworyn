using ProjectMarworyn.Models;

namespace ProjectMarworyn
{
    internal interface IPairingEngine
    {
        (List<Pair>, List<Person>) GeneratePairs(List<Person> people, List<Pair> pairs, int worldSeed);
    }
}