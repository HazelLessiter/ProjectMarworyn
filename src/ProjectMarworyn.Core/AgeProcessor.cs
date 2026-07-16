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
                _gameState.Text.Add($"{person.Name.FullName} is now {age} years old.");
            }

            return age;
        }

        private DateTime GetTimeFromLastChild(Person person)
        {
            var timeFromLastChild = person.TimeFromLastChild;

            if (person.WillHaveChildren == true && timeFromLastChild.Year < _appSettings.FertilityCooldownYears)
            {
                timeFromLastChild = timeFromLastChild.AddDays(1);
            }

            return timeFromLastChild;
        }
    }
}