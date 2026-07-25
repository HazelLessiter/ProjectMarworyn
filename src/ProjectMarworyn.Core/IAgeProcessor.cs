using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core
{
    internal interface IAgeProcessor
    {
        List<Person> Age(List<Person> people, DateTime currentTime);
    }
}