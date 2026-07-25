using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core
{
    internal interface IAttractionCalculator
    {
        bool CanPair(Person person);
        bool AreMutuallyAttracted(Person personA, Person personB);
        bool IsAttractedTo(Person person, Person candidate);
    }
}