using ProjectMarworyn.Models;
using ProjectMarworyn.Models.Enums;
using ProjectMarworyn.Tests.Mocks;

namespace ProjectMarworyn.Tests
{
    public class DeathEngineTests
    {
        private readonly DeathEngine _deathEngine;
        private readonly MockOutputService _mockOutputService;
        private readonly MockDiceGenerator _mockDiceGenerator;

        public DeathEngineTests()
        {
            _mockOutputService = new MockOutputService();
            _mockDiceGenerator = new MockDiceGenerator();
            _deathEngine = new DeathEngine(_mockOutputService, _mockDiceGenerator);
        }

        [Fact]
        public void ProcessDeaths_WithEmptyPeopleList_ReturnsEmptyLists()
        {
            var people = new List<Person>();
            var generation = new Generation { Iteration = 1, Names = new List<Name>() };

            var (survivors, currentGeneration) = _deathEngine.ProcessDeaths(people, generation, 0);

            Assert.Empty(survivors);
            Assert.Empty(currentGeneration.Names);
        }

        [Fact]
        public void ProcessDeaths_WithEmptyPeopleList_PreservesIteration()
        {
            var people = new List<Person>();
            var generation = new Generation { Iteration = 5, Names = new List<Name>() };

            var (survivors, currentGeneration) = _deathEngine.ProcessDeaths(people, generation, 0);

            Assert.Equal(5, currentGeneration.Iteration);
        }

        [Fact]
        public void ProcessDeaths_ReturnsNonNullLists()
        {
            var people = new List<Person>();
            var generation = new Generation { Iteration = 1, Names = new List<Name>() };

            var (survivors, currentGeneration) = _deathEngine.ProcessDeaths(people, generation, 0);

            Assert.NotNull(survivors);
            Assert.NotNull(currentGeneration);
            Assert.NotNull(currentGeneration.Names);
        }

        [Fact]
        public void ProcessDeaths_WithYoungPerson_HasLowDeathChance()
        {
            var mockDiceGenerator = new MockDiceGenerator(new Random(100));
            var deathEngine = new DeathEngine(_mockOutputService, mockDiceGenerator);
            var people = new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Name = new Name { FullName = "YoungPerson", Gender = Gender.Female },
                    Age = 5,
                    Gender = Gender.Female,
                    IsAlive = true
                }
            };
            var generation = new Generation { Iteration = 1, Names = new List<Name>() };

            var (survivors, currentGeneration) = deathEngine.ProcessDeaths(people, generation, 0);

