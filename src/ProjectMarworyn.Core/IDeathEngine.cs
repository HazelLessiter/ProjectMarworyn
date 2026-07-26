using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core
{
    internal interface IDeathEngine
    {
        List<Person> ProcessDeaths(List<Person> people, int worldSeed, DateTime currentTime);
    }
}