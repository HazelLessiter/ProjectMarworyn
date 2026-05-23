using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core
{
    public interface IAgeProcessor
    {
        List<Person> Age(List<Person> people);
    }
}