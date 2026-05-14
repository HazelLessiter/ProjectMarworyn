using ProjectMarworyn.Models;

namespace ProjectMarworyn
{
    internal interface IDeathEngine
    {
        Generation ProcessDeaths(List<Person> people, Generation generation, int worldSeed);
    }
}