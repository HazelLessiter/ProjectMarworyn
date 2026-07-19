using Microsoft.Extensions.Options;
using NSubstitute;
using ProjectMarworyn.Core.Configuration;
using ProjectMarworyn.Core.Generators;
using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Models.Enums;

namespace ProjectMarworyn.UnitTests
{
    public class PersonGeneratorTests
    {
        private readonly PersonGenerator _personGenerator;
        private readonly GameState _gameState;
        private IDiceGenerator _mockDiceGenerator;

        public PersonGeneratorTests()
        {
            _gameState = new GameState();
            _mockDiceGenerator = Substitute.For<IDiceGenerator>();
            _mockDiceGenerator.Create(Arg.Any<int>()).Returns(new Random(0));
            _mockDiceGenerator.Next(Arg.Any<Random>(), Arg.Any<int>(), Arg.Is<int>(x => x == 12)).Returns(1);
            _mockDiceGenerator.Next(Arg.Any<Random>(), Arg.Any<int>(), Arg.Is<int>(x => x == 29)).Returns(1);
            _mockDiceGenerator.Next(Arg.Any<Random>(), Arg.Any<int>(), Arg.Is<int>(x => x == 4)).Returns(2);
            _mockDiceGenerator.Next(Arg.Any<Random>(), Arg.Any<int>(), Arg.Is<int>(x => x == 101)).Returns(50);
            _personGenerator = new PersonGenerator(_mockDiceGenerator,
                _gameState,
                Options.Create(new AppSettings { FertilityCooldownYears = 3 }));
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

        // Gender comes straight from the data file, so misaligned (trans) and intersex
        // combinations must pass through unchanged.
        [Theory]
        [InlineData(Biosex.Female, Gender.Female)]
        [InlineData(Biosex.Male, Gender.Male)]
        [InlineData(Biosex.Male, Gender.Female)]
        [InlineData(Biosex.Female, Gender.NonBinary)]
        [InlineData(Biosex.Intersex, Gender.Male)]
        public void Initialise_AssignsGenderFromInitialPerson(Biosex biosex,
            Gender gender)
        {
            var initialPeople = new List<InitialPerson>
            {
                new InitialPerson { FullName = "Test", Biosex = biosex, Gender = gender }
            };

            var result = _personGenerator.Initialise(initialPeople,
                0);

            Assert.Equal(gender, result[0].Gender);
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

            var result = generator.GenerateChildren(new List<Pair> { pair },
                0,
                0,
                new List<Person> { pair.FPerson, pair.MPerson },
                new DateTime(1, 1, 1));

            Assert.Single(result.Children);
            Assert.Equal(expectedBiosex, result.Children[0].Biosex);
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

            var lowerResult = CreateGeneratorWithNextDouble(lowerValue)
                .GenerateChildren(new List<Pair> { pair1 }, 0, 0, new List<Person> { pair1.FPerson, pair1.MPerson }, new DateTime(1, 1, 1));
            var upperResult = CreateGeneratorWithNextDouble(upperValue)
                .GenerateChildren(new List<Pair> { pair2 }, 0, 0, new List<Person> { pair2.FPerson, pair2.MPerson }, new DateTime(1, 1, 1));

            Assert.Equal(lowerExpected, lowerResult.Children[0].Biosex);
            Assert.Equal(upperExpected, upperResult.Children[0].Biosex);
        }

        // The mocked NextDouble feeds both the biosex roll and the gender deviation roll,
        // so 0.4514 → biosex Female (45.14 <= 50.15) and deviation roll 45.14.
        // With TransgenderProbability at its default of 0, gender must align with biosex.
        [Theory]
        [InlineData(0.4514, Biosex.Female, Gender.Female)]
        [InlineData(0.5517, Biosex.Male, Gender.Male)]
        public void GenerateChildren_WithZeroTransgenderProbability_GenderAlignsWithBiosex(double nextDoubleValue,
            Biosex expectedBiosex,
            Gender expectedGender)
        {
            var generator = CreateGeneratorWithNextDouble(nextDoubleValue);
            var pair = CreateFertilePair();

            var result = generator.GenerateChildren(new List<Pair> { pair },
                0,
                0,
                new List<Person> { pair.FPerson, pair.MPerson },
                new DateTime(1, 1, 1));

            Assert.Equal(expectedBiosex, result.Children[0].Biosex);
            Assert.Equal(expectedGender, result.Children[0].Gender);
        }

        // With TransgenderProbability at 100% every deviation roll hits,
        // so gender must differ from the biosex-aligned gender.
        [Theory]
        [InlineData(0.4514, Biosex.Female, Gender.Male)]
        [InlineData(0.5517, Biosex.Male, Gender.Female)]
        public void GenerateChildren_WithFullTransgenderProbability_GenderDeviatesFromBiosex(double nextDoubleValue,
            Biosex expectedBiosex,
            Gender expectedGender)
        {
            var generator = CreateGeneratorWithNextDouble(nextDoubleValue,
                transgenderProbability: 100);
            var pair = CreateFertilePair();

            var result = generator.GenerateChildren(new List<Pair> { pair },
                0,
                0,
                new List<Person> { pair.FPerson, pair.MPerson },
                new DateTime(1, 1, 1));

            Assert.Equal(expectedBiosex, result.Children[0].Biosex);
            Assert.Equal(expectedGender, result.Children[0].Gender);
        }

        [Fact]
        public void GenerateChildren_IntersexChild_GenderIsEitherFemaleOrMale()
        {
            var generator = CreateGeneratorWithNextDouble(0.9900);
            var pair = CreateFertilePair();

            var result = generator.GenerateChildren(new List<Pair> { pair },
                0,
                0,
                new List<Person> { pair.FPerson, pair.MPerson },
                new DateTime(1, 1, 1));

            Assert.Equal(Biosex.Intersex, result.Children[0].Biosex);
            Assert.True(result.Children[0].Gender == Gender.Female || result.Children[0].Gender == Gender.Male);
        }

        // With NonBinaryProbability at 100% every roll lands in the non-binary band,
        // regardless of biosex (intersex included).
        [Theory]
        [InlineData(0.4514, Biosex.Female)]
        [InlineData(0.5517, Biosex.Male)]
        [InlineData(0.9900, Biosex.Intersex)]
        public void GenerateChildren_WithFullNonBinaryProbability_ChildIsNonBinary(double nextDoubleValue,
            Biosex expectedBiosex)
        {
            var generator = CreateGeneratorWithNextDouble(nextDoubleValue,
                nonBinaryProbability: 100);
            var pair = CreateFertilePair();

            var result = generator.GenerateChildren(new List<Pair> { pair },
                0,
                0,
                new List<Person> { pair.FPerson, pair.MPerson },
                new DateTime(1, 1, 1));

            Assert.Equal(expectedBiosex, result.Children[0].Biosex);
            Assert.Equal(Gender.NonBinary, result.Children[0].Gender);
        }

        // The non-binary band sits below the binary-flip band inside the same roll:
        // with NonBinaryProbability 50 and TransgenderProbability 100, a roll of 45.14
        // is non-binary while 55.17 falls through to the binary flip.
        [Theory]
        [InlineData(0.4514, Gender.NonBinary)] // biosex Female
        [InlineData(0.5517, Gender.Female)]    // biosex Male, flipped
        public void GenerateChildren_NonBinaryBandSitsBelowBinaryFlipBand(double nextDoubleValue,
            Gender expectedGender)
        {
            var generator = CreateGeneratorWithNextDouble(nextDoubleValue,
                transgenderProbability: 100,
                nonBinaryProbability: 50);
            var pair = CreateFertilePair();

            var result = generator.GenerateChildren(new List<Pair> { pair },
                0,
                0,
                new List<Person> { pair.FPerson, pair.MPerson },
                new DateTime(1, 1, 1));

            Assert.Equal(expectedGender, result.Children[0].Gender);
        }

        // Route 0: the traditional route, either binary convention at random.
        [Fact]
        public void GenerateChildren_NonBinaryChildOnTraditionalRoute_TakesOneOfTheTwoBinaryConventions()
        {
            var generator = CreateGeneratorWithNextDouble(0.4514,
                nonBinaryProbability: 100,
                namingRoute: 0);
            var pair = CreateFertilePair();

            var result = generator.GenerateChildren(new List<Pair> { pair },
                0,
                0,
                new List<Person> { pair.FPerson, pair.MPerson },
                new DateTime(1, 1, 1));

            var fullName = result.Children[0].Name.FullName;
            Assert.True(fullName == "JaneSmith" || fullName == "JohnDoe",
                $"Expected one of the two binary name conventions but got '{fullName}'");
        }

        // Routes 1 and 2: the dedicated non-binary conventions, with either parent's
        // part able to come first. The child's own Prefix/Suffix must decompose the
        // FullName so their children can inherit parts.
        [Theory]
        [InlineData(1, 0, "JaneJohn", "Jane", "John")]  // prefix + prefix, FPerson first
        [InlineData(1, 1, "JohnJane", "John", "Jane")]  // prefix + prefix, MPerson first
        [InlineData(2, 0, "DoeSmith", "Doe", "Smith")]  // suffix + suffix, FPerson first
        [InlineData(2, 1, "SmithDoe", "Smith", "Doe")]  // suffix + suffix, MPerson first
        public void GenerateChildren_NonBinaryChildOnDedicatedRoute_CombinesParentNameParts(int namingRoute,
            int partOrder,
            string expectedFullName,
            string expectedPrefix,
            string expectedSuffix)
        {
            var generator = CreateGeneratorWithNextDouble(0.4514,
                nonBinaryProbability: 100,
                namingRoute: namingRoute,
                partOrder: partOrder);
            var pair = CreateFertilePair();

            var result = generator.GenerateChildren(new List<Pair> { pair },
                0,
                0,
                new List<Person> { pair.FPerson, pair.MPerson },
                new DateTime(1, 1, 1));

            Assert.Equal(expectedFullName, result.Children[0].Name.FullName);
            Assert.Equal(expectedPrefix, result.Children[0].Name.Prefix);
            Assert.Equal(expectedSuffix, result.Children[0].Name.Suffix);
        }

        [Fact]
        public void GenerateChildren_WithCustomFertilityCooldownYears_ExcludesPairBelowConfiguredThreshold()
        {
            // Same nextDoubleValue as the passing Female case above, but the pair's TimeFromLastChild.Year (3)
            // is below this generator's configured threshold (5), so the pair must be excluded regardless of the dice roll
            var generator = CreateGeneratorWithNextDouble(0.4514,
                fertilityCooldownYears: 5);
            var pair = CreateFertilePair();

            var result = generator.GenerateChildren(new List<Pair> { pair },
                0,
                0,
                new List<Person> { pair.FPerson, pair.MPerson },
                new DateTime(1, 1, 1));

            Assert.Empty(result.Children);
        }

        private PersonGenerator CreateGeneratorWithNextDouble(double nextDoubleValue,
            int fertilityCooldownYears = 3,
            double transgenderProbability = 0,
            double nonBinaryProbability = 0,
            int namingRoute = 0,
            int partOrder = 0)
        {
            var mockDiceGenerator = Substitute.For<IDiceGenerator>();
            mockDiceGenerator.Create(Arg.Any<int>(), Arg.Any<DateTime>()).Returns(new Random(14)); // seed 14: childChance = 5
            mockDiceGenerator.NextDouble(Arg.Any<Random>()).Returns(nextDoubleValue);
            mockDiceGenerator.Next(Arg.Any<Random>(), Arg.Any<int>(), Arg.Any<int>()).Returns(50);
            mockDiceGenerator.Next(Arg.Any<Random>(), Arg.Any<int>(), Arg.Is<int>(x => x == 3)).Returns(namingRoute);
            mockDiceGenerator.Next(Arg.Any<Random>(), Arg.Any<int>(), Arg.Is<int>(x => x == 2)).Returns(partOrder);
            return new PersonGenerator(mockDiceGenerator,
                _gameState,
                Options.Create(new AppSettings
                {
                    FertilityCooldownYears = fertilityCooldownYears,
                    TransgenderProbability = transgenderProbability,
                    NonBinaryProbability = nonBinaryProbability
                }));
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