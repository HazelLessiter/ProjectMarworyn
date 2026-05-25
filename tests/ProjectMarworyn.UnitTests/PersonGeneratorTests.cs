using NSubstitute;
using ProjectMarworyn.Core.Generators;
using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Models.Enums;

namespace ProjectMarworyn.UnitTests
{
    public class PersonGeneratorTests
    {
        private readonly PersonGenerator _personGenerator;
        private readonly GameState _gameState;

        public PersonGeneratorTests()
        {
            _gameState = new GameState();
            var mockDiceGenerator = Substitute.For<IDiceGenerator>();
            mockDiceGenerator.Create(Arg.Any<int>()).Returns(new Random(0));
            mockDiceGenerator.Next(Arg.Any<Random>(), Arg.Any<int>(), Arg.Is<int>(x => x == 12)).Returns(1);
            mockDiceGenerator.Next(Arg.Any<Random>(), Arg.Any<int>(), Arg.Is<int>(x => x == 29)).Returns(1);
            mockDiceGenerator.Next(Arg.Any<Random>(), Arg.Any<int>(), Arg.Is<int>(x => x == 4)).Returns(2);
            mockDiceGenerator.Next(Arg.Any<Random>(), Arg.Any<int>(), Arg.Is<int>(x => x == 101)).Returns(50);
            _personGenerator = new PersonGenerator(mockDiceGenerator, _gameState);
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
                new InitialPerson { FullName = "TestName", Prefix = "Test", Suffix = "Name", Biosex = Biosex.Female }
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
                new InitialPerson { FullName = "Name1", Prefix = "N", Suffix = "1", Biosex = Biosex.Female },
                new InitialPerson { FullName = "Name2", Prefix = "N", Suffix = "2", Biosex = Biosex.Male },
                new InitialPerson { FullName = "Name3", Prefix = "N", Suffix = "3", Biosex = Biosex.Female }
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
                new InitialPerson { FullName = "Name1", Biosex = Biosex.Female },
                new InitialPerson { FullName = "Name2", Biosex = Biosex.Male },
                new InitialPerson { FullName = "Name3", Biosex = Biosex.Female }
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
                new InitialPerson { FullName = "TestName", Prefix = "Test", Suffix = "Name", Biosex = Biosex.Female }
            };

            var result = _personGenerator.Initialise(initialPeople,
                0);

            Assert.Equal("TestName", result[0].Name.FullName);
            Assert.Equal("Test", result[0].Name.Prefix);
            Assert.Equal("Name", result[0].Name.Suffix);
        }

        [Fact]
        public void Initialise_AssignsBioSexToPerson()
        {
            var initialPeople = new List<InitialPerson>
            {
                new InitialPerson { FullName = "TestName", Biosex = Biosex.Male }
            };

            var result = _personGenerator.Initialise(initialPeople,
                0);

            Assert.Equal(Biosex.Male, result[0].Biosex);
        }

        [Fact]
        public void Initialise_WithIntersexPerson_SetsBiosexToIntersex()
        {
            var initialPeople = new List<InitialPerson>
            {
                new InitialPerson { FullName = "Test", Biosex = Biosex.Intersex }
            };

            var result = _personGenerator.Initialise(initialPeople,
                0);

            Assert.Equal(Biosex.Intersex, result[0].Biosex);
        }

        [Fact]
        public void Initialise_WithIntersexPerson_GenderIsEitherFemaleOrMale()
        {
            var initialPeople = new List<InitialPerson>
            {
                new InitialPerson { FullName = "Test", Biosex = Biosex.Intersex }
            };

            var result = _personGenerator.Initialise(initialPeople,
                0);

            Assert.True(result[0].Gender == Gender.Female || result[0].Gender == Gender.Male);
        }

        [Fact]
        public void Initialise_SetsIsAliveToTrue()
        {
            var initialPeople = new List<InitialPerson>
            {
                new InitialPerson { FullName = "Test", Biosex = Biosex.Female }
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
                new InitialPerson { FullName = "Test", Biosex = Biosex.Female }
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
                new InitialPerson { FullName = "Test", Biosex = Biosex.Female }
            };

            var result = _personGenerator.Initialise(initialPeople,
                0);

            Assert.Equal(2, result[0].TimeFromLastChild.Year);
        }

