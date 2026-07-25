using NSubstitute;
using ProjectMarworyn.Core;
using ProjectMarworyn.Core.Generators;
using ProjectMarworyn.Core.Managers;
using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.UnitTests
{
    public class SimulationManagerTests
    {
        private readonly SimulationManager _simulationManager;
        private readonly IPersonGenerator _mockPersonGenerator;
        private readonly IDeathEngine _mockDeathEngine;
        private readonly IAgeProcessor _mockAgeProcessor;
        private readonly List<Person> _people;

        public SimulationManagerTests()
        {
            _people = new List<Person>
            {
                CreatePerson(1),
                CreatePerson(2)
            };

            var mockFileManager = Substitute.For<IFileManager>();
            mockFileManager.ReadInitialPersonFile().Returns(new List<InitialPerson>());

            var mockGenerationManager = Substitute.For<IGenerationManager>();
            mockGenerationManager.CheckForExtinction(Arg.Any<List<Person>>()).Returns(false);

            var mockSeedGenerator = Substitute.For<ISeedGenerator>();
            mockSeedGenerator.GetThreeWords().Returns(new List<SeedWord>());
            mockSeedGenerator.CreateWorldSeed(Arg.Any<List<SeedWord>>()).Returns(0);

            var mockHeartbeat = Substitute.For<IHeartbeat>();
            mockHeartbeat.GetCurrentTime().Returns(new DateTime(1, 6, 15));

            _mockPersonGenerator = Substitute.For<IPersonGenerator>();
            _mockPersonGenerator.Initialise(Arg.Any<List<InitialPerson>>(), Arg.Any<int>())
                .Returns(_people);
            _mockPersonGenerator.GenerateChildren(Arg.Any<List<Pair>>(),
                    Arg.Any<int>(),
                    Arg.Any<int>(),
                    Arg.Any<List<Person>>(),
                    Arg.Any<DateTime>())
                .Returns(new ChildGenerationResult { Children = new List<Person>(), People = _people });

            _mockAgeProcessor = Substitute.For<IAgeProcessor>();
            _mockAgeProcessor.Age(Arg.Any<List<Person>>(), Arg.Any<DateTime>()).Returns(x => x.ArgAt<List<Person>>(0));

            _mockDeathEngine = Substitute.For<IDeathEngine>();

            var mockPairingEngine = Substitute.For<IPairingEngine>();
            mockPairingEngine.GeneratePairs(Arg.Any<List<Person>>(),
                    Arg.Any<List<Pair>>(),
                    Arg.Any<int>(),
                    Arg.Any<DateTime>())
                .Returns(x => new PairingResult { Pairs = new List<Pair>(), People = x.ArgAt<List<Person>>(0) });

            _simulationManager = new SimulationManager(mockFileManager,
                mockGenerationManager,
                mockSeedGenerator,
                mockHeartbeat,
                _mockPersonGenerator,
                _mockAgeProcessor,
                _mockDeathEngine,
                mockPairingEngine,
                new GameState());
        }

        // The extinction check runs at the start of the day against yesterday's population,
        // so a day where everyone dies at once must not fall through to child generation
        // (which reads MaxBy(x => x.Id) off the now-empty people list).
        [Fact]
        public void ProgressDay_EveryoneDiesOnTheSameDay_DoesNotThrow()
        {
            _mockDeathEngine.ProcessDeaths(Arg.Any<List<Person>>(),
                    Arg.Any<int>(),
                    Arg.Any<DateTime>())
                .Returns(new List<Person>());
            _simulationManager.Start();

            var exception = Record.Exception(() => _simulationManager.ProgressDay());

            Assert.Null(exception);
        }

        // A child born today must be part of tomorrow's population. This also guards the
        // empty-population check against being inverted: if child generation stopped running
        // on normal days, the child would never appear and the second day's roll call fails.
        [Fact]
        public void ProgressDay_ChildBornToday_JoinsTheNextDaysPopulation()
        {
            var child = CreatePerson(99);
            _mockDeathEngine.ProcessDeaths(Arg.Any<List<Person>>(),
                    Arg.Any<int>(),
                    Arg.Any<DateTime>())
                .Returns(x => x.ArgAt<List<Person>>(0));
            _mockPersonGenerator.GenerateChildren(Arg.Any<List<Pair>>(),
                    Arg.Any<int>(),
                    Arg.Any<int>(),
                    Arg.Any<List<Person>>(),
                    Arg.Any<DateTime>())
                .Returns(x => new ChildGenerationResult
                {
                    Children = new List<Person> { child },
                    People = x.ArgAt<List<Person>>(3)
                });
            _simulationManager.Start();

            _simulationManager.ProgressDay();
            _simulationManager.ProgressDay();

            _mockAgeProcessor.Received().Age(Arg.Is<List<Person>>(x => x.Contains(child)),
                Arg.Any<DateTime>());
        }

        private static Person CreatePerson(int id) =>
            new()
            {
                Id = id,
                IsAlive = true,
                Age = 30,
                Name = new Name { FullName = $"Person{id}", Prefix = "Person", Suffix = $"{id}" },
                BirthMonth = 6,
                BirthDay = 15,
                DaysSinceLastChild = 0,
                WillHaveChildren = true,
                WillPair = true,
                IsFertile = true,
                HasPair = false
            };
    }
}