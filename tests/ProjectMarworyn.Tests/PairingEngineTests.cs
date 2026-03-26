using ProjectMarworyn.Models;
using ProjectMarworyn.Models.Enums;
using ProjectMarworyn.Tests.Mocks;

namespace ProjectMarworyn.Tests
{
    public class PairingEngineTests
    {
        private readonly PairingEngine _pairingEngine;
        private readonly MockDiceGenerator _mockDiceGenerator;
        private readonly MockOutputService _mockOutputService;

        public PairingEngineTests()
        {
            _mockDiceGenerator = new MockDiceGenerator();
            _mockOutputService = new MockOutputService();
            _pairingEngine = new PairingEngine(_mockDiceGenerator, _mockOutputService);
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
                    Name = new Name { FullName = "Child1", Gender = Gender.Female },
                    Age = 10,
                    Gender = Gender.Female,
                    HasPair = false
                },
                new Person
                {
                    Id = 2,
                    Name = new Name { FullName = "Child2", Gender = Gender.Male },
                    Age = 12,
                    Gender = Gender.Male,
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
            var mockDiceGenerator = new MockDiceGenerator(new Random(42));
            var pairingEngine = new PairingEngine(mockDiceGenerator, _mockOutputService);
            var femalePerson = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Jane", Gender = Gender.Female },
                Age = 25,
                Gender = Gender.Female,
                HasPair = false
            };
            var malePerson = new Person
            {
                Id = 2,
                Name = new Name { FullName = "John", Gender = Gender.Male },
                Age = 28,
                Gender = Gender.Male,
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
            var mockDiceGenerator = new MockDiceGenerator(new Random(42));
            var pairingEngine = new PairingEngine(mockDiceGenerator, _mockOutputService);
            var femalePerson = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Jane", Prefix = "Jane", Suffix = "Doe", Gender = Gender.Female },
                Age = 25,
                Gender = Gender.Female,
                HasPair = false
            };
            var malePerson = new Person
            {
                Id = 2,
                Name = new Name { FullName = "John", Prefix = "John", Suffix = "Smith", Gender = Gender.Male },
                Age = 28,
                Gender = Gender.Male,
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
                    Name = new Name { FullName = "Jane", Gender = Gender.Female },
                    Age = 25,
                    Gender = Gender.Female,
                    HasPair = true
                },
                new Person
                {
                    Id = 2,
                    Name = new Name { FullName = "John", Gender = Gender.Male },
                    Age = 28,
                    Gender = Gender.Male,
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
                    Name = new Name { FullName = "Jane", Gender = Gender.Female },
                    Age = 25,
                    Gender = Gender.Female,
                    HasPair = false
                },
                new Person
                {
                    Id = 2,
                    Name = new Name { FullName = "Alice", Gender = Gender.Female },
                    Age = 30,
                    Gender = Gender.Female,
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
                    Name = new Name { FullName = "John", Gender = Gender.Male },
                    Age = 25,
                    Gender = Gender.Male,
                    HasPair = false
                },
                new Person
                {
                    Id = 2,
                    Name = new Name { FullName = "Bob", Gender = Gender.Male },
                    Age = 30,
                    Gender = Gender.Male,
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
            var mockDiceGenerator = new MockDiceGenerator(new Random(42));
            var pairingEngine = new PairingEngine(mockDiceGenerator, _mockOutputService);
            var people = new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Name = new Name { FullName = "Jane", Gender = Gender.Female },
                    Age = 18,
                    Gender = Gender.Female,
                    HasPair = false
                },
                new Person
                {
                    Id = 2,
                    Name = new Name { FullName = "John", Gender = Gender.Male },
                    Age = 18,
                    Gender = Gender.Male,
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
                    Name = new Name { FullName = "Jane", Gender = Gender.Female },
                    Age = 17,
                    Gender = Gender.Female,
                    HasPair = false
                },
                new Person
                {
                    Id = 2,
                    Name = new Name { FullName = "John", Gender = Gender.Male },
                    Age = 17,
                    Gender = Gender.Male,
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
            var mockDiceGenerator = new MockDiceGenerator(new Random(42));
            var pairingEngine = new PairingEngine(mockDiceGenerator, _mockOutputService);
            var people = new List<Person>
            {
                new Person { Id = 1, Name = new Name { FullName = "F1", Gender = Gender.Female }, Age = 25, Gender = Gender.Female, HasPair = false },
                new Person { Id = 2, Name = new Name { FullName = "F2", Gender = Gender.Female }, Age = 26, Gender = Gender.Female, HasPair = false },
                new Person { Id = 3, Name = new Name { FullName = "F3", Gender = Gender.Female }, Age = 27, Gender = Gender.Female, HasPair = false },
                new Person { Id = 4, Name = new Name { FullName = "M1", Gender = Gender.Male }, Age = 28, Gender = Gender.Male, HasPair = false }
            };
            var pairs = new List<Pair>();

            var (resultPairs, resultPeople) = pairingEngine.GeneratePairs(people, pairs, 0);

            Assert.Single(resultPairs);
        }

