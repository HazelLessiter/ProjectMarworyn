using ProjectMarworyn.Core.Generators;
using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core
{
    internal class PairingEngine : IPairingEngine
    {
        private readonly IDiceGenerator _diceGenerator;
        private readonly IAttractionCalculator _attractionCalculator;
        private GameState _gameState;

        public PairingEngine(IDiceGenerator diceGenerator,
            IAttractionCalculator attractionCalculator,
            GameState gameState)
        {
            _diceGenerator = diceGenerator;
            _attractionCalculator = attractionCalculator;
            _gameState = gameState;
        }

        public PairingResult GeneratePairs(List<Person> people,
            List<Pair> pairs,
            int worldSeed,
            DateTime currentTime)
        {
            var dice = _diceGenerator.Create(worldSeed,
                currentTime);

            //Snapshot deliberately: pairing can claim anyone later in the list, so each
            //iteration re-checks HasPair instead of relying on a deferred query
            var singleAdults = people.Where(x => x.Age >= 18 &&
                    x.HasPair == false &&
                    x.WillPair &&
                    _attractionCalculator.CanPair(x))
                .ToList();

            foreach (var person in singleAdults)
            {
                if (person.HasPair)
                {
                    continue;
                }

                var candidates = singleAdults.Where(x => x.Id != person.Id &&
                        x.HasPair == false &&
                        _attractionCalculator.AreMutuallyAttracted(person,
                            x))
                    .ToList();

                if (candidates.Count == 0)
                {
                    continue;
                }

                var position = _diceGenerator.Next(dice,
                    0,
                    candidates.Count);

                var partner = candidates[position];

                var pair = new Pair()
                {
                    PersonA = person,
                    PersonB = partner
                };
                pairs.Add(pair);

                _gameState.Text.Add($"Pair: {person.Name.FullName} + {partner.Name.FullName}");

                person.HasPair = true;
                partner.HasPair = true;
            }

            var alivePairs = pairs.Where(x => x.PersonA.IsAlive &&
                    x.PersonB.IsAlive)
                .ToList();

            return new PairingResult
            {
                Pairs = alivePairs,
                People = people
            };
        }
    }
}