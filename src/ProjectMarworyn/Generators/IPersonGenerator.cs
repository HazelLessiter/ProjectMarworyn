using ProjectMarworyn.Models;

namespace ProjectMarworyn.Generators
{
    internal interface IPersonGenerator
    {
        List<Person> Initialise(List<Name> names);
    }
}