        [Fact]
        public void GeneratePairs_WithExistingPairs_AddsToExistingList()
        {
            var mockDiceGenerator = new MockDiceGenerator(new Random(42));
            var pairingEngine = new PairingEngine(mockDiceGenerator, _mockOutputService);
            var existingPair = new Pair
            {
                FPerson = new Person { Id = 1, Name = new Name { FullName = "Existing1", Gender = Gender.Female }, Age = 30, Gender = Gender.Female },
                MPerson = new Person { Id = 2, Name = new Name { FullName = "Existing2", Gender = Gender.Male }, Age = 32, Gender = Gender.Male }
            };
            var pairs = new List<Pair> { existingPair };
            var people = new List<Person>
            {
                new Person { Id = 3, Name = new Name { FullName = "Jane", Gender = Gender.Female }, Age = 25, Gender = Gender.Female, HasPair = false },
                new Person { Id = 4, Name = new Name { FullName = "John", Gender = Gender.Male }, Age = 28, Gender = Gender.Male, HasPair = false }
            };

            var (resultPairs, resultPeople) = pairingEngine.GeneratePairs(people, pairs, 0);

            Assert.Equal(2, resultPairs.Count);
        }

        [Fact]
        public void GeneratePairs_WritesToConsole()
        {
            var mockDiceGenerator = new MockDiceGenerator(new Random(42));
            var mockOutput = new MockOutputService();
            var pairingEngine = new PairingEngine(mockDiceGenerator, mockOutput);
            var people = new List<Person>
            {
                new Person { Id = 1, Name = new Name { FullName = "Jane", Gender = Gender.Female }, Age = 25, Gender = Gender.Female, HasPair = false },
                new Person { Id = 2, Name = new Name { FullName = "John", Gender = Gender.Male }, Age = 28, Gender = Gender.Male, HasPair = false }
            };
            var pairs = new List<Pair>();

            pairingEngine.GeneratePairs(people, pairs, 0);

            Assert.Single(mockOutput.Messages);
            Assert.Contains("Jane", mockOutput.Messages[0]);
            Assert.Contains("John", mockOutput.Messages[0]);
        }

        [Fact]
        public void GeneratePairs_MarksPersonsAsHavingPair()
        {
            var mockDiceGenerator = new MockDiceGenerator(new Random(42));
            var pairingEngine = new PairingEngine(mockDiceGenerator, _mockOutputService);
            var femalePerson = new Person
            {
                Id = 1,
                Name = new Name { FullName = "Jane", Gender = Gender.Female },
                Age = 25,
                Gender = Gender.Female,
                HasPair = false
            };
            var malePerson = new Person
            {
                Id = 2,
                Name = new Name { FullName = "John", Gender = Gender.Male },
                Age = 28,
                Gender = Gender.Male,
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
                new Person { Id = 1, Name = new Name { FullName = "Jane", Gender = Gender.Female }, Age = 25, Gender = Gender.Female, HasPair = false },
                new Person { Id = 2, Name = new Name { FullName = "John", Gender = Gender.Male }, Age = 28, Gender = Gender.Male, HasPair = false }
            };
            var pairs = new List<Pair>();

            var (resultPairs, resultPeople) = _pairingEngine.GeneratePairs(people, pairs, 0);

            Assert.NotNull(resultPeople);
            Assert.Equal(people, resultPeople);
        }
    }
}

