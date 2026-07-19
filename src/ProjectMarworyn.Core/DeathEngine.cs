using Microsoft.Extensions.Options;
using ProjectMarworyn.Core.Configuration;
using ProjectMarworyn.Core.Generators;
using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core
{
    internal class DeathEngine : IDeathEngine
    {
        private readonly IDiceGenerator _diceGenerator;
        private GameState _gameState;
        private readonly AppSettings _appSettings;

        public DeathEngine(IDiceGenerator diceGenerator,
            GameState gameState,
            IOptions<AppSettings> appSettings)
        {
            _diceGenerator = diceGenerator;
            _gameState = gameState;
            _appSettings = appSettings.Value;

            //Fail at startup rather than mid-run: without a catch-all bracket, the first person
            //to outlive the last explicit MaxAge would crash the simulation with a cryptic error
            if (_appSettings.DeathBrackets == null ||
                !_appSettings.DeathBrackets.Any(x => x.MaxAge == null))
            {
                throw new InvalidOperationException("DeathBrackets must contain a catch-all bracket with no MaxAge");
            }
        }

        public Generation ProcessDeaths(List<Person> people,
            Generation generation,
            int worldSeed,
            DateTime currentTime)
        {
            var dice = _diceGenerator.Create(worldSeed,
                currentTime);

            var survivors = new List<Person>();

            foreach (var person in people.Where(x => x.IsAlive == true))
            {
                var deathBracket = _appSettings.DeathBrackets.First(x => x.MaxAge == null ||
                    person.Age <= x.MaxAge);

                if (_diceGenerator.NextDouble(dice) * 100 <= deathBracket.DailyDeathChance)
                {
                    person.IsAlive = false;
                    _gameState.Text.Add($"{person.Name.FullName} has died at age {person.Age}");
                }
                else
                {
                    person.IsAlive = true;
                    survivors.Add(person);
                }
            }

            var currentGeneration = new Generation()
            {
                Iteration = generation.Iteration,
                People = survivors,
            };

            return currentGeneration;
        }
    }
}