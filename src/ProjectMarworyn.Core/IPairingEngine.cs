using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core
{
    internal interface IPairingEngine
    {
        List<Pair> GeneratePairs(List<Person> people, List<Pair> pairs, int worldSeed, DateTime currentTime);
    }
}