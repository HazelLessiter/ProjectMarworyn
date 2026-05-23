using ProjectMarworyn.Core;
using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Services;
using ProjectMarworyn.Tests.Mocks;

namespace ProjectMarworyn.Tests
{
    public class AgeProcessorTests
    {
        private readonly AgeProcessor _ageProcessor;
        private readonly IConsoleService _consoleService;

        public AgeProcessorTests()
        {
            _consoleService = new MockConsoleService();
            _ageProcessor = new AgeProcessor(_consoleService);
        }

        [Fact]
        public void Age_WithEmptyList_ReturnsEmptyList()
        {
            var people = new List<Person>();

            var result = _ageProcessor.Age(people);

            Assert.Empty(result);
        }

        [Fact]
        public void Age_WithEmptyList_ReturnsNonNullList()
        {
            var people = new List<Person>();

            var result = _ageProcessor.Age(people);

            Assert.NotNull(result);
        }

        [Fact]
        public void Age_WithAlivePerson_IncrementsTimeLivedByOneDay()
        {
            var person = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Test" },
                Age = 25,
                IsAlive = true,
                TimeLived = new DateTime(1, 1, 1).AddYears(25),
                TimeFromLastChild = new DateTime(1, 1, 1),
                WillHaveChildren = false
            };
            var originalTimeLived = person.TimeLived;
            var people = new List<Person> { person };

            var result = _ageProcessor.Age(people);

            Assert.Equal(originalTimeLived.AddDays(1), result[0].TimeLived);
        }

        [Fact]
        public void Age_WithPersonAtYearBoundary_IncrementsAge()
        {
            var person = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Test" },
                Age = 25,
                IsAlive = true,
                TimeLived = new DateTime(1, 12, 31),
                TimeFromLastChild = new DateTime(1, 1, 1),
                WillHaveChildren = false
            };
            var people = new List<Person> { person };

            var result = _ageProcessor.Age(people);

