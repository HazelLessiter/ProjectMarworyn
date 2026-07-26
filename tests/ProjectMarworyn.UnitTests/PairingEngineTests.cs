using NSubstitute;
using ProjectMarworyn.Core;
using ProjectMarworyn.Core.Generators;
using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Models.Enums;

namespace ProjectMarworyn.UnitTests
{
    public class PairingEngineTests
    {
        private readonly PairingEngine _pairingEngine;
        private readonly GameState _gameState;
        private readonly IAttractionCalculator _attractionCalculator;
        private IDiceGenerator _mockDiceGenerator;

        public PairingEngineTests()
        {
            _gameState = new GameState();
            _mockDiceGenerator = Substitute.For<IDiceGenerator>();
            _mockDiceGenerator.Create(Arg.Any<int>(), Arg.Any<DateTime>()).Returns(new Random(0));
            //The real calculator, not a mock: these tests assert orientation-driven pairing
            //outcomes, which is the engine and the attraction policy working together
            _attractionCalculator = new AttractionCalculator();
            _pairingEngine = new PairingEngine(_mockDiceGenerator,
                _attractionCalculator,
                _gameState);
        }

        [Fact]
        public void GeneratePairs_WithEmptyPeopleList_ReturnsEmptyPairs()
        {
            var people = new List<Person>();
            var pairs = new List<Pair>();

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Empty(result);
        }

        [Fact]
        public void GeneratePairs_WithNullPairs_ReturnsNonNullList()
        {
            var people = new List<Person>();
            var pairs = new List<Pair>();

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.NotNull(result);
        }

        [Fact]
        public void GeneratePairs_WithOnlyChildren_ReturnsEmptyPairs()
        {
            var people = new List<Person>
            {
                CreateAdult(1, "Child1", Biosex.Female, Gender.Female, age: 10),
                CreateAdult(2, "Child2", Biosex.Male, Gender.Male, age: 12)
            };
            var pairs = new List<Pair>();

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Empty(result);
        }

        [Fact]
        public void GeneratePairs_WithOneAdultFemaleAndOneAdultMale_CreatesOnePair()
        {
            var people = new List<Person>
            {
                CreateAdult(1, "Jane", Biosex.Female, Gender.Female),
                CreateAdult(2, "John", Biosex.Male, Gender.Male, age: 28)
            };
            var pairs = new List<Pair>();

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Single(result);
        }

        [Fact]
        public void GeneratePairs_WithOneAdultFemaleAndOneAdultMale_PairContainsCorrectPeople()
        {
            var people = new List<Person>
            {
                CreateAdult(1, "Jane", Biosex.Female, Gender.Female),
                CreateAdult(2, "John", Biosex.Male, Gender.Male, age: 28)
            };
            var pairs = new List<Pair>();

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Equal("Jane", result[0].PersonA.Name.FullName);
            Assert.Equal("John", result[0].PersonB.Name.FullName);
        }

        [Fact]
        public void GeneratePairs_WithAlreadyPairedPeople_DoesNotPairThem()
        {
            var people = new List<Person>
            {
                CreateAdult(1, "Jane", Biosex.Female, Gender.Female, hasPair: true),
                CreateAdult(2, "John", Biosex.Male, Gender.Male, age: 28, hasPair: true)
            };
            var pairs = new List<Pair>();

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Empty(result);
        }

        // Two heterosexual people of the same gender have no mutual attraction,
        // so the old "no males, no pairs" outcome survives via orientation.
        [Fact]
        public void GeneratePairs_WithOnlyHeterosexualWomen_ReturnsEmptyPairs()
        {
            var people = new List<Person>
            {
                CreateAdult(1, "Jane", Biosex.Female, Gender.Female),
                CreateAdult(2, "Alice", Biosex.Female, Gender.Female, age: 30)
            };
            var pairs = new List<Pair>();

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Empty(result);
        }

        [Fact]
        public void GeneratePairs_WithOnlyHeterosexualMen_ReturnsEmptyPairs()
        {
            var people = new List<Person>
            {
                CreateAdult(1, "John", Biosex.Male, Gender.Male),
                CreateAdult(2, "Bob", Biosex.Male, Gender.Male, age: 30)
            };
            var pairs = new List<Pair>();

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Empty(result);
        }

