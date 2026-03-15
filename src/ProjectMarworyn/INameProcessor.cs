using ProjectMarworyn.Models;

namespace ProjectMarworyn
{
    internal interface INameProcessor
    {
        public void ListNumberOfNamesByGender(List<Name> names);
        public Generation GenerateChildren(Generation generation);
    }
}