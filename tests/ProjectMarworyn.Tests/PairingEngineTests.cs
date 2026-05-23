using NSubstitute;
using ProjectMarworyn.Core;
using ProjectMarworyn.Core.Generators;
using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Models.Enums;
using ProjectMarworyn.Tests.Mocks;

namespace ProjectMarworyn.Tests
{
    public class PairingEngineTests
    {
        private readonly PairingEngine _pairingEngine;
        private readonly MockConsoleService _mockOutputService;

        public PairingEngineTests()
        {
            _mockOutputService = new MockConsoleService();
            var mockDiceGenerator = Substitute.For<IDiceGenerator>();
            mockDiceGenerator.Create(Arg.Any<int>()).Returns(new Random(0));
            _pairingEngine = new PairingEngine(mockDiceGenerator, _mockOutputService);
        }

        [Fact]
        public void GeneratePairs_WithEmptyPeopleList_ReturnsEmptyPairs()
        {
            var people = new List<Person>();
            var pairs = new List<Pair>();

            var (resultPairs, resultPeople) = _pairingEngine.GeneratePairs(people, pairs, 0);

            Assert.Empty(resultPairs);
        }

        [Fact]
        public void GeneratePairs_WithNullPairs_ReturnsNonNullList()
        {
            var people = new List<Person>();
            var pairs = new List<Pair>();

            var (resultPairs, resultPeople) = _pairingEngine.GeneratePairs(people, pairs, 0);

            Assert.NotNull(resultPairs);
            Assert.NotNull(resultPeople);
        }

        [Fact]
        public void GeneratePairs_WithOnlyChildren_ReturnsEmptyPairs()
        {
            var people = new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Name = new Name { FullName = "Child1" },
                    Age = 10,
                    Biosex = Biosex.Female,
                    HasPair = false
                },
                new Person
                {
                    Id = 2,
                    Name = new Name { FullName = "Child2" },
                    Age = 12,
                    Biosex = Biosex.Male,
                    HasPair = false
                }
            };
            var pairs = new List<Pair>();

            var (resultPairs, resultPeople) = _pairingEngine.GeneratePairs(people, pairs, 0);

