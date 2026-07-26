using Microsoft.Extensions.Options;
using ProjectMarworyn.Core;
using ProjectMarworyn.Core.Configuration;
using ProjectMarworyn.Core.Generators;
using ProjectMarworyn.Core.Managers;
using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Models.Enums;

namespace ProjectMarworyn.IntegrationTests;

public class PopulationPipelineTests
{
    private readonly GameState _gameState;
    private readonly IDiceGenerator _diceGenerator;
    private readonly IAttractionCalculator _attractionCalculator;
    private readonly AgeProcessor _ageProcessor;
    private readonly DeathEngine _deathEngine;
    private readonly PairingEngine _pairingEngine;
    private readonly PersonGenerator _personGenerator;
    private readonly GenerationManager _generationManager;

    public PopulationPipelineTests()
    {
        _gameState = new GameState();
        _diceGenerator = new DiceGenerator();
        _ageProcessor = new AgeProcessor(_gameState,
            Options.Create(new AppSettings { FertilityCooldownYears = 2 }));
        _deathEngine = new DeathEngine(_diceGenerator,
            _gameState,
            Options.Create(new AppSettings { DeathBrackets = CreateDefaultDeathBrackets() }));
        _attractionCalculator = new AttractionCalculator();
        _pairingEngine = new PairingEngine(_diceGenerator,
            _attractionCalculator,
            _gameState);
        _personGenerator = new PersonGenerator(_diceGenerator,
            _gameState,
            Options.Create(new AppSettings
            {
                FertilityCooldownYears = 2,
                OrientationWeights = CreateDefaultOrientationWeights()
            }));
        _generationManager = new GenerationManager();
    }

    [Fact]
    public void AgeProcessor_DeadPerson_IsExcludedFromOutput()
    {
        var people = new List<Person>
        {
            CreatePerson(1, isAlive: true),
            CreatePerson(2, isAlive: false)
        };

        var result = _ageProcessor.Age(people,
            new DateTime(1, 1, 1));

        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
    }

    [Fact]
    public void AgeProcessor_OnBirthday_AgeIsIncremented()
    {
        var person = CreatePerson(1, age: 20);

        var result = _ageProcessor.Age(new List<Person> { person },
            new DateTime(1, 6, 15));

        Assert.Equal(21, result[0].Age);
    }

    [Fact]
    public void AgeProcessor_NotOnBirthday_AgeIsUnchanged()
    {
        var person = CreatePerson(1, age: 20);

        var result = _ageProcessor.Age(new List<Person> { person },
            new DateTime(1, 6, 14));

        Assert.Equal(20, result[0].Age);
    }

    [Fact]
    public void AgeAndDeathPipeline_LargePopulation_SurvivorsAreSubsetOfInput()
    {
        var worldSeed = 42;
        var people = Enumerable.Range(0, 50)
            .Select(i => CreatePerson(i, age: 30))
            .ToList();

        var aged = _ageProcessor.Age(people,
            new DateTime(1, 1, 1));

        var survivors = _deathEngine.ProcessDeaths(aged,
            worldSeed,
            new DateTime(1, 1, 1));

        Assert.NotNull(survivors);
        Assert.True(survivors.Count <= aged.Count);
    }

    [Fact]
    public void DeathEngine_VeryOldPopulation_HighMortalityOverTime()
    {
        var worldSeed = 42;
        // 1000 samples at the Hundred bracket's 2.5% daily chance keeps a zero-death outcome
        // astronomically unlikely (~0.975^1000), so the assertion holds regardless of seed
        var people = Enumerable.Range(0, 1000)
            .Select(i => CreatePerson(i, age: 100))
            .ToList();

        var survivors = _deathEngine.ProcessDeaths(people,
            worldSeed,
            new DateTime(1, 1, 1));

        Assert.True(survivors.Count < people.Count);
    }

    [Fact]
    public void PairingEngine_EligibleAdultsOfBothSexes_PairsAreCreated()
    {
        var worldSeed = 42;
        var people = new List<Person>
        {
            CreatePerson(1, biosex: Biosex.Female, age: 25),
            CreatePerson(2, biosex: Biosex.Male, age: 25),
            CreatePerson(3, biosex: Biosex.Female, age: 28),
            CreatePerson(4, biosex: Biosex.Male, age: 28),
        };

        var result = _pairingEngine.GeneratePairs(people,
            new List<Pair>(),
            worldSeed,
            new DateTime(1, 1, 1));

        Assert.NotEmpty(result);
    }

    [Fact]
    public void PairingEngine_NoMales_NoPairsCreated()
    {
        var worldSeed = 42;
        var people = new List<Person>
        {
            CreatePerson(1, biosex: Biosex.Female, age: 25),
            CreatePerson(2, biosex: Biosex.Female, age: 28),
        };

        var result = _pairingEngine.GeneratePairs(people,
            new List<Pair>(),
            worldSeed,
            new DateTime(1, 1, 1));

        Assert.Empty(result);
    }