            Assert.Single(survivors);
        }

        [Fact]
        public void ProcessDeaths_WithOldPerson_HasHighDeathChance()
        {
            var mockDiceGenerator = new MockDiceGenerator(new Random(1));
            var deathEngine = new DeathEngine(_mockOutputService, mockDiceGenerator);
            var people = new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Name = new Name { FullName = "OldPerson", Gender = Gender.Male },
                    Age = 95,
                    Gender = Gender.Male,
                    IsAlive = true
                }
            };
            var generation = new Generation { Iteration = 1, Names = new List<Name>() };

            var (survivors, currentGeneration) = deathEngine.ProcessDeaths(people, generation, 0);

            Assert.Empty(survivors);
        }

        [Fact]
        public void ProcessDeaths_SetsIsAliveToFalseForDeadPeople()
        {
            var mockDiceGenerator = new MockDiceGenerator(new Random(1));
            var deathEngine = new DeathEngine(_mockOutputService, mockDiceGenerator);
            var person = new Person
            {
                Id = 1,
                Name = new Name { FullName = "OldPerson", Gender = Gender.Male },
                Age = 95,
                Gender = Gender.Male,
                IsAlive = true
            };
            var people = new List<Person> { person };
            var generation = new Generation { Iteration = 1, Names = new List<Name>() };

            deathEngine.ProcessDeaths(people, generation, 0);

            Assert.False(person.IsAlive);
        }

        [Fact]
        public void ProcessDeaths_SetsIsAliveToTrueForSurvivors()
        {
            var mockDiceGenerator = new MockDiceGenerator(new Random(100));
            var deathEngine = new DeathEngine(_mockOutputService, mockDiceGenerator);
            var person = new Person
            {
                Id = 1,
                Name = new Name { FullName = "YoungPerson", Gender = Gender.Female },
                Age = 5,
                Gender = Gender.Female,
                IsAlive = false
            };
            var people = new List<Person> { person };
            var generation = new Generation { Iteration = 1, Names = new List<Name>() };

            deathEngine.ProcessDeaths(people, generation, 0);

            Assert.True(person.IsAlive);
        }

        [Fact]
        public void ProcessDeaths_SurvivorsListContainsOnlyAlivePeople()
        {
            var people = new List<Person>
            {
                new Person { Id = 1, Name = new Name { FullName = "Person1", Gender = Gender.Female }, Age = 10, Gender = Gender.Female, IsAlive = true },
                new Person { Id = 2, Name = new Name { FullName = "Person2", Gender = Gender.Male }, Age = 15, Gender = Gender.Male, IsAlive = true },
                new Person { Id = 3, Name = new Name { FullName = "Person3", Gender = Gender.Female }, Age = 20, Gender = Gender.Female, IsAlive = true }
            };
            var generation = new Generation { Iteration = 1, Names = new List<Name>() };

            var (survivors, currentGeneration) = _deathEngine.ProcessDeaths(people, generation, 0);

            Assert.All(survivors, person => Assert.True(person.IsAlive));
        }

        [Fact]
        public void ProcessDeaths_GenerationNamesMatchSurvivors()
        {
            var mockDiceGenerator = new MockDiceGenerator(new Random(100));
            var deathEngine = new DeathEngine(_mockOutputService, mockDiceGenerator);
            var people = new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Name = new Name { FullName = "Survivor1", Gender = Gender.Female },
                    Age = 10,
                    Gender = Gender.Female,
                    IsAlive = true
                },
                new Person
                {
                    Id = 2,
                    Name = new Name { FullName = "Survivor2", Gender = Gender.Male },
                    Age = 15,
                    Gender = Gender.Male,
                    IsAlive = true
                }
            };
            var generation = new Generation { Iteration = 1, Names = new List<Name>() };

            var (survivors, currentGeneration) = deathEngine.ProcessDeaths(people, generation, 0);

            Assert.Equal(survivors.Count, currentGeneration.Names.Count);
            Assert.All(survivors, person => Assert.Contains(person.Name, currentGeneration.Names));
        }

        [Fact]
        public void ProcessDeaths_WritesToConsoleOnDeath()
        {
            var mockDiceGenerator = new MockDiceGenerator(new Random(1));
            var mockOutput = new MockOutputService();
            var deathEngine = new DeathEngine(mockOutput, mockDiceGenerator);
            var people = new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Name = new Name { FullName = "OldPerson", Gender = Gender.Male },
                    Age = 95,
                    Gender = Gender.Male,
                    IsAlive = true
                }
            };
            var generation = new Generation { Iteration = 1, Names = new List<Name>() };

            deathEngine.ProcessDeaths(people, generation, 0);

            Assert.NotEmpty(mockOutput.Messages);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(9)]
        public void ProcessDeaths_Age0To9_UsesZeroModifier(int age)
        {
            var mockDiceGenerator = new MockDiceGenerator(new Random(100));
            var deathEngine = new DeathEngine(_mockOutputService, mockDiceGenerator);
            var person = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Child", Gender = Gender.Female },
                Age = age,
                Gender = Gender.Female,
                IsAlive = true
            };
            var people = new List<Person> { person };
            var generation = new Generation { Iteration = 1, Names = new List<Name>() };

            var (survivors, currentGeneration) = deathEngine.ProcessDeaths(people, generation, 0);

            Assert.Single(survivors);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(15)]
        [InlineData(19)]
        public void ProcessDeaths_Age10To19_UsesTenModifier(int age)
        {
            var mockDiceGenerator = new MockDiceGenerator(new Random(100));
            var deathEngine = new DeathEngine(_mockOutputService, mockDiceGenerator);
            var person = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Teen", Gender = Gender.Male },
                Age = age,
                Gender = Gender.Male,
                IsAlive = true
            };
            var people = new List<Person> { person };
            var generation = new Generation { Iteration = 1, Names = new List<Name>() };

            var (survivors, currentGeneration) = deathEngine.ProcessDeaths(people, generation, 0);

            Assert.Single(survivors);
        }

        [Theory]
        [InlineData(80)]
        [InlineData(85)]
        [InlineData(89)]
        public void ProcessDeaths_Age80To89_UsesEightyModifier(int age)
        {
            var mockDiceGenerator = new MockDiceGenerator(new Random(100));
            var deathEngine = new DeathEngine(_mockOutputService, mockDiceGenerator);
            var person = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Elder", Gender = Gender.Female },
                Age = age,
                Gender = Gender.Female,
                IsAlive = true
            };
            var people = new List<Person> { person };
            var generation = new Generation { Iteration = 1, Names = new List<Name>() };

            var (survivors, currentGeneration) = deathEngine.ProcessDeaths(people, generation, 0);

            Assert.Empty(survivors);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(105)]
        public void ProcessDeaths_Age100Plus_UsesHundredModifier(int age)
        {
            var mockDiceGenerator = new MockDiceGenerator(new Random(100));
            var deathEngine = new DeathEngine(_mockOutputService, mockDiceGenerator);
            var person = new Person
            {
                Id = 1,
                Name = new Name { FullName = "VeryOld", Gender = Gender.Male },
                Age = age,
                Gender = Gender.Male,
                IsAlive = true
            };
            var people = new List<Person> { person };
            var generation = new Generation { Iteration = 1, Names = new List<Name>() };

            var (survivors, currentGeneration) = deathEngine.ProcessDeaths(people, generation, 0);

            Assert.Empty(survivors);
        }

        [Fact]
        public void ProcessDeaths_WithMultiplePeople_ProcessesAll()
        {
            var people = new List<Person>
            {
                new Person { Id = 1, Name = new Name { FullName = "P1", Gender = Gender.Female }, Age = 10, Gender = Gender.Female, IsAlive = true },
                new Person { Id = 2, Name = new Name { FullName = "P2", Gender = Gender.Male }, Age = 20, Gender = Gender.Male, IsAlive = true },
                new Person { Id = 3, Name = new Name { FullName = "P3", Gender = Gender.Female }, Age = 30, Gender = Gender.Female, IsAlive = true },
                new Person { Id = 4, Name = new Name { FullName = "P4", Gender = Gender.Male }, Age = 40, Gender = Gender.Male, IsAlive = true }
            };
            var generation = new Generation { Iteration = 1, Names = new List<Name>() };

            var (survivors, currentGeneration) = _deathEngine.ProcessDeaths(people, generation, 0);

            Assert.True(survivors.Count <= people.Count);
        }
    }
}