            Assert.Empty(resultPairs);
        }

        [Fact]
        public void GeneratePairs_WithOneAdultFemaleAndOneAdultMale_CreatesOnePair()
        {
            var mockDiceGenerator = Substitute.For<IDiceGenerator>();
            mockDiceGenerator.Create(Arg.Any<int>()).Returns(new Random(0));
            var pairingEngine = new PairingEngine(mockDiceGenerator, _mockOutputService);
            var femalePerson = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Jane" },
                Age = 25,
                Biosex = Biosex.Female,
                HasPair = false
            };
            var malePerson = new Person
            {
                Id = 2,
                Name = new Name { FullName = "John" },
                Age = 28,
                Biosex = Biosex.Male,
                HasPair = false
            };
            var people = new List<Person> { femalePerson, malePerson };
            var pairs = new List<Pair>();

            var (resultPairs, resultPeople) = pairingEngine.GeneratePairs(people, pairs, 0);

            Assert.Single(resultPairs);
        }

        [Fact]
        public void GeneratePairs_WithOneAdultFemaleAndOneAdultMale_PairContainsCorrectPeople()
        {
            var mockDiceGenerator = Substitute.For<IDiceGenerator>();
            mockDiceGenerator.Create(Arg.Any<int>()).Returns(new Random(0));
            var pairingEngine = new PairingEngine(mockDiceGenerator, _mockOutputService);
            var femalePerson = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Jane", Prefix = "Jane", Suffix = "Doe" },
                Age = 25,
                Biosex = Biosex.Female,
                HasPair = false
            };
            var malePerson = new Person
            {
                Id = 2,
                Name = new Name { FullName = "John", Prefix = "John", Suffix = "Smith" },
                Age = 28,
                Biosex = Biosex.Male,
                HasPair = false
            };
            var people = new List<Person> { femalePerson, malePerson };
            var pairs = new List<Pair>();

            var (resultPairs, resultPeople) = pairingEngine.GeneratePairs(people, pairs, 0);

            Assert.Equal("Jane", resultPairs[0].FPerson.Name.FullName);
            Assert.Equal("John", resultPairs[0].MPerson.Name.FullName);
        }

        [Fact]
        public void GeneratePairs_WithAlreadyPairedPeople_DoesNotPairThem()
        {
            var people = new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Name = new Name { FullName = "Jane" },
                    Age = 25,
                    Biosex = Biosex.Female,
                    HasPair = true
                },
                new Person
                {
                    Id = 2,
                    Name = new Name { FullName = "John" },
                    Age = 28,
                    Biosex = Biosex.Male,
                    HasPair = true
                }
            };
            var pairs = new List<Pair>();

            var (resultPairs, resultPeople) = _pairingEngine.GeneratePairs(people, pairs, 0);

            Assert.Empty(resultPairs);
        }

        [Fact]
        public void GeneratePairs_WithOnlyFemales_ReturnsEmptyPairs()
        {
            var people = new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Name = new Name { FullName = "Jane" },
                    Age = 25,
                    Biosex = Biosex.Female,
                    HasPair = false
                },
                new Person
                {
                    Id = 2,
                    Name = new Name { FullName = "Alice" },
                    Age = 30,
                    Biosex = Biosex.Female,
                    HasPair = false
                }
            };
            var pairs = new List<Pair>();

            var (resultPairs, resultPeople) = _pairingEngine.GeneratePairs(people, pairs, 0);

            Assert.Empty(resultPairs);
        }

        [Fact]
        public void GeneratePairs_WithOnlyMales_ReturnsEmptyPairs()
        {
            var people = new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Name = new Name { FullName = "John" },
                    Age = 25,
                    Biosex = Biosex.Male,
                    HasPair = false
                },
                new Person
                {
                    Id = 2,
                    Name = new Name { FullName = "Bob" },
                    Age = 30,
                    Biosex = Biosex.Male,
                    HasPair = false
                }
            };
            var pairs = new List<Pair>();

            var (resultPairs, resultPeople) = _pairingEngine.GeneratePairs(people, pairs, 0);

            Assert.Empty(resultPairs);
        }

        [Fact]
        public void GeneratePairs_WithExactly18YearOlds_CreatesPairs()
        {
            var mockDiceGenerator = Substitute.For<IDiceGenerator>();
            mockDiceGenerator.Create(Arg.Any<int>()).Returns(new Random(0));
            var pairingEngine = new PairingEngine(mockDiceGenerator, _mockOutputService);
            var people = new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Name = new Name { FullName = "Jane" },
                    Age = 18,
                    Biosex = Biosex.Female,
                    HasPair = false
                },
                new Person
                {
                    Id = 2,
                    Name = new Name { FullName = "John" },
                    Age = 18,
                    Biosex = Biosex.Male,
                    HasPair = false
                }
            };
            var pairs = new List<Pair>();

            var (resultPairs, resultPeople) = pairingEngine.GeneratePairs(people, pairs, 0);

            Assert.Single(resultPairs);
        }

        [Fact]
        public void GeneratePairs_With17YearOlds_DoesNotCreatePairs()
        {
            var people = new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Name = new Name { FullName = "Jane" },
                    Age = 17,
                    Biosex = Biosex.Female,
                    HasPair = false
                },
                new Person
                {
                    Id = 2,
                    Name = new Name { FullName = "John" },
                    Age = 17,
                    Biosex = Biosex.Male,
                    HasPair = false
                }
            };
            var pairs = new List<Pair>();

            var (resultPairs, resultPeople) = _pairingEngine.GeneratePairs(people, pairs, 0);

            Assert.Empty(resultPairs);
        }

        [Fact]
        public void GeneratePairs_WithMoreFemalesThanMales_CreatesLimitedPairs()
        {
            var mockDiceGenerator = Substitute.For<IDiceGenerator>();
            mockDiceGenerator.Create(Arg.Any<int>()).Returns(new Random(0));
            var pairingEngine = new PairingEngine(mockDiceGenerator, _mockOutputService);
            var people = new List<Person>
            {
                new Person { Id = 1, Name = new Name { FullName = "F1" }, Age = 25, Biosex = Biosex.Female, HasPair = false },
                new Person { Id = 2, Name = new Name { FullName = "F2" }, Age = 26, Biosex = Biosex.Female, HasPair = false },
                new Person { Id = 3, Name = new Name { FullName = "F3" }, Age = 27, Biosex = Biosex.Female, HasPair = false },
                new Person { Id = 4, Name = new Name { FullName = "M1" }, Age = 28, Biosex = Biosex.Male, HasPair = false }
            };
            var pairs = new List<Pair>();

            var (resultPairs, resultPeople) = pairingEngine.GeneratePairs(people, pairs, 0);

            Assert.Single(resultPairs);
        }

        [Fact]
        public void GeneratePairs_WithExistingPairs_AddsToExistingList()
        {
            var mockDiceGenerator = Substitute.For<IDiceGenerator>();
            mockDiceGenerator.Create(Arg.Any<int>()).Returns(new Random(0));
            var pairingEngine = new PairingEngine(mockDiceGenerator, _mockOutputService);
            var existingPair = new Pair
            {
                FPerson = new Person { Id = 1, Name = new Name { FullName = "Existing1" }, Age = 30, Biosex = Biosex.Female },
                MPerson = new Person { Id = 2, Name = new Name { FullName = "Existing2" }, Age = 32, Biosex = Biosex.Male }
            };
            var pairs = new List<Pair> { existingPair };
            var people = new List<Person>
            {
                new Person { Id = 3, Name = new Name { FullName = "Jane" }, Age = 25, Biosex = Biosex.Female, HasPair = false },
                new Person { Id = 4, Name = new Name { FullName = "John" }, Age = 28, Biosex = Biosex.Male, HasPair = false }
            };

            var (resultPairs, resultPeople) = pairingEngine.GeneratePairs(people, pairs, 0);

            Assert.Equal(2, resultPairs.Count);
        }

        [Fact]
        public void GeneratePairs_WritesToConsole()
        {
            var mockDiceGenerator = Substitute.For<IDiceGenerator>();
            mockDiceGenerator.Create(Arg.Any<int>()).Returns(new Random(0));
            var mockOutput = new MockConsoleService();
            var pairingEngine = new PairingEngine(mockDiceGenerator, mockOutput);
            var people = new List<Person>
            {
                new Person { Id = 1, Name = new Name { FullName = "Jane" }, Age = 25, Biosex = Biosex.Female, HasPair = false },
                new Person { Id = 2, Name = new Name { FullName = "John" }, Age = 28, Biosex = Biosex.Male, HasPair = false }
            };
            var pairs = new List<Pair>();

            pairingEngine.GeneratePairs(people, pairs, 0);

            Assert.Single(mockOutput.Lines);
            Assert.Contains("Jane", mockOutput.Lines[0]);
            Assert.Contains("John", mockOutput.Lines[0]);
        }

        [Fact]
        public void GeneratePairs_MarksPersonsAsHavingPair()
        {
            var mockDiceGenerator = Substitute.For<IDiceGenerator>();
            mockDiceGenerator.Create(Arg.Any<int>()).Returns(new Random(0));
            var pairingEngine = new PairingEngine(mockDiceGenerator, _mockOutputService);
            var femalePerson = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Jane" },
                Age = 25,
                Biosex = Biosex.Female,
                HasPair = false
            };
            var malePerson = new Person
            {
                Id = 2,
                Name = new Name { FullName = "John" },
                Age = 28,
                Biosex = Biosex.Male,
                HasPair = false
            };
            var people = new List<Person> { femalePerson, malePerson };
            var pairs = new List<Pair>();

            pairingEngine.GeneratePairs(people, pairs, 0);

            Assert.True(femalePerson.HasPair);
            Assert.True(malePerson.HasPair);
        }

        [Fact]
        public void GeneratePairs_ReturnsPeopleList()
        {
            var people = new List<Person>
            {
                new Person { Id = 1, Name = new Name { FullName = "Jane" }, Age = 25, Biosex = Biosex.Female, HasPair = false },
                new Person { Id = 2, Name = new Name { FullName = "John" }, Age = 28, Biosex = Biosex.Male, HasPair = false }
            };
            var pairs = new List<Pair>();

            var (resultPairs, resultPeople) = _pairingEngine.GeneratePairs(people, pairs, 0);

            Assert.NotNull(resultPeople);
            Assert.Equal(people, resultPeople);
        }
    }
}
