using ProjectMarworyn.Core.Generators;
using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Models.Enums;

namespace ProjectMarworyn.Core
{
    internal class DeathEngine : IDeathEngine
    {
        private readonly IDiceGenerator _diceGenerator;
        private GameState _gameState;

        public DeathEngine(IDiceGenerator diceGenerator,
            GameState gameState)
        {
            _diceGenerator = diceGenerator;
            _gameState = gameState;
        }

        public Generation ProcessDeaths(List<Person> people,
            Generation generation,
            int worldSeed)
        {
            var deathChance = 0.0;
            var deathModifier = DeathModifier.Zero;
            var survivors = new List<Person>();
            var names = new List<Name>();
            var random = _diceGenerator.Create(worldSeed);

            foreach (var person in people.Where(x => x.IsAlive == true))
            {
                var age = person.Age;

                //Note: Switch expressions were introduced in C# 8
                //I should attempt to use some of the newer features in C# post .NET 5
                deathModifier = age switch
                {
                    <= 9 => DeathModifier.Zero,
                    <= 19 => DeathModifier.Ten,
                    <= 29 => DeathModifier.Twenty,
                    <= 39 => DeathModifier.Thirty,
                    <= 49 => DeathModifier.Fourty,
                    <= 59 => DeathModifier.Fifty,
                    <= 69 => DeathModifier.Sixty,
                    <= 79 => DeathModifier.Seventy,
                    <= 89 => DeathModifier.Eighty,
                    <= 99 => DeathModifier.Ninety,
                    _ => DeathModifier.Hundred
                };

                deathChance = (int)deathModifier / 100.0;

                if (_diceGenerator.NextDouble(random) * 100 <= deathChance)
                {
                    person.IsAlive = false;
                    _gameState.Text.Add($"{person.Name.FullName} has died at age {person.Age}");
                }
                else
                {
                    person.IsAlive = true;
                    survivors.Add(person);
                    names.Add(person.Name);
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