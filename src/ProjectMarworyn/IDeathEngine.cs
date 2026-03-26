using ProjectMarworyn.Models;

namespace ProjectMarworyn
{
    internal interface IDeathEngine
    {
        (List<Person>, Generation) ProcessDeaths(List<Person> people, Generation generation, int worldSeed);
    }
}