        [Fact]
        public void GeneratePairs_WithExactly18YearOlds_CreatesPairs()
        {
            var people = new List<Person>
            {
                CreateAdult(1, "Jane", Biosex.Female, Gender.Female, age: 18),
                CreateAdult(2, "John", Biosex.Male, Gender.Male, age: 18)
            };
            var pairs = new List<Pair>();

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Single(result);
        }

        [Fact]
        public void GeneratePairs_With17YearOlds_DoesNotCreatePairs()
        {
            var people = new List<Person>
            {
                CreateAdult(1, "Jane", Biosex.Female, Gender.Female, age: 17),
                CreateAdult(2, "John", Biosex.Male, Gender.Male, age: 17)
            };
            var pairs = new List<Pair>();

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Empty(result);
        }

        [Fact]
        public void GeneratePairs_WithMoreFemalesThanMales_CreatesLimitedPairs()
        {
            var people = new List<Person>
            {
                CreateAdult(1, "F1", Biosex.Female, Gender.Female),
                CreateAdult(2, "F2", Biosex.Female, Gender.Female, age: 26),
                CreateAdult(3, "F3", Biosex.Female, Gender.Female, age: 27),
                CreateAdult(4, "M1", Biosex.Male, Gender.Male, age: 28)
            };
            var pairs = new List<Pair>();

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Single(result);
        }

