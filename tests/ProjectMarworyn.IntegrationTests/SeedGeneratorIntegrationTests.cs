using Microsoft.Extensions.Options;
using ProjectMarworyn.Core;
using ProjectMarworyn.Core.Configuration;
using ProjectMarworyn.Core.Generators;
using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.IntegrationTests;

public class SeedGeneratorIntegrationTests
{
    private readonly SeedGenerator _seedGenerator;

    public SeedGeneratorIntegrationTests()
    {
        var gameState = new GameState();
        var options = Options.Create(new AppSettings
        {
            SeedWordFilePath = ConfigFileHelper.GetPath("Configuration/SeedWord.json")
        });
        var fileManager = new FileManager(options);
        _seedGenerator = new SeedGenerator(fileManager, gameState);
    }

    [Fact]
    public void GetThreeWords_WithRealSeedWordFile_ReturnsExactlyThreeWords()
    {
        var words = _seedGenerator.GetThreeWords();

        Assert.Equal(3, words.Count);
    }

    [Fact]
    public void GetThreeWords_WithRealSeedWordFile_AllWordsAreNonEmpty()
    {
        var words = _seedGenerator.GetThreeWords();

        Assert.All(words, w => Assert.False(string.IsNullOrEmpty(w.Word)));
    }

    [Fact]
    public void CreateWorldSeed_SameWords_ReturnsSameSeed()
    {
        var words = new List<SeedWord>
        {
            new() { Id = 1, Word = "mountain" },
            new() { Id = 2, Word = "river" },
            new() { Id = 3, Word = "forest" }
        };

        var seed1 = _seedGenerator.CreateWorldSeed(words);
        var seed2 = _seedGenerator.CreateWorldSeed(words);

        Assert.Equal(seed1, seed2);
    }

    [Fact]
    public void CreateWorldSeed_DifferentWords_ReturnsDifferentSeeds()
    {
        var wordsA = new List<SeedWord>
        {
            new() { Id = 1, Word = "mountain" },
            new() { Id = 2, Word = "river" },
            new() { Id = 3, Word = "forest" }
        };
        var wordsB = new List<SeedWord>
        {
            new() { Id = 1, Word = "ocean" },
            new() { Id = 2, Word = "valley" },
            new() { Id = 3, Word = "desert" }
        };

        var seedA = _seedGenerator.CreateWorldSeed(wordsA);
        var seedB = _seedGenerator.CreateWorldSeed(wordsB);

        Assert.NotEqual(seedA, seedB);
    }

    [Fact]
    public void CreateWorldSeed_EmptyWordList_ReturnsZero()
    {
        var seed = _seedGenerator.CreateWorldSeed(new List<SeedWord>());

        Assert.Equal(0, seed);
    }

    [Fact]
    public void CreateWorldSeed_WordsLoggedToGameState()
    {
        var gameState = new GameState();
        var options = Options.Create(new AppSettings
        {
            SeedWordFilePath = ConfigFileHelper.GetPath("Configuration/SeedWord.json")
        });
        var fileManager = new FileManager(options);
        var seedGenerator = new SeedGenerator(fileManager, gameState);
        var words = new List<SeedWord>
        {
            new() { Id = 1, Word = "hill" },
            new() { Id = 2, Word = "lake" },
            new() { Id = 3, Word = "pine" }
        };

        seedGenerator.CreateWorldSeed(words);

        Assert.Contains(gameState.Text, t => t.Contains("HILL-LAKE-PINE"));
    }
}