        [Fact]
        public void Initialise_AssignsRandomAge()
        {
            var initialPeople = new List<InitialPerson>
            {
                new InitialPerson { FullName = "Test", Biosex = Biosex.Female }
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
                new InitialPerson { FullName = "Test", Biosex = Biosex.Female }
            };

            var result = _personGenerator.Initialise(initialPeople,
                0);

            var expectedTimeLived = new DateTime(1, 1, 1).AddYears(result[0].Age);
            Assert.Equal(expectedTimeLived, result[0].TimeLived);
        }

        // Verifies the exact biosex probability thresholds used in RandomBiosex.
        // femaleChance = (int)BiosexModifier.Female / 100.0 = 50.15
        // maleUpperBound = femaleChance + (int)BiosexModifier.Male / 100.0 = 98.30
        // Test values use a 10% margin either side of each threshold.
        // Seed 14 guarantees childChance = 5 (< 15), so a child is always born.
        [Theory]
        [InlineData(0.4514, Biosex.Female)]   // 10% below Female threshold (nextDouble 0.5015)
        [InlineData(0.5517, Biosex.Male)]     // 10% above Female threshold — falls in Male range
        [InlineData(0.8847, Biosex.Male)]     // 10% below Male upper bound (nextDouble 0.9830)
        [InlineData(0.9900, Biosex.Intersex)] // above Male upper bound
        public void GenerateChildren_BiosexDiceRollThreshold_ReturnsCorrectBiosex(double nextDoubleValue,
            Biosex expectedBiosex)
        {
            var generator = CreateGeneratorWithNextDouble(nextDoubleValue);
            var pair = CreateFertilePair();

            var (children, _) = generator.GenerateChildren(new List<Pair> { pair },
                0,
                0,
                new List<Person> { pair.FPerson, pair.MPerson });

            Assert.Single(children);
            Assert.Equal(expectedBiosex, children[0].Biosex);
        }

        // Verifies the Female→Male and Male→Intersex boundary transitions.
        [Theory]
        [InlineData(0.5015, 0.5016, Biosex.Female, Biosex.Male)]     // Female→Male boundary
        [InlineData(0.9830, 0.9831, Biosex.Male,   Biosex.Intersex)] // Male→Intersex boundary
        public void GenerateChildren_BiosexBoundaryTransition_CorrectBiosexAssigned(double lowerValue,
            double upperValue,
            Biosex lowerExpected,
            Biosex upperExpected)
        {
            var pair1 = CreateFertilePair();
            var pair2 = CreateFertilePair();

            var (lowerChildren, _) = CreateGeneratorWithNextDouble(lowerValue)
                .GenerateChildren(new List<Pair> { pair1 }, 0, 0, new List<Person> { pair1.FPerson, pair1.MPerson });
            var (upperChildren, _) = CreateGeneratorWithNextDouble(upperValue)
                .GenerateChildren(new List<Pair> { pair2 }, 0, 0, new List<Person> { pair2.FPerson, pair2.MPerson });

            Assert.Equal(lowerExpected, lowerChildren[0].Biosex);
            Assert.Equal(upperExpected, upperChildren[0].Biosex);
        }

        private PersonGenerator CreateGeneratorWithNextDouble(double nextDoubleValue)
        {
            var mockDiceGenerator = Substitute.For<IDiceGenerator>();
            mockDiceGenerator.Create(Arg.Any<int>()).Returns(new Random(14)); // seed 14: childChance = 5
            mockDiceGenerator.NextDouble(Arg.Any<Random>()).Returns(nextDoubleValue);
            mockDiceGenerator.Next(Arg.Any<Random>(), Arg.Any<int>(), Arg.Any<int>()).Returns(50);
            return new PersonGenerator(mockDiceGenerator, _gameState);
        }

        private Pair CreateFertilePair()
        {
            return new Pair
            {
                FPerson = new Person
                {
                    Id = 1,
                    Name = new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe" },
                    Age = 25,
                    Biosex = Biosex.Female,
                    IsAlive = true,
                    WillHaveChildren = true,
                    TimeFromLastChild = new DateTime(3, 1, 1)
                },
                MPerson = new Person
                {
                    Id = 2,
                    Name = new Name { FullName = "JohnSmith", Prefix = "John", Suffix = "Smith" },
                    Age = 30,
                    Biosex = Biosex.Male,
                    IsAlive = true,
                    WillHaveChildren = true,
                    TimeFromLastChild = new DateTime(3, 1, 1)
                }
            };
        }
    }
}