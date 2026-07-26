using Microsoft.Extensions.Options;
using NSubstitute;
using ProjectMarworyn.Core;
using ProjectMarworyn.Core.Configuration;
using ProjectMarworyn.Core.Generators;
using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Models.Enums;

namespace ProjectMarworyn.UnitTests
{
    public class DeathEngineTests
    {
        private readonly GameState _gameState;

        public DeathEngineTests()
        {
            _gameState = new GameState();
        }

        [Fact]
        public void ProcessDeaths_WithEmptyPeopleList_ReturnsEmptyList()
        {
            var engine = CreateEngine(1.0);

            var survivors = engine.ProcessDeaths(new List<Person>(),
                0,
                new DateTime(1, 1, 1));

            Assert.Empty(survivors);
        }

        [Fact]
        public void ProcessDeaths_ReturnsNonNullList()
        {
            var engine = CreateEngine(1.0);

            var survivors = engine.ProcessDeaths(new List<Person>(),
                0,
                new DateTime(1, 1, 1));

            Assert.NotNull(survivors);
        }

        [Fact]
        public void ProcessDeaths_SkipsPeopleWithIsAliveFalse()
        {
            // 0.0 is below every bracket's threshold — any alive person would die,
            // which proves dead people are genuinely filtered before the death roll
            var engine = CreateEngine(0.0);
            var people = new List<Person> { CreatePerson(50, isAlive: false) };

            var survivors = engine.ProcessDeaths(people,
                0,
                new DateTime(1, 1, 1));

            Assert.Empty(survivors);
            Assert.Empty(_gameState.Text);
        }

        [Fact]
        public void ProcessDeaths_MixedAliveAndDeadPeople_OnlyProcessesAlivePeople()
        {
            var engine = CreateEngine(1.0);
            var alivePerson = new Person { Id = 1, Name = new Name { FullName = "Alive" }, Age = 30, Biosex = Biosex.Male, IsAlive = true };
            var deadPerson = new Person { Id = 2, Name = new Name { FullName = "Dead" }, Age = 30, Biosex = Biosex.Female, IsAlive = false };
            var people = new List<Person> { alivePerson, deadPerson };

            var survivors = engine.ProcessDeaths(people,
                0,
                new DateTime(1, 1, 1));

            Assert.Single(survivors);
            Assert.Contains(alivePerson, survivors);
        }

        [Fact]
        public void ProcessDeaths_WhenPersonDies_SetsIsAliveToFalse()
        {
            var engine = CreateEngine(0.0);
            var person = CreatePerson(50);

            engine.ProcessDeaths(new List<Person> { person },
                0,
                new DateTime(1, 1, 1));

            Assert.False(person.IsAlive);
        }

        [Fact]
        public void ProcessDeaths_WhenPersonSurvives_IsAliveRemainsTrue()
        {
            var engine = CreateEngine(1.0);
            var person = CreatePerson(30);

            engine.ProcessDeaths(new List<Person> { person },
                0,
                new DateTime(1, 1, 1));

            Assert.True(person.IsAlive);
        }

        [Fact]
        public void ProcessDeaths_SurvivorsListContainsOnlyAlivePeople()
        {
            var engine = CreateEngine(1.0);
            var people = new List<Person> { CreatePerson(10), CreatePerson(20), CreatePerson(30) };

            var survivors = engine.ProcessDeaths(people,
                0,
                new DateTime(1, 1, 1));

            Assert.All(survivors, person => Assert.True(person.IsAlive));
        }

        [Fact]
        public void ProcessDeaths_ReturnsAllSurvivors()
        {
            var engine = CreateEngine(1.0);
            var person1 = new Person { Id = 1, Name = new Name { FullName = "Survivor1" }, Age = 10, Biosex = Biosex.Female, IsAlive = true };
            var person2 = new Person { Id = 2, Name = new Name { FullName = "Survivor2" }, Age = 15, Biosex = Biosex.Male, IsAlive = true };
            var people = new List<Person> { person1, person2 };

            var survivors = engine.ProcessDeaths(people,
                0,
                new DateTime(1, 1, 1));

            Assert.Equal(2, survivors.Count);
            Assert.All(survivors, person => Assert.True(person.IsAlive));
        }

        [Fact]
        public void ProcessDeaths_OnDeath_WritesDeathMessageToConsole()
        {
            var engine = CreateEngine(0.0);

            engine.ProcessDeaths(new List<Person> { CreatePerson(50) },
                0,
                new DateTime(1, 1, 1));

            Assert.NotEmpty(_gameState.Text);
        }

        [Fact]
        public void ProcessDeaths_OnSurvival_NoConsoleOutput()
        {
            var engine = CreateEngine(1.0);

            engine.ProcessDeaths(new List<Person> { CreatePerson(30) },
                0,
                new DateTime(1, 1, 1));

            Assert.Empty(_gameState.Text);
        }

