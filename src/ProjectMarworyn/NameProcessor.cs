using ProjectMarworyn.Models;

namespace ProjectMarworyn
{
    internal class NameProcessor : INameProcessor
    {
        private readonly IConsoleService _consoleService;

        public NameProcessor(IConsoleService consoleService)
        {
            _consoleService = consoleService;
        }

        public void ListNumberOfNamesByGender(List<Name> names)
        {
            var fNames = names.Where(x => x.Gender == Gender.Female)
                .Count();
            var mNames = names.Where(x => x.Gender == Gender.Male)
                .Count();

            _consoleService.WriteLine($"Number of female names: {fNames}, Number of male names: {mNames}");
            _consoleService.Delay();
        }

        public List<Pair> PairNames(List<Name> names)
        {
            var fNames = GetNamesByGender(names,
                Gender.Female);
            var mNames = GetNamesByGender(names,
                Gender.Male);

            var pairs = new List<Pair>();

            var random = new Random();
            foreach (var fName in fNames)
            {
                var mNameCount = mNames.Count;
                if (mNameCount <= 0)
                {
                    break;
                }

                var position = random.Next(0,
                    mNameCount);

                var mName = mNames[position];

                if (mName != null)
                {
                    pairs.Add(new Pair
                    {
                        FName = fName,
                        MName = mName
                    });

                    mNames.RemoveAt(position);

                    _consoleService.WriteLine($"Pair: {fName.FullName} + {mName.FullName}");
                    _consoleService.Delay();
                }
            }

            return pairs;
        }

        private List<Name> GetNamesByGender(List<Name> names,
            Gender gender)
        {
            var namesByGender = names.Where(x => x.Gender == gender)
                .ToList();

            return namesByGender;
        }
    }
}