using ProjectMarworyn.Models;

namespace ProjectMarworyn
{
    internal interface INameProcessor
    {
        public void ListNumberOfNamesByGender(List<Name> names);
        public void GenerateChildren(List<Name> names);
    }
}