            Assert.Equal(26, result[0].Age);
        }

        [Fact]
        public void Age_WithPersonNotAtYearBoundary_DoesNotIncrementAge()
        {
            var person = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Test" },
                Age = 25,
                IsAlive = true,
                TimeLived = new DateTime(1, 6, 15),
                TimeFromLastChild = new DateTime(1, 1, 1),
                WillHaveChildren = false
            };
            var people = new List<Person> { person };

            var result = _ageProcessor.Age(people);

            Assert.Equal(25, result[0].Age);
        }

        [Fact]
        public void Age_WithDeadPerson_DoesNotIncludeInResult()
        {
            var person = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Test" },
                Age = 25,
                IsAlive = false,
                TimeLived = new DateTime(1, 1, 1).AddYears(25),
                TimeFromLastChild = new DateTime(1, 1, 1),
                WillHaveChildren = false
            };
            var people = new List<Person> { person };

            var result = _ageProcessor.Age(people);

            Assert.Empty(result);
        }

        [Fact]
        public void Age_WithMultiplePeople_OnlyIncludesAlive()
        {
            var people = new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Name = new Name
                    {
                        FullName = "Alive"
                    },
                    Age = 25, IsAlive = true, TimeLived = new DateTime(1, 6, 15),
                    TimeFromLastChild = new DateTime(1, 1, 1), WillHaveChildren = false
                },
                new Person
                {
                    Id = 2,
                    Name = new Name
                    {
                        FullName = "Dead"
                    },
                    Age = 30, IsAlive = false, TimeLived = new DateTime(1, 6, 15),
                    TimeFromLastChild = new DateTime(1, 1, 1), WillHaveChildren = false
                },
                new Person
                {
                    Id = 3,
                    Name = new Name
                    {
                        FullName = "Alive2"
                    },
                    Age = 28, IsAlive = true, TimeLived = new DateTime(1, 6, 15),
                    TimeFromLastChild = new DateTime(1, 1, 1), WillHaveChildren = false
                }
            };

            var result = _ageProcessor.Age(people);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void Age_WithPersonWantingChildrenAndTimeFromLastChildLessThan2_IncrementsTimeFromLastChild()
        {
            var person = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Test" },
                Age = 25,
                IsAlive = true,
                TimeLived = new DateTime(1, 6, 15),
                TimeFromLastChild = new DateTime(1, 1, 1),
                WillHaveChildren = true
            };
            var people = new List<Person> { person };

            var result = _ageProcessor.Age(people);

            Assert.Equal(new DateTime(1, 1, 2), result[0].TimeFromLastChild);
        }

        [Fact]
        public void Age_WithPersonWantingChildrenAndTimeFromLastChildEquals1_IncrementsTimeFromLastChild()
        {
            var person = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Test" },
                Age = 25,
                IsAlive = true,
                TimeLived = new DateTime(1, 6, 15),
                TimeFromLastChild = new DateTime(1, 1, 1),
                WillHaveChildren = true
            };
            var people = new List<Person> { person };

            var result = _ageProcessor.Age(people);

            Assert.Equal(new DateTime(1, 1, 2), result[0].TimeFromLastChild);
        }

        [Fact]
        public void Age_WithPersonWantingChildrenAndTimeFromLastChildEquals2_DoesNotIncrementTimeFromLastChild()
        {
            var person = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Test" },
                Age = 25,
                IsAlive = true,
                TimeLived = new DateTime(1, 6, 15),
                TimeFromLastChild = new DateTime(3, 1, 1),
                WillHaveChildren = true
            };
            var people = new List<Person> { person };

            var result = _ageProcessor.Age(people);

            Assert.Equal(new DateTime(3, 1, 1), result[0].TimeFromLastChild);
        }

        [Fact]
        public void Age_PersonWillHaveChildFalse_DoesNotIncrementTimeFromLastChild()
        {
            var person = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Test" },
                Age = 25,
                IsAlive = true,
                TimeLived = new DateTime(1, 6, 15),
                TimeFromLastChild = new DateTime(1, 1, 1),
                WillHaveChildren = false
            };
            var people = new List<Person> { person };

            var result = _ageProcessor.Age(people);

            Assert.Equal(new DateTime(1, 1, 1), result[0].TimeFromLastChild);
        }

        [Fact]
        public void Age_PreservesPersonId()
        {
            var person = new Person
            {
                Id = 42,
                Name = new Name { FullName = "Test" },
                Age = 25,
                IsAlive = true,
                TimeLived = new DateTime(1, 6, 15),
                TimeFromLastChild = new DateTime(1, 1, 1),
                WillHaveChildren = false
            };
            var people = new List<Person> { person };

            var result = _ageProcessor.Age(people);

            Assert.Equal(42, result[0].Id);
        }

        [Fact]
        public void Age_PreservesPersonName()
        {
            var name = new Name { FullName = "TestName" };
            var person = new Person
            {
                Id = 1,
                Name = name,
                Age = 25,
                IsAlive = true,
                TimeLived = new DateTime(1, 6, 15),
                TimeFromLastChild = new DateTime(1, 1, 1),
                WillHaveChildren = false
            };
            var people = new List<Person> { person };

            var result = _ageProcessor.Age(people);

            Assert.Equal(name, result[0].Name);
        }

        [Fact]
        public void Age_PreservesIsAliveStatus()
        {
            var person = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Test" },
                Age = 25,
                IsAlive = true,
                TimeLived = new DateTime(1, 6, 15),
                TimeFromLastChild = new DateTime(1, 1, 1),
                WillHaveChildren = false
            };
            var people = new List<Person> { person };

            var result = _ageProcessor.Age(people);

            Assert.True(result[0].IsAlive);
        }

        [Fact]
        public void Age_PreservesWillHaveChildren()
        {
            var person = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Test" },
                Age = 25,
                IsAlive = true,
                TimeLived = new DateTime(1, 6, 15),
                TimeFromLastChild = new DateTime(1, 1, 1),
                WillHaveChildren = true
            };
            var people = new List<Person> { person };

            var result = _ageProcessor.Age(people);

            Assert.True(result[0].WillHaveChildren);
        }

        [Fact]
        public void Age_WithMultiplePeople_ProcessesAll()
        {
            var people = new List<Person>
            {
                new Person
                {
                    Id = 1, Name = new Name { FullName = "P1" }, Age = 20, IsAlive = true, TimeLived = new DateTime(1, 6, 15), TimeFromLastChild = new DateTime(1, 1, 1), WillHaveChildren = false
                },
                new Person
                {
                    Id = 2, Name = new Name { FullName = "P2" }, Age = 30, IsAlive = true, TimeLived = new DateTime(1, 6, 15), TimeFromLastChild = new DateTime(1, 1, 1), WillHaveChildren = false
                },
                new Person
                {
                    Id = 3, Name = new Name { FullName = "P3" }, Age = 40, IsAlive = true, TimeLived = new DateTime(1, 6, 15), TimeFromLastChild = new DateTime(1, 1, 1), WillHaveChildren = false
                }
            };

            var result = _ageProcessor.Age(people);

            Assert.Equal(3, result.Count);
            Assert.All(result, p => Assert.Equal(new DateTime(1, 6, 16), p.TimeLived));
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(1, 6, 15)]
        [InlineData(1, 12, 30)]
        public void Age_WithDifferentDates_AddsOneDay(int year,
            int month,
            int day)
        {
            var timeLived = new DateTime(year,
                month,
                day);
            var person = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Test" },
                Age = 25,
                IsAlive = true,
                TimeLived = timeLived,
                TimeFromLastChild = new DateTime(1, 1, 1),
                WillHaveChildren = false
            };
            var people = new List<Person> { person };

            var result = _ageProcessor.Age(people);

            Assert.Equal(timeLived.AddDays(1), result[0].TimeLived);
        }
    }
}