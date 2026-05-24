using ProjectMarworyn.Core.Generators;
using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Models.Enums;
using ProjectMarworyn.Core.Services;

namespace ProjectMarworyn.Core
{
    internal class PairingEngine : IPairingEngine
    {
        private readonly IDiceGenerator _diceGenerator;
        private readonly IConsoleService _consoleService;

        public PairingEngine(IDiceGenerator diceGenerator,
            IConsoleService consoleService)
        {
            _diceGenerator = diceGenerator;
            _consoleService = consoleService;
        }

        public (List<Pair>, List<Person>) GeneratePairs(List<Person> people,
            List<Pair> pairs,
            int worldSeed)
        {
            var random = _diceGenerator.Create(worldSeed);

            //Note: I did some reading today regarding the perfomance of foreach, in some cases there can be a negative performance hit
            //I should take a closer look at O(n) vs O(1) to optimise my code
            var singleFemaleAdults = people.Where(x => x.Biosex == Biosex.Female &&
                x.Age >= 18 &&
                x.HasPair == false);

            foreach (var fPerson in singleFemaleAdults)
            {
                var singleMaleAdults = people.Where(x => x.Biosex == Biosex.Male &&
                        x.Age >= 18 &&
                        x.HasPair == false)
                    .ToList();
                var mCount = singleMaleAdults.Count();

                if (mCount <= 0)
                {
                    break;
                }

                var position = _diceGenerator.Next(random,
                    0,
                    mCount);

                var mPerson = singleMaleAdults[position];

                pairs.Add(new Pair()
                {
                    FPerson = fPerson,
                    MPerson = mPerson
                });

                _consoleService.WriteLine($"Pair: {fPerson.Name.FullName} + {mPerson.Name.FullName}",
                    ConsoleColor.Cyan);

                fPerson.HasPair = true;
                mPerson.HasPair = true;
            }

            var alivePairs = pairs;
            foreach (var pair in pairs)
            {
                if (pair.MPerson.IsAlive == false)
                {
                    alivePairs.Remove(pair);
                }
                if(pair.FPerson.IsAlive == false)
                {
                    alivePairs.Remove(pair);
                }
            }

            return (alivePairs,
                people);
        }
    }
}