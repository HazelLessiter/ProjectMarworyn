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

        public PairingResult GeneratePairs(List<Person> people,
            List<Pair> pairs,
            int worldSeed,
            DateTime currentTime)
        {
            var dice = _diceGenerator.Create(worldSeed,
                currentTime);

            //Deliberately not materialised: the deferred query re-evaluates HasPair as iteration
            //advances, so anyone paired mid-loop is skipped automatically. A .ToList() snapshot
            //would re-visit them - irrelevant while partners only come from the male pool, but a
            //trap once pairing can touch upcoming females (e.g. same-sex pairing, issue #15 stage 4)
            var singleFemaleAdults = people.Where(x => x.Biosex == Biosex.Female &&
                x.Age >= 18 &&
                x.HasPair == false);

            //Filtered once up-front and reduced via RemoveAt below, rather than re-filtering `people` on every female -
            //RemoveAt keeps the same relative order a fresh filter would produce, so pairing outcomes for a given world seed are unchanged
            var singleMaleAdults = people.Where(x => x.Biosex == Biosex.Male &&
                    x.Age >= 18 &&
                    x.HasPair == false)
                .ToList();

            foreach (var fPerson in singleFemaleAdults)
            {
                var mCount = singleMaleAdults.Count;

                if (mCount <= 0)
                {
                    break;
                }

                var position = _diceGenerator.Next(dice,
                    0,
                    mCount);

                var mPerson = singleMaleAdults[position];
                singleMaleAdults.RemoveAt(position);

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

            var alivePairs = pairs.Where(x => x.FPerson.IsAlive &&
                    x.MPerson.IsAlive)
                .ToList();

            return new PairingResult
            {
                Pairs = alivePairs,
                People = people
            };
        }
    }
}