        [Fact]
        public void ProcessDeaths_DeathMessage_ContainsPersonNameAndAge()
        {
            var engine = CreateEngine(0.0);
            var person = new Person
            {
                Id = 1,
                Name = new Name { FullName = "John Smith" },
                Age = 55,
                Biosex = Biosex.Male,
                IsAlive = true
            };

            engine.ProcessDeaths(new List<Person> { person },
                0,
                new DateTime(1, 1, 1));

            Assert.Single(_gameState.Text);
            Assert.Contains("John Smith", _gameState.Text[0]);
            Assert.Contains("55", _gameState.Text[0]);
        }

        // Verifies the exact death probability thresholds for all 11 default age brackets.
        // Formula: death when NextDouble() * 100 <= DailyDeathChance (in %)
        // Threshold (NextDouble boundary) = DailyDeathChance / 100
        // Test values use a 10% margin either side of each threshold.
        [Theory]
        [InlineData(5,   0.0009,  true)]  // 0-9    (0.1%):  threshold 0.001
        [InlineData(5,   0.0011,  false)]
        [InlineData(15,  0.00009, true)]  // 10-19  (0.01%): threshold 0.0001
        [InlineData(15,  0.00011, false)]
        [InlineData(25,  0.00018, true)]  // 20-29  (0.02%): threshold 0.0002
        [InlineData(25,  0.00022, false)]
        [InlineData(35,  0.00045, true)]  // 30-39  (0.05%): threshold 0.0005
        [InlineData(35,  0.00055, false)]
        [InlineData(45,  0.0009,  true)]  // 40-49  (0.1%):  threshold 0.001
        [InlineData(45,  0.0011,  false)]
        [InlineData(55,  0.00135, true)]  // 50-59  (0.15%): threshold 0.0015
        [InlineData(55,  0.00165, false)]
        [InlineData(65,  0.0018,  true)]  // 60-69  (0.2%):  threshold 0.002
        [InlineData(65,  0.0022,  false)]
        [InlineData(75,  0.00225, true)]  // 70-79  (0.25%): threshold 0.0025
        [InlineData(75,  0.00275, false)]
        [InlineData(85,  0.0045,  true)]  // 80-89  (0.5%):  threshold 0.005
        [InlineData(85,  0.0055,  false)]
        [InlineData(95,  0.009,   true)]  // 90-99  (1.0%):  threshold 0.01
        [InlineData(95,  0.011,   false)]
        [InlineData(105, 0.0225,  true)]  // 100+   (2.5%):  threshold 0.025
        [InlineData(105, 0.0275,  false)]
        public void ProcessDeaths_DeathProbabilityThreshold_CorrectlyDeterminesOutcome(int age,
            double nextDoubleValue,
            bool expectsDeath)
        {
            var engine = CreateEngine(nextDoubleValue);

            var survivors = engine.ProcessDeaths(new List<Person> { CreatePerson(age) },
                0,
                new DateTime(1, 1, 1));

            if (expectsDeath)
                Assert.Empty(survivors);
            else
                Assert.Single(survivors);
        }

        // Verifies bracket selection at every age boundary of the default table.
        // A NextDouble value between the two adjacent thresholds produces opposite outcomes
        // for each side, proving the boundary is wired correctly.
        // The 9→10 boundary is an exception: infant mortality means 0-9 has a higher chance than 10-19.
        [Theory]
        [InlineData(9,  10,  0.0005,   true,  false)]  // 0.1%→0.01%:  threshold 0.001   vs 0.0001, value between
        [InlineData(19, 20,  0.00015,  false, true)]   // 0.01%→0.02%: threshold 0.0001  vs 0.0002
        [InlineData(29, 30,  0.00035,  false, true)]   // 0.02%→0.05%: threshold 0.0002  vs 0.0005
        [InlineData(39, 40,  0.00075,  false, true)]   // 0.05%→0.1%:  threshold 0.0005  vs 0.001
        [InlineData(49, 50,  0.00125,  false, true)]   // 0.1%→0.15%:  threshold 0.001   vs 0.0015
        [InlineData(59, 60,  0.00175,  false, true)]   // 0.15%→0.2%:  threshold 0.0015  vs 0.002
        [InlineData(69, 70,  0.00225,  false, true)]   // 0.2%→0.25%:  threshold 0.002   vs 0.0025
        [InlineData(79, 80,  0.00375,  false, true)]   // 0.25%→0.5%:  threshold 0.0025  vs 0.005
        [InlineData(89, 90,  0.0075,   false, true)]   // 0.5%→1.0%:   threshold 0.005   vs 0.01
        [InlineData(99, 100, 0.0175,   false, true)]   // 1.0%→2.5%:   threshold 0.01    vs 0.025
        public void ProcessDeaths_AgeBoundaryTransition_CorrectBracketApplied(int lowerAge,
            int upperAge,
            double nextDoubleValue,
            bool lowerDies,
            bool upperDies)
        {
            var engine = CreateEngine(nextDoubleValue);

            var lowerSurvivors = engine.ProcessDeaths(new List<Person> { CreatePerson(lowerAge) },
                0,
                new DateTime(1, 1, 1));
            var upperSurvivors = engine.ProcessDeaths(new List<Person> { CreatePerson(upperAge) },
                0,
                new DateTime(1, 1, 1));

            if (lowerDies)
                Assert.Empty(lowerSurvivors);
            else
                Assert.Single(lowerSurvivors);

            if (upperDies)
                Assert.Empty(upperSurvivors);
            else
                Assert.Single(upperSurvivors);
        }