        [Fact]
        public void GeneratePairs_WithExistingPairs_AddsToExistingList()
        {
            var existingPair = new Pair
            {
                PersonA = CreateAdult(1, "Existing1", Biosex.Female, Gender.Female, age: 30),
                PersonB = CreateAdult(2, "Existing2", Biosex.Male, Gender.Male, age: 32)
            };
            var pairs = new List<Pair> { existingPair };
            var people = new List<Person>
            {
                CreateAdult(3, "Jane", Biosex.Female, Gender.Female),
                CreateAdult(4, "John", Biosex.Male, Gender.Male, age: 28)
            };

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GeneratePairs_WritesToConsole()
        {
            var people = new List<Person>
            {
                CreateAdult(1, "Jane", Biosex.Female, Gender.Female),
                CreateAdult(2, "John", Biosex.Male, Gender.Male, age: 28)
            };
            var pairs = new List<Pair>();

            _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Single(_gameState.Text);
            Assert.Contains("Jane", _gameState.Text[0]);
            Assert.Contains("John", _gameState.Text[0]);
        }

        [Fact]
        public void GeneratePairs_MarksPersonsAsHavingPair()
        {
            var femalePerson = CreateAdult(1, "Jane", Biosex.Female, Gender.Female);
            var malePerson = CreateAdult(2, "John", Biosex.Male, Gender.Male, age: 28);
            var people = new List<Person> { femalePerson, malePerson };
            var pairs = new List<Pair>();

            _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.True(femalePerson.HasPair);
            Assert.True(malePerson.HasPair);
        }

        [Theory]
        [InlineData(Gender.Female)]
        [InlineData(Gender.Male)]
        public void GeneratePairs_TwoHomosexualPeopleOfSameGender_CreatesPair(Gender gender)
        {
            var biosex = gender == Gender.Female ?
                Biosex.Female :
                Biosex.Male;
            var people = new List<Person>
            {
                CreateAdult(1, "A", biosex, gender, Orientation.Homosexual),
                CreateAdult(2, "B", biosex, gender, Orientation.Homosexual, age: 28)
            };
            var pairs = new List<Pair>();

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Single(result);
        }

        // Attraction must be mutual: she is attracted to him, he is not attracted to her.
        [Fact]
        public void GeneratePairs_HeterosexualWomanAndHomosexualMan_NoPairCreated()
        {
            var people = new List<Person>
            {
                CreateAdult(1, "Jane", Biosex.Female, Gender.Female),
                CreateAdult(2, "John", Biosex.Male, Gender.Male, Orientation.Homosexual, age: 28)
            };
            var pairs = new List<Pair>();

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Empty(result);
        }

        [Fact]
        public void GeneratePairs_BisexualManAndHomosexualMan_CreatesPair()
        {
            var people = new List<Person>
            {
                CreateAdult(1, "Bi", Biosex.Male, Gender.Male, Orientation.Bisexual),
                CreateAdult(2, "Homo", Biosex.Male, Gender.Male, Orientation.Homosexual, age: 28)
            };
            var pairs = new List<Pair>();

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Single(result);
        }

        // The pansexual person reaches the non-binary gender; the heterosexual non-binary
        // person is attracted to any gender other than their own.
        [Fact]
        public void GeneratePairs_PansexualWomanAndHeterosexualNonBinaryPerson_CreatesPair()
        {
            var people = new List<Person>
            {
                CreateAdult(1, "Pan", Biosex.Female, Gender.Female, Orientation.Pansexual),
                CreateAdult(2, "Enby", Biosex.Male, Gender.NonBinary, age: 28)
            };
            var pairs = new List<Pair>();

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Single(result);
        }

        // WillPair is the orientation-independent opt-out: a compatible partner is available,
        // but the person who rolled WillPair = false never enters the pool.
        [Fact]
        public void GeneratePairs_WillPairFalse_PersonNeverPairs()
        {
            var people = new List<Person>
            {
                CreateAdult(1, "Jane", Biosex.Female, Gender.Female, willPair: false),
                CreateAdult(2, "John", Biosex.Male, Gender.Male, age: 28)
            };
            var pairs = new List<Pair>();

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Empty(result);
            Assert.False(people[0].HasPair);
        }

        [Theory]
        [InlineData(Orientation.Aromantic)]
        [InlineData(Orientation.Aroace)]
        public void GeneratePairs_AromanticOrAroacePerson_NeverPairs(Orientation orientation)
        {
            var people = new List<Person>
            {
                CreateAdult(1, "Aro", Biosex.Female, Gender.Female, orientation),
                CreateAdult(2, "John", Biosex.Male, Gender.Male, age: 28)
            };
            var pairs = new List<Pair>();

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Empty(result);
        }

        // Intersex people were silently excluded from the old biosex-pool pairing;
        // attraction-driven pairing brings them in via their gender.
        [Fact]
        public void GeneratePairs_IntersexPerson_CanPair()
        {
            var people = new List<Person>
            {
                CreateAdult(1, "Jane", Biosex.Female, Gender.Female),
                CreateAdult(2, "Inter", Biosex.Intersex, Gender.Male, age: 28)
            };
            var pairs = new List<Pair>();

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Single(result);
        }

        // Someone claimed as a partner mid-loop must be skipped when their own turn comes:
        // four compatible people form exactly two pairs, nobody pairs twice.
        [Fact]
        public void GeneratePairs_FourCompatiblePeople_EveryonePairsExactlyOnce()
        {
            var people = new List<Person>
            {
                CreateAdult(1, "F1", Biosex.Female, Gender.Female),
                CreateAdult(2, "M1", Biosex.Male, Gender.Male, age: 26),
                CreateAdult(3, "F2", Biosex.Female, Gender.Female, age: 27),
                CreateAdult(4, "M2", Biosex.Male, Gender.Male, age: 28)
            };
            var pairs = new List<Pair>();

            var result = _pairingEngine.GeneratePairs(people,
                pairs,
                0,
                new DateTime(1, 1, 1));

            Assert.Equal(2, result.Count);
            Assert.All(people, person => Assert.True(person.HasPair));
        }

        private static Person CreateAdult(int id,
            string fullName,
            Biosex biosex,
            Gender gender,
            Orientation orientation = Orientation.Heterosexual,
            int age = 25,
            bool willPair = true,
            bool hasPair = false)
        {
            return new Person
            {
                Id = id,
                Name = new Name { FullName = fullName },
                Age = age,
                Biosex = biosex,
                Gender = gender,
                Orientation = orientation,
                WillPair = willPair,
                HasPair = hasPair,
                IsAlive = true
            };
        }
    }
}