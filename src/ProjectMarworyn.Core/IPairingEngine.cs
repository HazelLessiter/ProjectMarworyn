using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core
{
    public interface IPairingEngine
    {
        (List<Pair>, List<Person>) GeneratePairs(List<Person> people, List<Pair> pairs, int worldSeed);
    }
}