using ProjectMarworyn.Models;
using ProjectMarworyn.Models.Enums;
using ProjectMarworyn.Services;

namespace ProjectMarworyn
{
    internal class DeathEngine : IDeathEngine
    {
        private readonly IConsoleService _consoleService;

        public DeathEngine(IConsoleService consoleService)
        {
            _consoleService = consoleService;
        }

        public (List<Person>, Generation) ProcessDeaths(List<Person> people,
            Generation generation)
        {
            int deathChance = 0;
            var deathModifier = DeathModifier.Zero;
            var surivors = new List<Person>();
            var names = new List<Name>();
            bool death = false;
            var random = new Random();//TODO: Use seed

            foreach (var person in people)
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
                    <= 99 => DeathModifier.Ninthy,
                    _ => DeathModifier.Hundred
                };

                deathChance = (int)deathModifier / 100;

                if (random.Next(0, 101) <= deathChance)
                {
                    person.IsAlive = false;
                    death = true;
                    _consoleService.WriteLine($"{person.Name} has died at age {person.Age}");
                }
                else
                {
                    person.IsAlive = true;
                    surivors.Add(person);
                    names.Add(person.Name);
                    death = false;
                }
            }

            var currentGeneration = new Generation()
            {
                Iteration = generation.Iteration,
                Names = names,
            };

            return (surivors, currentGeneration);//Refactor: Would rather not use tuples
        }
    }
}