        // Guards the bug fix: old code used random.Next(0, 101) (integer) against deathChance (decimal).
        // The mock returns 0 for Next(0,101), so the old path 0 <= deathChance wrongly triggers death
        // for the Eighty bracket (modifier 50, deathChance 0.5, threshold 0.005).
        // The correct float path gives 0.04 * 100 = 4.0 > 0.5, correctly giving survival.
        [Fact]
        public void ProcessDeaths_BugFix_FloatComparisonPreventsSpuriousDeath()
        {
            var engine = CreateEngine(0.04);

            var survivors = engine.ProcessDeaths(new List<Person> { CreatePerson(85) },
                0,
                new DateTime(1, 1, 1));

            Assert.Single(survivors);
        }

        // Custom brackets flip the default outcomes for both ages at the same dice roll,
        // so this fails if the engine stops reading the configured table.
        [Fact]
        public void ProcessDeaths_WithCustomDeathBrackets_UsesConfiguredChances()
        {
            var customBrackets = new List<DeathBracket>
            {
                new DeathBracket { MaxAge = 9, DailyDeathChance = 0 },
                new DeathBracket { DailyDeathChance = 100 }
            };
            var engine = CreateEngine(0.5,
                customBrackets);

            var childSurvivors = engine.ProcessDeaths(new List<Person> { CreatePerson(5) },
                0,
                new DateTime(1, 1, 1));
            var adultSurvivors = engine.ProcessDeaths(new List<Person> { CreatePerson(50) },
                0,
                new DateTime(1, 1, 1));

            Assert.Single(childSurvivors);
            Assert.Empty(adultSurvivors);
        }

        // A misconfigured table (hand-edited Appsettings.json) must fail at construction
        // with a clear message, not mid-run when someone outlives the last explicit bracket.
        [Fact]
        public void Constructor_WithNullDeathBrackets_Throws()
        {
            var exception = Record.Exception(() => CreateEngineWithBrackets(null));

            Assert.IsType<InvalidOperationException>(exception);
        }

        [Fact]
        public void Constructor_WithoutCatchAllBracket_Throws()
        {
            var bracketsWithoutCatchAll = new List<DeathBracket>
            {
                new DeathBracket { MaxAge = 99, DailyDeathChance = 1.0 }
            };

            var exception = Record.Exception(() => CreateEngineWithBrackets(bracketsWithoutCatchAll));

            Assert.IsType<InvalidOperationException>(exception);
        }

        [Fact]
        public void Constructor_WithCatchAllBracket_DoesNotThrow()
        {
            var exception = Record.Exception(() => CreateEngineWithBrackets(CreateDefaultDeathBrackets()));

            Assert.Null(exception);
        }

        private DeathEngine CreateEngineWithBrackets(List<DeathBracket> deathBrackets)
        {
            return new DeathEngine(Substitute.For<IDiceGenerator>(),
                _gameState,
                Options.Create(new AppSettings { DeathBrackets = deathBrackets }));
        }

        private static Person CreatePerson(int age, bool isAlive = true)
        {
            return new Person
            {
                Id = 1,
                Name = new Name { FullName = "TestPerson" },
                Age = age,
                Biosex = Biosex.Male,
                IsAlive = isAlive
            };
        }

        private DeathEngine CreateEngine(double nextDoubleValue,
            List<DeathBracket> deathBrackets = null)
        {
            var mockDiceGenerator = Substitute.For<IDiceGenerator>();
            mockDiceGenerator.NextDouble(Arg.Any<Random>()).Returns(nextDoubleValue);
            return new DeathEngine(mockDiceGenerator,
                _gameState,
                Options.Create(new AppSettings { DeathBrackets = deathBrackets ?? CreateDefaultDeathBrackets() }));
        }

        private static List<DeathBracket> CreateDefaultDeathBrackets()
        {
            return new List<DeathBracket>
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
        }
    }
}