using ProjectMarworyn.Models;
using ProjectMarworyn.Services;

namespace ProjectMarworyn
{
    internal class AgeProcessor : IAgeProcessor
    {
        private readonly IConsoleService _consoleService;

        public AgeProcessor(IConsoleService consoleService)
        {
            _consoleService = consoleService;
        }

        public List<Person> Age(List<Person> people)
        {
            var agedPeople = new List<Person>();

            foreach (var person in people)
            {
                if (!GetIfALive(person))
                {
                    continue;
                }

                var timeLived = GetTimeLived(person);

                var age = GetAge(person,
                    timeLived);
                var timeFromLastChild = GetTimeFromLastChild(person);

                person.Age = age;
                person.TimeFromLastChild = timeFromLastChild;
                person.TimeLived = timeLived;

                agedPeople.Add(person);
            }

            return agedPeople;
        }

        private bool GetIfALive(Person person)
        {
            return person.IsAlive;
        }

        private DateTime GetTimeLived(Person person)
        {
            return person.TimeLived.AddDays(1);
        }

        private int GetAge(Person person, DateTime timeLived)
        {
            var age = person.Age;

            if (timeLived.Year > person.TimeLived.Year)
            {
                age += 1;
                Console.ForegroundColor = ConsoleColor.White;
                _consoleService.WriteLine($"{person.Name.FullName} is now {age} years old.");
            }

            return age;
        }

        private DateTime GetTimeFromLastChild(Person person)
        {
            var timeFromLastChild = person.TimeFromLastChild;

            if (person.WillHaveChildren == true)
            {
                timeFromLastChild = timeFromLastChild.AddDays(1);
            }

            return timeFromLastChild;
        }
    }
}