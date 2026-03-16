using ProjectMarworyn.Models;
using ProjectMarworyn.Tests.Mocks;

namespace ProjectMarworyn.Tests
{
    public class NameProcessorTests
    {
        private readonly NameProcessor _nameProcessor;

        public NameProcessorTests()
        {
            _nameProcessor = new NameProcessor(new MockOutputService());
        }

        [Fact]
        public void ListNumberOfNamesByGender_WithEmptyList_DoesNotThrow()
        {
            var names = new List<Name>();

            var exception = Record.Exception(() => _nameProcessor.ListNumberOfNamesByGender(names));

            Assert.Null(exception);
        }

        [Fact]
        public void ListNumberOfNamesByGender_WithNames_DoesNotThrow()
        {
            var names = new List<Name>
            {
                new Name { FullName = "Female1", Gender = Gender.Female },
                new Name { FullName = "Male1", Gender = Gender.Male }
            };

            var exception = Record.Exception(() => _nameProcessor.ListNumberOfNamesByGender(names));

            Assert.Null(exception);
        }

        [Fact]
        public void PairNames_WithEmptyList_ReturnsEmptyPairs()
        {
            var names = new List<Name>();

            var result = _nameProcessor.PairNames(names);

            Assert.Empty(result);
        }

        [Fact]
        public void PairNames_WithOneFemaleAndOneMale_ReturnsOnePair()
        {
            var names = new List<Name>
            {
                new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe", Gender = Gender.Female },
                new Name { FullName = "JohnSmith", Prefix = "John", Suffix = "Smith", Gender = Gender.Male }
            };

            var result = _nameProcessor.PairNames(names);

            Assert.Single(result);
        }

        [Fact]
        public void PairNames_WithOneFemaleAndOneMale_PairContainsCorrectNames()
        {
            var names = new List<Name>
            {
                new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe", Gender = Gender.Female },
                new Name { FullName = "JohnSmith", Prefix = "John", Suffix = "Smith", Gender = Gender.Male }
            };

            var result = _nameProcessor.PairNames(names);

            Assert.Equal("JaneDoe", result[0].FName.FullName);
            Assert.Equal("JohnSmith", result[0].MName.FullName);
        }

        [Fact]
        public void PairNames_WithMoreFemalesThanMales_ReturnsPairsUpToMaleCount()
        {
            var names = new List<Name>
            {
                new Name { FullName = "Female1", Prefix = "F1", Suffix = "S1", Gender = Gender.Female },
                new Name { FullName = "Female2", Prefix = "F2", Suffix = "S2", Gender = Gender.Female },
                new Name { FullName = "Female3", Prefix = "F3", Suffix = "S3", Gender = Gender.Female },
                new Name { FullName = "Male1", Prefix = "M1", Suffix = "MS1", Gender = Gender.Male }
            };

            var result = _nameProcessor.PairNames(names);

            Assert.Single(result);
        }

        [Fact]
        public void PairNames_WithMoreMalesThanFemales_ReturnsPairsUpToFemaleCount()
        {
            var names = new List<Name>
            {
                new Name { FullName = "Female1", Prefix = "F1", Suffix = "S1", Gender = Gender.Female },
                new Name { FullName = "Male1", Prefix = "M1", Suffix = "MS1", Gender = Gender.Male },
                new Name { FullName = "Male2", Prefix = "M2", Suffix = "MS2", Gender = Gender.Male }
            };

            var result = _nameProcessor.PairNames(names);

            Assert.Single(result);
        }

        [Fact]
        public void PairNames_WithOnlyFemales_ReturnsEmptyPairs()
        {
            var names = new List<Name>
            {
                new Name { FullName = "Female1", Prefix = "F1", Suffix = "S1", Gender = Gender.Female },
                new Name { FullName = "Female2", Prefix = "F2", Suffix = "S2", Gender = Gender.Female }
            };

            var result = _nameProcessor.PairNames(names);

            Assert.Empty(result);
        }

        [Fact]
        public void PairNames_WithOnlyMales_ReturnsEmptyPairs()
        {
            var names = new List<Name>
            {
                new Name { FullName = "Male1", Prefix = "M1", Suffix = "MS1", Gender = Gender.Male },
                new Name { FullName = "Male2", Prefix = "M2", Suffix = "MS2", Gender = Gender.Male }
            };

            var result = _nameProcessor.PairNames(names);

            Assert.Empty(result);
        }

        [Theory]
        [InlineData(2, 2)]
        [InlineData(4, 4)]
        [InlineData(1, 3)]
        public void PairNames_ReturnsExpectedNumberOfPairs(int maleCount, int femaleCount)
        {
            var names = new List<Name>();
            for (int i = 0; i < femaleCount; i++)
                names.Add(new Name { FullName = $"Female{i}", Prefix = $"F{i}", Suffix = $"S{i}", Gender = Gender.Female });
            for (int i = 0; i < maleCount; i++)
                names.Add(new Name { FullName = $"Male{i}", Prefix = $"M{i}", Suffix = $"MS{i}", Gender = Gender.Male });

            var result = _nameProcessor.PairNames(names);

            Assert.Equal(Math.Min(maleCount, femaleCount), result.Count);
        }
    }
}