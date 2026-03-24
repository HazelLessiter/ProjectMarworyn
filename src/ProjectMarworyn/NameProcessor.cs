using ProjectMarworyn.Generators;
using ProjectMarworyn.Models;
using ProjectMarworyn.Models.Enums;
using ProjectMarworyn.Services;

namespace ProjectMarworyn
{
    internal class NameProcessor : INameProcessor
    {
        private readonly IConsoleService _consoleService;
        private readonly IDiceGenerator _diceGenerator;

        public NameProcessor(IConsoleService consoleService,
            IDiceGenerator diceGenerator)
        {
            _consoleService = consoleService;
            _diceGenerator = diceGenerator;
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

        public List<Pair> PairNames(List<Name> names,
            int worldSeed)
        {
            var fNames = GetNamesByGender(names,
                Gender.Female);
            var mNames = GetNamesByGender(names,
                Gender.Male);

            var pairs = new List<Pair>();

            var random = _diceGenerator.Create(worldSeed);
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
                        FPerson = fName,
                        MPerson = mName
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