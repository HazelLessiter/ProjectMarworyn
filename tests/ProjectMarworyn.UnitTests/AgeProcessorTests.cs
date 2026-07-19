using Microsoft.Extensions.Options;
using ProjectMarworyn.Core;
using ProjectMarworyn.Core.Configuration;
using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.UnitTests
{
    public class AgeProcessorTests
    {
        private readonly AgeProcessor _ageProcessor;

        public AgeProcessorTests()
        {
            _ageProcessor = new AgeProcessor(new GameState(),
                Options.Create(new AppSettings { FertilityCooldownYears = 2 }));
        }

        private static Person CreatePerson(int age = 25,
            bool isAlive = true,
            int birthMonth = 3,
            int birthDay = 10,
            int daysSinceLastChild = 0,
            bool willHaveChildren = false,
            int id = 1,
            Name name = null) =>
            new()
            {
                Id = id,
                Name = name ?? new Name { FullName = "Test" },
                Age = age,
                IsAlive = isAlive,
                BirthMonth = birthMonth,
                BirthDay = birthDay,
                DaysSinceLastChild = daysSinceLastChild,
                WillHaveChildren = willHaveChildren
            };

        [Fact]
        public void Age_WithEmptyList_ReturnsEmptyList()
        {
            var people = new List<Person>();

            var result = _ageProcessor.Age(people,
                new DateTime(5, 6, 15));

            Assert.Empty(result);
        }

        [Fact]
        public void Age_WithEmptyList_ReturnsNonNullList()
        {
            var people = new List<Person>();

            var result = _ageProcessor.Age(people,
                new DateTime(5, 6, 15));

            Assert.NotNull(result);
        }

        [Fact]
        public void Age_OnPersonsBirthday_IncrementsAge()
        {
            var people = new List<Person> { CreatePerson(age: 25, birthMonth: 3, birthDay: 10) };

            var result = _ageProcessor.Age(people,
                new DateTime(5, 3, 10));

            Assert.Equal(26, result[0].Age);
        }

        [Theory]
        [InlineData(5, 6, 15)]  // different month and day
        [InlineData(5, 3, 11)]  // right month, wrong day
        [InlineData(5, 4, 10)]  // right day, wrong month
        public void Age_NotOnPersonsBirthday_DoesNotIncrementAge(int year,
            int month,
            int day)
        {
            var people = new List<Person> { CreatePerson(age: 25, birthMonth: 3, birthDay: 10) };

            var result = _ageProcessor.Age(people,
                new DateTime(year, month, day));

            Assert.Equal(25, result[0].Age);
        }

        // People born on the 29th of February age up on the 29th in leap years
        // and on the 1st of March in non-leap years - never twice, never zero times.
        [Theory]
        [InlineData(4, 2, 29, 26)]  // leap year, actual birthday
        [InlineData(4, 3, 1, 25)]   // leap year, 1st of March is not their day
        [InlineData(5, 3, 1, 26)]   // non-leap year, ages up on the 1st of March
        [InlineData(5, 2, 28, 25)]  // non-leap year, 28th of February is not their day
        public void Age_PersonBornOnLeapDay_AgesUpExactlyOncePerYear(int year,
            int month,
            int day,
            int expectedAge)
        {
            var people = new List<Person> { CreatePerson(age: 25, birthMonth: 2, birthDay: 29) };

            var result = _ageProcessor.Age(people,
                new DateTime(year, month, day));

            Assert.Equal(expectedAge, result[0].Age);
        }

        [Fact]
        public void Age_WithDeadPerson_DoesNotIncludeInResult()
        {
            var people = new List<Person> { CreatePerson(isAlive: false) };

            var result = _ageProcessor.Age(people,
                new DateTime(5, 6, 15));

            Assert.Empty(result);
        }

        [Fact]
        public void Age_WithMultiplePeople_OnlyIncludesAlive()
        {
            var people = new List<Person>
            {
                CreatePerson(id: 1, isAlive: true),
                CreatePerson(id: 2, isAlive: false),
                CreatePerson(id: 3, isAlive: true)
            };

            var result = _ageProcessor.Age(people,
                new DateTime(5, 6, 15));

            Assert.Equal(2, result.Count);
        }

        // Cooldown threshold with FertilityCooldownYears = 2 is 730 days.
        [Theory]
        [InlineData(0)]
        [InlineData(729)]
        public void Age_WithPersonWantingChildrenBelowCooldown_IncrementsDaysSinceLastChild(int daysSinceLastChild)
        {
            var people = new List<Person>
            {
                CreatePerson(daysSinceLastChild: daysSinceLastChild, willHaveChildren: true)
            };

            var result = _ageProcessor.Age(people,
                new DateTime(5, 6, 15));

            Assert.Equal(daysSinceLastChild + 1, result[0].DaysSinceLastChild);
        }

        [Fact]
        public void Age_WithPersonWantingChildrenAtCooldown_DoesNotIncrementDaysSinceLastChild()
        {
            var people = new List<Person>
            {
                CreatePerson(daysSinceLastChild: 2 * 365, willHaveChildren: true)
            };

            var result = _ageProcessor.Age(people,
                new DateTime(5, 6, 15));

            Assert.Equal(2 * 365, result[0].DaysSinceLastChild);
        }

        [Fact]
        public void Age_WithCustomFertilityCooldownYears_RespectsConfiguredThreshold()
        {
            // Same 730-day input as the test above, but a higher configured threshold (5 years = 1825 days)
            // means it has NOT reached cooldown yet and should increment
            var ageProcessor = new AgeProcessor(new GameState(),
                Options.Create(new AppSettings { FertilityCooldownYears = 5 }));
            var people = new List<Person>
            {
                CreatePerson(daysSinceLastChild: 2 * 365, willHaveChildren: true)
            };

            var result = ageProcessor.Age(people,
                new DateTime(5, 6, 15));

            Assert.Equal(2 * 365 + 1, result[0].DaysSinceLastChild);
        }

        [Fact]
        public void Age_PersonWillHaveChildFalse_DoesNotIncrementDaysSinceLastChild()
        {
            var people = new List<Person>
            {
                CreatePerson(daysSinceLastChild: 0, willHaveChildren: false)
            };

            var result = _ageProcessor.Age(people,
                new DateTime(5, 6, 15));

            Assert.Equal(0, result[0].DaysSinceLastChild);
        }

        [Fact]
        public void Age_PreservesPersonId()
        {
            var people = new List<Person> { CreatePerson(id: 42) };

            var result = _ageProcessor.Age(people,
                new DateTime(5, 6, 15));

            Assert.Equal(42, result[0].Id);
        }

        [Fact]
        public void Age_PreservesPersonName()
        {
            var name = new Name { FullName = "TestName" };
            var people = new List<Person> { CreatePerson(name: name) };

            var result = _ageProcessor.Age(people,
                new DateTime(5, 6, 15));

            Assert.Equal(name, result[0].Name);
        }

        [Fact]
        public void Age_PreservesIsAliveStatus()
        {
            var people = new List<Person> { CreatePerson() };

            var result = _ageProcessor.Age(people,
                new DateTime(5, 6, 15));

            Assert.True(result[0].IsAlive);
        }

        [Fact]
        public void Age_PreservesWillHaveChildren()
        {
            var people = new List<Person> { CreatePerson(willHaveChildren: true) };

            var result = _ageProcessor.Age(people,
                new DateTime(5, 6, 15));

            Assert.True(result[0].WillHaveChildren);
        }

        [Fact]
        public void Age_WithMultiplePeople_ProcessesAll()
        {
            var people = new List<Person>
            {
                CreatePerson(id: 1, age: 20, birthMonth: 6, birthDay: 15),
                CreatePerson(id: 2, age: 30, birthMonth: 6, birthDay: 15),
                CreatePerson(id: 3, age: 40, birthMonth: 6, birthDay: 15)
            };

            var result = _ageProcessor.Age(people,
                new DateTime(5, 6, 15));

            Assert.Equal(3, result.Count);
            Assert.Equal(21, result[0].Age);
            Assert.Equal(31, result[1].Age);
            Assert.Equal(41, result[2].Age);
        }
    }
}
