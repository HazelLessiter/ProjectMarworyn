using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core
{
    internal interface IDeathEngine
    {
        Generation ProcessDeaths(List<Person> people, Generation generation, int worldSeed, Random dice);
    }
}