    [Fact]
    public void PairingEngine_UnderagePeople_NoPairsCreated()
    {
        var worldSeed = 42;
        var people = new List<Person>
        {
            CreatePerson(1, biosex: Biosex.Female, age: 16),
            CreatePerson(2, biosex: Biosex.Male, age: 17),
        };

        var result = _pairingEngine.GeneratePairs(people,
            new List<Pair>(),
            worldSeed,
            new DateTime(1, 1, 1));

        Assert.Empty(result);
    }

    [Fact]
    public void PairingAndChildGeneration_ManyFertilePairs_ChildrenAreProduced()
    {
        var worldSeed = 1;
        var people = Enumerable.Range(0, 20)
            .SelectMany(i => new[]
            {
                CreateFertilePerson(i * 2, Biosex.Female),
                CreateFertilePerson(i * 2 + 1, Biosex.Male)
            })
            .ToList();

        var pairingResult = _pairingEngine.GeneratePairs(people,
            new List<Pair>(),
            worldSeed,
            new DateTime(1, 1, 1));
        var childResult = _personGenerator.GenerateChildren(pairingResult,
            worldSeed,
            people.Max(p => p.Id),
            new DateTime(1, 1, 1));

        Assert.NotNull(childResult);
        Assert.NotEmpty(childResult);
    }

    [Fact]
    public void GenerationManager_CheckForExtinction_OnePerson_ReturnsTrue()
    {
        var people = new List<Person> { CreatePerson(1) };

        Assert.True(_generationManager.CheckForExtinction(people));
    }

    [Fact]
    public void GenerationManager_CheckForExtinction_TwoPeople_ReturnsFalse()
    {
        var people = new List<Person> { CreatePerson(1), CreatePerson(2) };

        Assert.False(_generationManager.CheckForExtinction(people));
    }

    private static List<DeathBracket> CreateDefaultDeathBrackets() =>
        new()
        {
            new DeathBracket { MaxAge = 9, DailyDeathChance = 0.1 },
            new DeathBracket { MaxAge = 19, DailyDeathChance = 0.01 },
            new DeathBracket { MaxAge = 29, DailyDeathChance = 0.02 },
            new DeathBracket { MaxAge = 39, DailyDeathChance = 0.05 },
            new DeathBracket { MaxAge = 49, DailyDeathChance = 0.1 },
            new DeathBracket { MaxAge = 59, DailyDeathChance = 0.15 },
            new DeathBracket { MaxAge = 69, DailyDeathChance = 0.2 },
            new DeathBracket { MaxAge = 79, DailyDeathChance = 0.25 },
            new DeathBracket { MaxAge = 89, DailyDeathChance = 0.5 },
            new DeathBracket { MaxAge = 99, DailyDeathChance = 1.0 },
            new DeathBracket { DailyDeathChance = 2.5 }
        };

    private static List<OrientationWeight> CreateDefaultOrientationWeights() =>
        new()
        {
            new OrientationWeight { Orientation = Orientation.Heterosexual, Weight = 96.81 },
            new OrientationWeight { Orientation = Orientation.Homosexual, Weight = 1.5 },
            new OrientationWeight { Orientation = Orientation.Bisexual, Weight = 1.3 },
            new OrientationWeight { Orientation = Orientation.Pansexual, Weight = 0.23 },
            new OrientationWeight { Orientation = Orientation.Asexual, Weight = 0.06 },
            new OrientationWeight { Orientation = Orientation.Aromantic, Weight = 0.05 },
            new OrientationWeight { Orientation = Orientation.Aroace, Weight = 0.05 }
        };

    private static Person CreatePerson(int id, bool isAlive = true, int age = 30,
        Biosex biosex = Biosex.Female, int birthMonth = 6, int birthDay = 15) =>
        new()
        {
            Id = id,
            IsAlive = isAlive,
            Age = age,
            Biosex = biosex,
            Gender = biosex == Biosex.Male ? Gender.Male : Gender.Female,
            Name = new Name { FullName = $"Person{id}", Prefix = "Person", Suffix = $"{id}" },
            BirthMonth = birthMonth,
            BirthDay = birthDay,
            DaysSinceLastChild = 0,
            WillHaveChildren = true,
            WillPair = true,
            IsFertile = true,
            HasPair = false
        };

    private static Person CreateFertilePerson(int id, Biosex biosex) =>
        new()
        {
            Id = id,
            IsAlive = true,
            Age = 25,
            Biosex = biosex,
            Gender = biosex == Biosex.Male ? Gender.Male : Gender.Female,
            Name = new Name { FullName = $"Person{id}", Prefix = "Person", Suffix = $"{id}" },
            BirthMonth = 6,
            BirthDay = 15,
            DaysSinceLastChild = 730,
            WillHaveChildren = true,
            WillPair = true,
            IsFertile = true,
            HasPair = false
        };
}