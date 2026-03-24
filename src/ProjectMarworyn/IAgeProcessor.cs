using ProjectMarworyn.Models;

namespace ProjectMarworyn
{
    internal interface IAgeProcessor
    {
        List<Person> Age(List<Person> people);
    }
}