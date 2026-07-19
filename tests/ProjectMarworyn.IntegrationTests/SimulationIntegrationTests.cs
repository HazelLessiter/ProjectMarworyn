using Microsoft.Extensions.DependencyInjection;
using ProjectMarworyn.Core.Configuration;
using ProjectMarworyn.Core.Extensions;
using ProjectMarworyn.Core.Managers;
using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.IntegrationTests;

public class SimulationIntegrationTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly ISimulationManager _simulationManager;
    private readonly SimulationClock _clock;
    private readonly GameState _gameState;

    public SimulationIntegrationTests()
    {
        var services = new ServiceCollection();
        services.Configure<AppSettings>(options =>
        {
            options.InitialPeopleFilePath = ConfigFileHelper.GetPath("Configuration/InitialPeople.json");
            options.SeedWordFilePath = ConfigFileHelper.GetPath("Configuration/SeedWord.json");
            //A single catch-all bracket keeps the population alive for the clock/logging assertions
            options.DeathBrackets = new List<DeathBracket>
            {
                new DeathBracket { DailyDeathChance = 0.05 }
            };
        });
        services.AddCoreServices();

        _serviceProvider = services.BuildServiceProvider();
        _simulationManager = _serviceProvider.GetRequiredService<ISimulationManager>();
        _clock = _serviceProvider.GetRequiredService<SimulationClock>();
        _gameState = _serviceProvider.GetRequiredService<GameState>();
    }

    public void Dispose() => _serviceProvider.Dispose();

    [Fact]
    public void Start_WithRealConfigFiles_ClockIsRunning()
    {
        _simulationManager.Start();

        Assert.True(_clock.IsRunning);
    }

    [Fact]
    public void Start_WithRealConfigFiles_WorldSeedIsLogged()
    {
        _simulationManager.Start();

        Assert.Contains(_gameState.Text, t => t.StartsWith("World seed created:"));
    }

    [Fact]
    public void ProgressDay_OneDay_ClockTickCountIsOne()
    {
        _simulationManager.Start();
        _simulationManager.ProgressDay();

        Assert.Equal(1, _clock.TickCount);
    }

    [Fact]
    public void ProgressDay_OneDay_GameStateContainsDate()
    {
        _simulationManager.Start();
        _simulationManager.ProgressDay();

        Assert.Contains(_gameState.Text, t => t.StartsWith("Date:"));
    }

    [Fact]
    public void ProgressDay_MultipleDays_ClockAdvancesCorrectly()
    {
        _simulationManager.Start();

        _simulationManager.ProgressDay();
        _simulationManager.ProgressDay();
        _simulationManager.ProgressDay();

        Assert.Equal(3, _clock.TickCount);
    }

    [Fact]
    public void ProgressDay_365Days_NewYearIsLogged()
    {
        _simulationManager.Start();

        for (int i = 0; i < 365; i++)
        {
            _simulationManager.ProgressDay();
            if (!_clock.IsRunning)
                return;
        }

        Assert.Contains(_gameState.Text, t => t == "Happy new year!");
    }

    // State-based counterpart to the SimulationManagerTests orchestration test: with the real
    // engines running, children being born is observable through GameState. Roughly a third of
    // the initial population starts fertility-eligible and each fertile pair has a ~40% daily
    // birth chance, so a birthless year is astronomically unlikely regardless of seed.
    // GameState.Text is cleared every day, so the check runs inside the loop.
    [Fact]
    public void ProgressDay_OverAYear_ChildrenAreBorn()
    {
        _simulationManager.Start();

        for (int i = 0; i < 365; i++)
        {
            _simulationManager.ProgressDay();
            if (_gameState.Text.Any(t => t.Contains("was born to")))
                return;
            if (!_clock.IsRunning)
                break;
        }

        Assert.Fail("Expected at least one child to be born within a year");
    }
}