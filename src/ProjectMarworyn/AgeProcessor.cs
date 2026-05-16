using ProjectMarworyn.Models;

namespace ProjectMarworyn
{
    internal class AgeProcessor : IAgeProcessor
    {
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

                var agedPerson = new Person
                {
                    Id = person.Id,
                    Name = person.Name,
                    Age = age,
                    Biosex = person.Biosex,
                    IsAlive = person.IsAlive,
                    TimeFromLastChild = timeFromLastChild,
                    TimeLived = timeLived,
                    WillHaveChildren = person.WillHaveChildren,
                    HasPair = person.HasPair
                };

                agedPeople.Add(agedPerson);
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
            }

            return age;
        }

        private int GetTimeFromLastChild(Person person)
        {
            var timeFromLastChild = person.TimeFromLastChild;

            if (person.WillHaveChildren == true &&
                timeFromLastChild < 2)
            {
                timeFromLastChild += 1;
            }

            return timeFromLastChild;
        }
    }
}