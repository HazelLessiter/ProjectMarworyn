using ProjectMarworyn.Generators;
using ProjectMarworyn.Models;
using ProjectMarworyn.Tests.Mocks;

namespace ProjectMarworyn.Tests
{
    public class SeedGeneratorTests
    {
        private readonly MockFileManager _mockFileManager;
        private readonly MockConsoleService _mockConsoleService;
        private readonly SeedGenerator _seedGenerator;

        public SeedGeneratorTests()
        {
            _mockFileManager = new MockFileManager();
            _mockConsoleService = new MockConsoleService();
            _seedGenerator = new SeedGenerator(_mockFileManager,
                _mockConsoleService);
        }

        private static List<SeedWord> MakeSeedWords(int count) => Enumerable.Range(1, count)
            .Select(i => new SeedWord { Id = i, Word = $"Word{i}" })
            .ToList();

        [Fact]
        public void GetThreeWords_ReturnsNonNullList()
        {
            _mockFileManager.SeedWordsToReturn = MakeSeedWords(10);

            var result = _seedGenerator.GetThreeWords();

            Assert.NotNull(result);
        }

        [Fact]
        public void GetThreeWords_WhenSeedWordsNull_ReturnsEmptyList()
        {
            _mockFileManager.SeedWordsToReturn = null;

            var result = _seedGenerator.GetThreeWords();

            Assert.Empty(result);
        }

        [Fact]
        public void GetThreeWords_WhenSeedWordsEmpty_ReturnsEmptyList()
        {
            _mockFileManager.SeedWordsToReturn = new List<SeedWord>();

            var result = _seedGenerator.GetThreeWords();

            Assert.Empty(result);
        }

        [Fact]
        public void GetThreeWords_ReturnsOnlyWordsFromSeedWordFile()
        {
            _mockFileManager.SeedWordsToReturn = MakeSeedWords(10);

            var result = _seedGenerator.GetThreeWords();

            Assert.All(result, w => Assert.Contains(w, _mockFileManager.SeedWordsToReturn));
        }

        [Fact]
        public void CreateWorldSeed_WithThreeWords_ReturnsInt()
        {
            var seedWords = new List<SeedWord>
            {
                new SeedWord { Id = 1, Word = "Oak" },
                new SeedWord { Id = 2, Word = "Wren" },
                new SeedWord { Id = 3, Word = "Moss" }
            };

            var result = _seedGenerator.CreateWorldSeed(seedWords);

            Assert.IsType<int>(result);
        }

        [Fact]
        public void CreateWorldSeed_SameWords_ReturnsSameSeed()
        {
            var seedWords = new List<SeedWord>
            {
                new SeedWord { Id = 1, Word = "Oak" },
                new SeedWord { Id = 2, Word = "Wren" },
                new SeedWord { Id = 3, Word = "Moss" }
            };

            var first = _seedGenerator.CreateWorldSeed(seedWords);
            var second = _seedGenerator.CreateWorldSeed(seedWords);

            Assert.Equal(first, second);
        }

        [Fact]
        public void CreateWorldSeed_DifferentWords_ReturnsDifferentSeed()
        {
            var seedWordsA = new List<SeedWord>
            {
                new SeedWord { Id = 1, Word = "Oak" },
                new SeedWord { Id = 2, Word = "Wren" },
                new SeedWord { Id = 3, Word = "Moss" }
            };
            var seedWordsB = new List<SeedWord>
            {
                new SeedWord { Id = 1, Word = "Birch" },
                new SeedWord { Id = 2, Word = "Otter" },
                new SeedWord { Id = 3, Word = "Frost" }
            };

            var seedA = _seedGenerator.CreateWorldSeed(seedWordsA);
            var seedB = _seedGenerator.CreateWorldSeed(seedWordsB);

            Assert.NotEqual(seedA, seedB);
        }

        [Fact]
        public void CreateWorldSeed_WordsAreCaseInsensitive()
        {
            var lower = new List<SeedWord>
            {
                new SeedWord { Id = 1, Word = "oak" },
                new SeedWord { Id = 2, Word = "wren" },
                new SeedWord { Id = 3, Word = "moss" }
            };
            var upper = new List<SeedWord>
            {
                new SeedWord { Id = 1, Word = "OAK" },
                new SeedWord { Id = 2, Word = "WREN" },
                new SeedWord { Id = 3, Word = "MOSS" }
            };

            var lowerSeed = _seedGenerator.CreateWorldSeed(lower);
            var upperSeed = _seedGenerator.CreateWorldSeed(upper);

            Assert.Equal(lowerSeed, upperSeed);
        }

        [Fact]
        public void CreateWorldSeed_WritesWorldSeedToConsole()
        {
            var seedWords = new List<SeedWord>
            {
                new SeedWord { Id = 1, Word = "Oak" },
                new SeedWord { Id = 2, Word = "Wren" },
                new SeedWord { Id = 3, Word = "Moss" }
            };

            _seedGenerator.CreateWorldSeed(seedWords);

            Assert.Single(_mockConsoleService.Lines);
            Assert.Contains("OAK-WREN-MOSS", _mockConsoleService.Lines[0]);
        }
    }
}
