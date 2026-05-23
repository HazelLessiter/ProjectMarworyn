using ProjectMarworyn.Core;
using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Models.Enums;

namespace ProjectMarworyn.Tests
{
    public class GenerationManagerTests
    {
        private readonly GenerationManager _generationManager;

        public GenerationManagerTests()
        {
            _generationManager = new GenerationManager();
        }

        [Fact]
        public void Initialise_WithEmptyList_ReturnsGenerationWithNoPeople()
        {
            var people = new List<Person>();

            var result = _generationManager.Initialise(people);

            Assert.Empty(result.People);
        }

        [Fact]
        public void Initialise_WithEmptyList_ReturnsIterationZero()
        {
            var people = new List<Person>();

            var result = _generationManager.Initialise(people);

            Assert.Equal(0, result.Iteration);
        }

        [Fact]
        public void Initialise_WithPeople_ReturnsGenerationWithSamePeople()
        {
            var people = new List<Person>
            {
                new Person { Id = 0, Name = new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe" }, Age = 25, Biosex = Biosex.Female, IsAlive = true },
                new Person { Id = 1, Name = new Name { FullName = "JohnSmith", Prefix = "John", Suffix = "Smith" }, Age = 30, Biosex = Biosex.Male, IsAlive = true }
            };

            var result = _generationManager.Initialise(people);

            Assert.Equal(2, result.People.Count);
            Assert.Equal(people, result.People);
        }

        [Fact]
        public void Initialise_WithPeople_ReturnsIterationZero()
        {
            var people = new List<Person>
            {
                new Person { Id = 0, Name = new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe" }, Age = 25, Biosex = Biosex.Female, IsAlive = true }
            };

            var result = _generationManager.Initialise(people);

            Assert.Equal(0, result.Iteration);
        }

        [Fact]
        public void Initialise_WithSinglePerson_ReturnsGenerationWithSinglePerson()
        {
            var people = new List<Person>
            {
                new Person { Id = 0, Name = new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe" }, Age = 25, Biosex = Biosex.Female, IsAlive = true }
            };

            var result = _generationManager.Initialise(people);

            Assert.Single(result.People);
            Assert.Equal("JaneDoe", result.People[0].Name.FullName);
        }

        [Fact]
        public void Initialise_WithMultiplePeople_PreservesPersonProperties()
        {
            var people = new List<Person>
            {
                new Person { Id = 0, Name = new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe" }, Age = 25, Biosex = Biosex.Female, IsAlive = true },
                new Person { Id = 1, Name = new Name { FullName = "JohnSmith", Prefix = "John", Suffix = "Smith" }, Age = 30, Biosex = Biosex.Male, IsAlive = true },
                new Person { Id = 2, Name = new Name { FullName = "AliceWonder", Prefix = "Alice", Suffix = "Wonder" }, Age = 28, Biosex = Biosex.Female, IsAlive = true }
            };

            var result = _generationManager.Initialise(people);

            Assert.Equal(3, result.People.Count);
            Assert.Equal("Jane", result.People[0].Name.Prefix);
            Assert.Equal("Doe", result.People[0].Name.Suffix);
            Assert.Equal("John", result.People[1].Name.Prefix);
            Assert.Equal("Smith", result.People[1].Name.Suffix);
        }

        [Fact]
        public void Initialise_ReturnsNonNullGeneration()
        {
            var people = new List<Person>();

            var result = _generationManager.Initialise(people);

            Assert.NotNull(result);
        }

        [Fact]
        public void Initialise_ReturnsNonNullPeopleList()
        {
            var people = new List<Person>();

            var result = _generationManager.Initialise(people);

            Assert.NotNull(result.People);
        }

        [Fact]
        public void Initialise_WithLargePersonList_ReturnsAllPeople()
        {
            var people = new List<Person>();
            for (int i = 0; i < 100; i++)
            {
                people.Add(new Person
                {
                    Id = i,
                    Name = new Name { FullName = $"Name{i}", Prefix = $"Prefix{i}", Suffix = $"Suffix{i}" },
                    Age = 25,
                    Biosex = Biosex.Female,
                    IsAlive = true
                });
            }

            var result = _generationManager.Initialise(people);

            Assert.Equal(100, result.People.Count);
        }

        [Fact]
        public void CheckForExtinction_WithNullPeople_ReturnsTrue()
        {
            List<Person> people = null;

            var result = _generationManager.CheckForExtinction(people);

            Assert.True(result);
        }

        [Fact]
        public void CheckForExtinction_WithEmptyList_ReturnsTrue()
        {
            var people = new List<Person>();

            var result = _generationManager.CheckForExtinction(people);

            Assert.True(result);
        }

        [Fact]
        public void CheckForExtinction_WithOnePerson_ReturnsTrue()
        {
            var people = new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Name = new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe" },
                    Age = 25,
                    Biosex = Biosex.Female,
                    IsAlive = true
                }
            };

            var result = _generationManager.CheckForExtinction(people);

            Assert.True(result);
        }

        [Fact]
        public void CheckForExtinction_WithTwoPeople_ReturnsFalse()
        {
            var people = new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Name = new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe" },
                    Age = 25,
                    Biosex = Biosex.Female,
                    IsAlive = true
                },
                new Person
                {
                    Id = 2,
                    Name = new Name { FullName = "JohnSmith", Prefix = "John", Suffix = "Smith" },
                    Age = 30,
                    Biosex = Biosex.Male,
                    IsAlive = true
                }
            };

            var result = _generationManager.CheckForExtinction(people);

            Assert.False(result);
        }

        [Fact]
        public void CheckForExtinction_WithMultiplePeople_ReturnsFalse()
        {
            var people = new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Name = new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe" },
                    Age = 25,
                    Biosex = Biosex.Female,
                    IsAlive = true
                },
                new Person
                {
                    Id = 2,
                    Name = new Name { FullName = "JohnSmith", Prefix = "John", Suffix = "Smith" },
                    Age = 30,
                    Biosex = Biosex.Male,
                    IsAlive = true
                },
                new Person
                {
                    Id = 3,
                    Name = new Name { FullName = "AliceWonder", Prefix = "Alice", Suffix = "Wonder" },
                    Age = 28,
                    Biosex = Biosex.Female,
                    IsAlive = true
                }
            };

            var result = _generationManager.CheckForExtinction(people);

            Assert.False(result);
        }
    }
}