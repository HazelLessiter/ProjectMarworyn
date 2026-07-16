using ProjectMarworyn.Core;
using ProjectMarworyn.Core.Generators;
using ProjectMarworyn.Core.Managers;
using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Models.Enums;

namespace ProjectMarworyn.IntegrationTests;

public class PopulationPipelineTests
{
    private readonly GameState _gameState;
    private readonly IDiceGenerator _diceGenerator;
    private readonly AgeProcessor _ageProcessor;
    private readonly DeathEngine _deathEngine;
    private readonly PairingEngine _pairingEngine;
    private readonly PersonGenerator _personGenerator;
    private readonly GenerationManager _generationManager;

    public PopulationPipelineTests()
    {
        _gameState = new GameState();
        _diceGenerator = new DiceGenerator();
        _ageProcessor = new AgeProcessor(_gameState);
        _deathEngine = new DeathEngine(_diceGenerator, _gameState);
        _pairingEngine = new PairingEngine(_diceGenerator, _gameState);
        _personGenerator = new PersonGenerator(_diceGenerator, _gameState);
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

        var result = _ageProcessor.Age(people);

        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
    }

    [Fact]
    public void AgeProcessor_PersonAtYearBoundary_AgeIsIncremented()
    {
        var person = CreatePerson(1, age: 20, timeLived: new DateTime(1, 12, 31));

        var result = _ageProcessor.Age(new List<Person> { person });

        Assert.Equal(21, result[0].Age);
    }

    [Fact]
    public void AgeProcessor_PersonNotAtYearBoundary_AgeIsUnchanged()
    {
        var person = CreatePerson(1, age: 20, timeLived: new DateTime(1, 6, 15));

        var result = _ageProcessor.Age(new List<Person> { person });

        Assert.Equal(20, result[0].Age);
    }

    [Fact]
    public void AgeAndDeathPipeline_LargePopulation_SurvivorsAreSubsetOfInput()
    {
        var worldSeed = 42;
        var people = Enumerable.Range(0, 50)
            .Select(i => CreatePerson(i, age: 30))
            .ToList();
        var generation = _generationManager.Initialise(people);

        var aged = _ageProcessor.Age(people);

        var dice = _diceGenerator.Create(worldSeed,
            new DateTime(1, 1, 1));

        var result = _deathEngine.ProcessDeaths(aged,
            generation,
            worldSeed,
            dice);

        Assert.NotNull(result.People);
        Assert.True(result.People.Count <= aged.Count);
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
        var generation = _generationManager.Initialise(people);

        var dice = _diceGenerator.Create(worldSeed,
            new DateTime(1, 1, 1));

        var result = _deathEngine.ProcessDeaths(people,
            generation,
            worldSeed,
            dice);

        Assert.True(result.People.Count < people.Count);
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

        var dice = _diceGenerator.Create(worldSeed,
            new DateTime(1, 1, 1));

        var (pairs, _) = _pairingEngine.GeneratePairs(people,
            new List<Pair>(),
            worldSeed,
            dice);

        Assert.NotEmpty(pairs);
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

        var dice = _diceGenerator.Create(worldSeed,
            new DateTime(1, 1, 1));

        var (pairs, _) = _pairingEngine.GeneratePairs(people,
            new List<Pair>(),
            worldSeed,
            dice);

        Assert.Empty(pairs);
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

        var dice = _diceGenerator.Create(worldSeed,
            new DateTime(1, 1, 1));

        var (pairs, _) = _pairingEngine.GeneratePairs(people,
            new List<Pair>(),
            worldSeed,
            dice);

        Assert.Empty(pairs);
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

        var dice = _diceGenerator.Create(worldSeed,
            new DateTime(1, 1, 1));

        var (pairs, updatedPeople) = _pairingEngine.GeneratePairs(people,
            new List<Pair>(),
            worldSeed,
            dice);
        var (children, _) = _personGenerator.GenerateChildren(pairs,
            worldSeed,
            people.Max(p => p.Id),
            updatedPeople,
            new DateTime(1, 1, 1));

        Assert.NotNull(children);
        Assert.NotEmpty(children);
    }

    [Fact]
    public void GenerationManager_Initialise_SetsIterationToZero()
    {
        var people = new List<Person> { CreatePerson(1) };

        var generation = _generationManager.Initialise(people);

        Assert.Equal(0, generation.Iteration);
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

    private static Person CreatePerson(int id, bool isAlive = true, int age = 30,
        Biosex biosex = Biosex.Female, DateTime? timeLived = null) =>
        new()
        {
            Id = id,
            IsAlive = isAlive,
            Age = age,
            Biosex = biosex,
            Gender = biosex == Biosex.Male ? Gender.Male : Gender.Female,
            Name = new Name { FullName = $"Person{id}", Prefix = "Person", Suffix = $"{id}" },
            TimeLived = timeLived ?? new DateTime(1, 1, 1),
            TimeFromLastChild = new DateTime(1, 1, 1),
            WillHaveChildren = true,
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
            TimeLived = new DateTime(1, 1, 1),
            TimeFromLastChild = new DateTime(3, 1, 1),
            WillHaveChildren = true,
            HasPair = false
        };
}
