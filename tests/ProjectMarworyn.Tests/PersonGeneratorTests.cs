using ProjectMarworyn.Generators;
using ProjectMarworyn.Models;
using ProjectMarworyn.Models.Enums;
using ProjectMarworyn.Tests.Mocks;

namespace ProjectMarworyn.Tests
{
    public class PersonGeneratorTests
    {
        private readonly PersonGenerator _personGenerator;
        private readonly MockDiceGenerator _mockDiceGenerator;
        private readonly MockOutputService _mockOutputService;

        public PersonGeneratorTests()
        {
            _mockDiceGenerator = new MockDiceGenerator();
            _mockOutputService = new MockOutputService();
            _personGenerator = new PersonGenerator(_mockDiceGenerator,
                _mockOutputService);
        }

        [Fact]
        public void Initialise_WithEmptyList_ReturnsEmptyList()
        {
            var initialPeople = new List<InitialPerson>();

            var result = _personGenerator.Initialise(initialPeople,
                0);

            Assert.Empty(result);
        }

        [Fact]
        public void Initialise_WithEmptyList_ReturnsNonNullList()
        {
            var initialPeople = new List<InitialPerson>();

            var result = _personGenerator.Initialise(initialPeople,
                0);

            Assert.NotNull(result);
        }

        [Fact]
        public void Initialise_WithSingleName_ReturnsSinglePerson()
        {
            var initialPeople = new List<InitialPerson>
            {
                new InitialPerson { FullName = "TestName", Prefix = "Test", Suffix = "Name", Gender = Gender.Female }
            };

            var result = _personGenerator.Initialise(initialPeople,
                0);

            Assert.Single(result);
        }

        [Fact]
        public void Initialise_WithMultipleNames_ReturnsMatchingNumberOfPeople()
        {
            var initialPeople = new List<InitialPerson>
            {
                new InitialPerson { FullName = "Name1", Prefix = "N", Suffix = "1", Gender = Gender.Female },
                new InitialPerson { FullName = "Name2", Prefix = "N", Suffix = "2", Gender = Gender.Male },
                new InitialPerson { FullName = "Name3", Prefix = "N", Suffix = "3", Gender = Gender.Female }
            };

            var result = _personGenerator.Initialise(initialPeople,
                0);

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void Initialise_AssignsSequentialIds()
        {
            var initialPeople = new List<InitialPerson>
            {
                new InitialPerson { FullName = "Name1", Gender = Gender.Female },
                new InitialPerson { FullName = "Name2", Gender = Gender.Male },
                new InitialPerson { FullName = "Name3", Gender = Gender.Female }
            };

            var result = _personGenerator.Initialise(initialPeople,
                0);

            Assert.Equal(0, result[0].Id);
            Assert.Equal(1, result[1].Id);
            Assert.Equal(2, result[2].Id);
        }

        [Fact]
        public void Initialise_AssignsNameToPerson()
        {
            var initialPeople = new List<InitialPerson>
            {
                new InitialPerson { FullName = "TestName", Prefix = "Test", Suffix = "Name", Gender = Gender.Female }
            };

            var result = _personGenerator.Initialise(initialPeople,
                0);

            Assert.Equal("TestName", result[0].Name.FullName);
            Assert.Equal("Test", result[0].Name.Prefix);
            Assert.Equal("Name", result[0].Name.Suffix);
        }

        [Fact]
        public void Initialise_AssignsGenderToPerson()
        {
            var initialPeople = new List<InitialPerson>
            {
                new InitialPerson { FullName = "TestName", Gender = Gender.Male }
            };

            var result = _personGenerator.Initialise(initialPeople,
                0);

            Assert.Equal(Gender.Male, result[0].Gender);
        }

        [Fact]
        public void Initialise_SetsIsAliveToTrue()
        {
            var initialPeople = new List<InitialPerson>
            {
                new InitialPerson { FullName = "Test", Gender = Gender.Female }
            };

            var result = _personGenerator.Initialise(initialPeople,
                0);

            Assert.True(result[0].IsAlive);
        }

        [Fact]
        public void Initialise_SetsHasPairToFalse()
        {
            var initialPeople = new List<InitialPerson>
            {
                new InitialPerson { FullName = "Test", Gender = Gender.Female }
            };

            var result = _personGenerator.Initialise(initialPeople,
                0);

            Assert.False(result[0].HasPair);
        }

        [Fact]
        public void Initialise_SetsTimeFromLastChildToTwo()
        {
            var initialPeople = new List<InitialPerson>
            {
                new InitialPerson { FullName = "Test", Gender = Gender.Female }
            };

            var result = _personGenerator.Initialise(initialPeople,
                0);

            Assert.Equal(2, result[0].TimeFromLastChild);
        }

        [Fact]
        public void Initialise_AssignsRandomAge()
        {
            var initialPeople = new List<InitialPerson>
            {
                new InitialPerson { FullName = "Test", Gender = Gender.Female }
            };

            var result = _personGenerator.Initialise(initialPeople,
                0);

            Assert.InRange(result[0].Age, 0, 79);
        }

        [Fact]
        public void Initialise_SetsTimeLivedBasedOnAge()
        {
            var initialPeople = new List<InitialPerson>
            {
                new InitialPerson { FullName = "Test", Gender = Gender.Female }
            };

            var result = _personGenerator.Initialise(initialPeople,
                0);

            var expectedTimeLived = new DateTime(1, 1, 1).AddYears(result[0].Age);
            Assert.Equal(expectedTimeLived, result[0].TimeLived);
        }
    }
}