using ProjectMarworyn.Core.Generators;
using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Models.Enums;

namespace ProjectMarworyn.Core
{
    internal class PairingEngine : IPairingEngine
    {
        private readonly IDiceGenerator _diceGenerator;
        private GameState _gameState;

        public PairingEngine(IDiceGenerator diceGenerator,
            GameState gameState)
        {
            _diceGenerator = diceGenerator;
            _gameState = gameState;
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

                var pair = new Pair()
                {
                    FPerson = fPerson,
                    MPerson = mPerson
                };
                pairs.Add(pair);

                _gameState.Text.Add($"Pair: {fPerson.Name.FullName} + {mPerson.Name.FullName}");

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