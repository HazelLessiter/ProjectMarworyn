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
            var names = new List<Name>();

            var result = _personGenerator.Initialise(names,
                0);

            Assert.Empty(result);
        }

        [Fact]
        public void Initialise_WithEmptyList_ReturnsNonNullList()
        {
            var names = new List<Name>();

            var result = _personGenerator.Initialise(names,
                0);

            Assert.NotNull(result);
        }

        [Fact]
        public void Initialise_WithSingleName_ReturnsSinglePerson()
        {
            var names = new List<Name>
            {
                new Name { FullName = "TestName", Prefix = "Test", Suffix = "Name", Gender = Gender.Female }
            };

            var result = _personGenerator.Initialise(names,
                0);

            Assert.Single(result);
        }

        [Fact]
        public void Initialise_WithMultipleNames_ReturnsMatchingNumberOfPeople()
        {
            var names = new List<Name>
            {
                new Name { FullName = "Name1", Prefix = "N", Suffix = "1", Gender = Gender.Female },
                new Name { FullName = "Name2", Prefix = "N", Suffix = "2", Gender = Gender.Male },
                new Name { FullName = "Name3", Prefix = "N", Suffix = "3", Gender = Gender.Female }
            };

            var result = _personGenerator.Initialise(names,
                0);

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void Initialise_AssignsSequentialIds()
        {
            var names = new List<Name>
            {
                new Name { FullName = "Name1", Gender = Gender.Female },
                new Name { FullName = "Name2", Gender = Gender.Male },
                new Name { FullName = "Name3", Gender = Gender.Female }
            };

            var result = _personGenerator.Initialise(names,
                0);

            Assert.Equal(0, result[0].Id);
            Assert.Equal(1, result[1].Id);
            Assert.Equal(2, result[2].Id);
        }

        [Fact]
        public void Initialise_AssignsNameToPerson()
        {
            var name = new Name { FullName = "TestName", Prefix = "Test", Suffix = "Name", Gender = Gender.Female };
            var names = new List<Name> { name };

            var result = _personGenerator.Initialise(names,
                0);

            Assert.Equal(name, result[0].Name);
        }

        [Fact]
        public void Initialise_AssignsGenderFromName()
        {
            var names = new List<Name>
            {
                new Name { FullName = "Female", Gender = Gender.Female },
                new Name { FullName = "Male", Gender = Gender.Male }
            };

            var result = _personGenerator.Initialise(names,
                0);

            Assert.Equal(Gender.Female, result[0].Gender);
            Assert.Equal(Gender.Male, result[1].Gender);
        }

        [Fact]
        public void Initialise_SetsIsAliveToTrue()
        {
            var names = new List<Name>
            {
                new Name { FullName = "Test", Gender = Gender.Female }
            };

            var result = _personGenerator.Initialise(names,
                0);

            Assert.True(result[0].IsAlive);
        }

        [Fact]
        public void Initialise_SetsHasPairToFalse()
        {
            var names = new List<Name>
            {
                new Name { FullName = "Test", Gender = Gender.Female }
            };

            var result = _personGenerator.Initialise(names,
                0);

            Assert.False(result[0].HasPair);
        }

        [Fact]
        public void Initialise_SetsTimeFromLastChildToZero()
        {
            var names = new List<Name>
            {
                new Name { FullName = "Test", Gender = Gender.Female }
            };

            var result = _personGenerator.Initialise(names,
                0);

            Assert.Equal(0, result[0].TimeFromLastChild);
        }

        [Fact]
        public void Initialise_AssignsRandomAge()
        {
            var names = new List<Name>
            {
                new Name { FullName = "Test", Gender = Gender.Female }
            };

            var result = _personGenerator.Initialise(names,
                0);

            Assert.InRange(result[0].Age, 0, 79);
        }

        [Fact]
        public void Initialise_SetsTimeLivedBasedOnAge()
        {
            var names = new List<Name>
            {
                new Name { FullName = "Test", Gender = Gender.Female }
            };

            var result = _personGenerator.Initialise(names,
                0);

            var expectedTimeLived = new DateTime(1, 1, 1).AddYears(result[0].Age);
            Assert.Equal(expectedTimeLived, result[0].TimeLived);
        }
    }
}