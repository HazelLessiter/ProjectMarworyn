using ProjectMarworyn.Models;

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
        public void Initialise_WithEmptyList_ReturnsGenerationWithEmptyNames()
        {
            var names = new List<Name>();

            var result = _generationManager.Initialise(names);

            Assert.Empty(result.Names);
        }

        [Fact]
        public void Initialise_WithEmptyList_ReturnsIterationZero()
        {
            var names = new List<Name>();

            var result = _generationManager.Initialise(names);

            Assert.Equal(0, result.Iteration);
        }

        [Fact]
        public void Initialise_WithNames_ReturnsGenerationWithSameNames()
        {
            var names = new List<Name>
            {
                new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe", Gender = Gender.Female },
                new Name { FullName = "JohnSmith", Prefix = "John", Suffix = "Smith", Gender = Gender.Male }
            };

            var result = _generationManager.Initialise(names);

            Assert.Equal(2, result.Names.Count);
            Assert.Equal(names, result.Names);
        }

        [Fact]
        public void Initialise_WithNames_ReturnsIterationZero()
        {
            var names = new List<Name>
            {
                new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe", Gender = Gender.Female }
            };

            var result = _generationManager.Initialise(names);

            Assert.Equal(0, result.Iteration);
        }

        [Fact]
        public void Initialise_WithSingleName_ReturnsGenerationWithSingleName()
        {
            var names = new List<Name>
            {
                new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe", Gender = Gender.Female }
            };

            var result = _generationManager.Initialise(names);

            Assert.Single(result.Names);
            Assert.Equal("JaneDoe", result.Names[0].FullName);
        }

        [Fact]
        public void Initialise_WithMultipleNames_PreservesNameProperties()
        {
            var names = new List<Name>
            {
                new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe", Gender = Gender.Female },
                new Name { FullName = "JohnSmith", Prefix = "John", Suffix = "Smith", Gender = Gender.Male },
                new Name { FullName = "AliceWonder", Prefix = "Alice", Suffix = "Wonder", Gender = Gender.Female }
            };

            var result = _generationManager.Initialise(names);

            Assert.Equal(3, result.Names.Count);
            Assert.Equal("Jane", result.Names[0].Prefix);
            Assert.Equal("Doe", result.Names[0].Suffix);
            Assert.Equal(Gender.Female, result.Names[0].Gender);
            Assert.Equal("John", result.Names[1].Prefix);
            Assert.Equal("Smith", result.Names[1].Suffix);
            Assert.Equal(Gender.Male, result.Names[1].Gender);
        }

        [Fact]
        public void Initialise_ReturnsNonNullGeneration()
        {
            var names = new List<Name>();

            var result = _generationManager.Initialise(names);

            Assert.NotNull(result);
        }

        [Fact]
        public void Initialise_ReturnsNonNullNamesList()
        {
            var names = new List<Name>();

            var result = _generationManager.Initialise(names);

            Assert.NotNull(result.Names);
        }

        [Fact]
        public void Initialise_WithLargeNameList_ReturnsAllNames()
        {
            var names = new List<Name>();
            for (int i = 0; i < 100; i++)
            {
                names.Add(new Name
                {
                    FullName = $"Name{i}",
                    Prefix = $"Prefix{i}",
                    Suffix = $"Suffix{i}",
                    Gender = i % 2 == 0 ? Gender.Female : Gender.Male
                });
            }

            var result = _generationManager.Initialise(names);

            Assert.Equal(100, result.Names.Count);
        }
    }
}