using ProjectMarworyn.Generators;
using ProjectMarworyn.Models;
using ProjectMarworyn.Models.Enums;
using ProjectMarworyn.Services;

namespace ProjectMarworyn
{
    internal class DeathEngine : IDeathEngine
    {
        private readonly IConsoleService _consoleService;
        private readonly IDiceGenerator _diceGenerator;

        public DeathEngine(IConsoleService consoleService,
            IDiceGenerator diceGenerator)
        {
            _consoleService = consoleService;
            _diceGenerator = diceGenerator;
        }

        public Generation ProcessDeaths(List<Person> people,
            Generation generation,
            int worldSeed)
        {
            var deathChance = 0.0;
            var deathModifier = DeathModifier.Zero;
            var survivors = new List<Person>();
            var names = new List<Name>();
            bool death = false;
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

                if (random.NextDouble() * 100 <= deathChance)
                {
                    person.IsAlive = false;
                    death = true;
                    _consoleService.WriteLine($"{person.Name.FullName} has died at age {person.Age}");
                }
                else
                {
                    person.IsAlive = true;
                    survivors.Add(person);
                    names.Add(person.Name);
                    death = false;
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