using ProjectMarworyn.Core.Managers;
using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Models.Enums;

namespace ProjectMarworyn.UnitTests
{
    public class GenerationManagerTests
    {
        private readonly GenerationManager _generationManager;

        public GenerationManagerTests()
        {
            _generationManager = new GenerationManager();
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