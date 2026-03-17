using ProjectMarworyn.Models;

namespace ProjectMarworyn
{
    internal interface INameProcessor
    {
        public void ListNumberOfNamesByGender(List<Name> names);
        public List<Pair> PairNames(List<Name> names, int worldSeed);
    }
}