using Microsoft.Extensions.Options;
using ProjectMarworyn.Core.Configuration;
using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core
{
    internal class AgeProcessor : IAgeProcessor
    {
        private GameState _gameState;
        private readonly AppSettings _appSettings;

        public AgeProcessor(GameState gameState,
            IOptions<AppSettings> appSettings)
        {
            _gameState = gameState;
            _appSettings = appSettings.Value;
        }

        public List<Person> Age(List<Person> people,
            DateTime currentTime)
        {
            var agedPeople = new List<Person>();

            foreach (var person in people)
            {
                if (!GetIfAlive(person))
                {
                    continue;
                }

                var age = GetAge(person,
                    currentTime);
                var daysSinceLastChild = GetDaysSinceLastChild(person);

                person.Age = age;
                person.DaysSinceLastChild = daysSinceLastChild;

                agedPeople.Add(person);
            }

            return agedPeople;
        }

        private bool GetIfAlive(Person person)
        {
            return person.IsAlive;
        }

        private int GetAge(Person person, DateTime currentTime)
        {
            var age = person.Age;

            if (IsBirthday(person,
                currentTime))
            {
                age += 1;
                _gameState.Text.Add($"{person.Name.FullName} is now {age} years old.");
            }

            return age;
        }

        private bool IsBirthday(Person person, DateTime currentTime)
        {
            if (currentTime.Month == person.BirthMonth &&
                currentTime.Day == person.BirthDay)
            {
                return true;
            }

            //People born on the 29th of February age up on the 1st of March in non-leap years
            return person.BirthMonth == 2 &&
                person.BirthDay == 29 &&
                !DateTime.IsLeapYear(currentTime.Year) &&
                currentTime.Month == 3 &&
                currentTime.Day == 1;
        }

        private int GetDaysSinceLastChild(Person person)
        {
            var daysSinceLastChild = person.DaysSinceLastChild;

            if (person.WillHaveChildren == true && daysSinceLastChild < _appSettings.FertilityCooldownYears * SimulationConstants.DaysPerYear)
            {
                daysSinceLastChild += 1;
            }

            return daysSinceLastChild;
        }
    }
}