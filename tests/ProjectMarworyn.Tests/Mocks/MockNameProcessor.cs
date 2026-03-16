using ProjectMarworyn.Models;

namespace ProjectMarworyn.Tests.Mocks
{
    internal class MockNameProcessor : INameProcessor
    {
        public List<Pair> PairsToReturn { get; set; } = new List<Pair>();

        public void ListNumberOfNamesByGender(List<Name> names)
        {
        }

        public List<Pair> PairNames(List<Name> names)
        {
            return PairsToReturn;
        }
    }
}