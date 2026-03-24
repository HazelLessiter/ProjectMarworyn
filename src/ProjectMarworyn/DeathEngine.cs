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
                switch(age)
                {
                    case (age <= 9)
                        deathModifier = DeathModifier.Zero;
                        break;
                    case (age <= 19)
                        deathModifier = DeathModifier.Ten;
                        break;
                    case (age <= 29)
                        deathModifier = DeathModifier.Twenty;
                        break;
                    case (age <= 39)
                        deathModifier = DeathModifier.Thirty;
                        break;
                    case (age <= 49)
                        deathModifier = DeathModifier.Fourty;
                        break;
                    case (age <= 59)
                        deathModifier = DeathModifier.Fifty;
                        break;
                    case (age <= 69)
                        deathModifier = DeathModifier.Sixty;
                        break;
                    case (age <= 79)
                        deathModifier = DeathModifier.Seventy;
                        break;
                    case (age <= 89)
                        deathModifier = DeathModifier.Eighty;
                        break;
                    case (age <= 99)
                        deathModifier = DeathModifier.Ninthy;
                        break;
                    case (age > 99)
                        deathModifier = DeathModifier.Hundred;
                        break;
                }

                deathChance = deathModifier / 100;

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