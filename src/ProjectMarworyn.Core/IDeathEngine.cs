using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core
{
    public interface IDeathEngine
    {
        Generation ProcessDeaths(List<Person> people, Generation generation, int worldSeed);
    }
}