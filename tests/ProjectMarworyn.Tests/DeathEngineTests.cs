using NSubstitute;
using ProjectMarworyn.Core.Generators;
using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Models.Enums;
using ProjectMarworyn.Tests.Mocks;

namespace ProjectMarworyn.Tests
{
    public class DeathEngineTests
    {
        private readonly MockConsoleService _mockOutputService;

        public DeathEngineTests()
        {
            _mockOutputService = new MockConsoleService();
        }

        [Fact]
        public void ProcessDeaths_WithEmptyPeopleList_ReturnsEmptyLists()
        {
            var engine = CreateEngine(1.0);

            var currentGeneration = engine.ProcessDeaths(new List<Person>(),
                CreateGeneration(),
                0);

            Assert.Empty(currentGeneration.People);
        }

        [Fact]
        public void ProcessDeaths_WithEmptyPeopleList_PreservesIteration()
        {
            var engine = CreateEngine(1.0);

            var currentGeneration = engine.ProcessDeaths(new List<Person>(),
                CreateGeneration(5),
                0);

            Assert.Equal(5, currentGeneration.Iteration);
        }

        [Fact]
        public void ProcessDeaths_ReturnsNonNullLists()
        {
            var engine = CreateEngine(1.0);

            var currentGeneration = engine.ProcessDeaths(new List<Person>(),
                CreateGeneration(),
                0);

            Assert.NotNull(currentGeneration);
            Assert.NotNull(currentGeneration.People);
        }

        [Fact]
        public void ProcessDeaths_SkipsPeopleWithIsAliveFalse()
        {
            // 0.0 is below every bracket's threshold — any alive person would die,
            // which proves dead people are genuinely filtered before the death roll
            var engine = CreateEngine(0.0);
            var people = new List<Person> { CreatePerson(50, isAlive: false) };

            var result = engine.ProcessDeaths(people,
                CreateGeneration(),
                0);

            Assert.Empty(result.People);
            Assert.Empty(_mockOutputService.Lines);
        }

        [Fact]
        public void ProcessDeaths_MixedAliveAndDeadPeople_OnlyProcessesAlivePeople()
        {
            var engine = CreateEngine(1.0);
            var alivePerson = new Person { Id = 1, Name = new Name { FullName = "Alive" }, Age = 30, Biosex = Biosex.Male, IsAlive = true };
            var deadPerson = new Person { Id = 2, Name = new Name { FullName = "Dead" }, Age = 30, Biosex = Biosex.Female, IsAlive = false };
            var people = new List<Person> { alivePerson, deadPerson };

            var result = engine.ProcessDeaths(people,
                CreateGeneration(),
                0);

            Assert.Single(result.People);
            Assert.Contains(alivePerson, result.People);
        }

        [Fact]
        public void ProcessDeaths_WhenPersonDies_SetsIsAliveToFalse()
        {
            var engine = CreateEngine(0.0);
            var person = CreatePerson(50);

            engine.ProcessDeaths(new List<Person> { person },
                CreateGeneration(),
                0);

            Assert.False(person.IsAlive);
        }

        [Fact]
        public void ProcessDeaths_WhenPersonSurvives_IsAliveRemainsTrue()
        {
            var engine = CreateEngine(1.0);
            var person = CreatePerson(30);

            engine.ProcessDeaths(new List<Person> { person },
                CreateGeneration(),
                0);

            Assert.True(person.IsAlive);
        }

        [Fact]
        public void ProcessDeaths_SurvivorsListContainsOnlyAlivePeople()
        {
            var engine = CreateEngine(1.0);
            var people = new List<Person> { CreatePerson(10), CreatePerson(20), CreatePerson(30) };

            var result = engine.ProcessDeaths(people,
                CreateGeneration(),
                0);

            Assert.All(result.People, person => Assert.True(person.IsAlive));
        }

        [Fact]
        public void ProcessDeaths_SurvivingPeopleStoredInGeneration()
        {
            var engine = CreateEngine(1.0);
            var person1 = new Person { Id = 1, Name = new Name { FullName = "Survivor1" }, Age = 10, Biosex = Biosex.Female, IsAlive = true };
            var person2 = new Person { Id = 2, Name = new Name { FullName = "Survivor2" }, Age = 15, Biosex = Biosex.Male, IsAlive = true };
            var people = new List<Person> { person1, person2 };

            var currentGeneration = engine.ProcessDeaths(people,
                CreateGeneration(),
                0);

            Assert.Equal(2, currentGeneration.People.Count);
            Assert.All(currentGeneration.People, person => Assert.True(person.IsAlive));
        }

        [Fact]
        public void ProcessDeaths_PreservesGenerationIteration()
        {
            var engine = CreateEngine(1.0);

            var currentGeneration = engine.ProcessDeaths(new List<Person> { CreatePerson(20) },
                CreateGeneration(7),
                0);

            Assert.Equal(7, currentGeneration.Iteration);
        }

        [Fact]
        public void ProcessDeaths_OnDeath_WritesDeathMessageToConsole()
        {
            var engine = CreateEngine(0.0);

            engine.ProcessDeaths(new List<Person> { CreatePerson(50) },
                CreateGeneration(),
                0);

            Assert.NotEmpty(_mockOutputService.Lines);
        }

        [Fact]
        public void ProcessDeaths_OnSurvival_NoConsoleOutput()
        {
            var engine = CreateEngine(1.0);

            engine.ProcessDeaths(new List<Person> { CreatePerson(30) },
                CreateGeneration(),
                0);

            Assert.Empty(_mockOutputService.Lines);
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
                CreateGeneration(),
                0);

            Assert.Single(_mockOutputService.Lines);
            Assert.Contains("John Smith", _mockOutputService.Lines[0]);
            Assert.Contains("55", _mockOutputService.Lines[0]);
        }

        // Verifies the exact death probability thresholds for all 11 age brackets.
        // Formula: deathChance = (int)deathModifier / 100.0; death when NextDouble() * 100 <= deathChance
        // Threshold (NextDouble boundary) = (int)deathModifier / 10000
        // Test values use a 10% margin either side of each threshold.
        [Theory]
        [InlineData(5,   0.0054,  true)]  // Zero   (0-9,   modifier  60): threshold 0.006
        [InlineData(5,   0.0066,  false)]
        [InlineData(15,  0.0009,  true)]  // Ten    (10-19, modifier  10): threshold 0.001
        [InlineData(15,  0.0011,  false)]
        [InlineData(25,  0.0018,  true)]  // Twenty (20-29, modifier  20): threshold 0.002
        [InlineData(25,  0.0022,  false)]
        [InlineData(35,  0.0027,  true)]  // Thirty (30-39, modifier  30): threshold 0.003
        [InlineData(35,  0.0033,  false)]
        [InlineData(45,  0.0072,  true)]  // Fourty (40-49, modifier  80): threshold 0.008
        [InlineData(45,  0.0088,  false)]
        [InlineData(55,  0.0081,  true)]  // Fifty  (50-59, modifier  90): threshold 0.009
        [InlineData(55,  0.0099,  false)]
        [InlineData(65,  0.009,   true)]  // Sixty  (60-69, modifier 100): threshold 0.01
        [InlineData(65,  0.011,   false)]
        [InlineData(75,  0.0135,  true)]  // Seventy(70-79, modifier 150): threshold 0.015
        [InlineData(75,  0.0165,  false)]
        [InlineData(85,  0.034,   true)]  // Eighty (80-89, modifier 375): threshold 0.0375
        [InlineData(85,  0.041,   false)]
        [InlineData(95,  0.047,   true)]  // Ninety (90-99, modifier 525): threshold 0.0525
        [InlineData(95,  0.058,   false)]
        [InlineData(105, 0.135,   true)]  // Hundred(100+,  modifier 1500): threshold 0.15
        [InlineData(105, 0.165,   false)]
        public void ProcessDeaths_DeathProbabilityThreshold_CorrectlyDeterminesOutcome(int age,
            double nextDoubleValue,
            bool expectsDeath)
        {
            var engine = CreateEngine(nextDoubleValue);

            var result = engine.ProcessDeaths(new List<Person> { CreatePerson(age) },
                CreateGeneration(),
                0);

            if (expectsDeath)
                Assert.Empty(result.People);
            else
                Assert.Single(result.People);
        }

        // Verifies the switch expression assigns the correct DeathModifier at every age boundary.
        // A NextDouble value between the two adjacent thresholds produces opposite outcomes
        // for each side, proving the boundary is wired correctly.
        // The 9→10 boundary is an exception: infant mortality means 0-9 has a higher modifier than 10-19.
        [Theory]
        [InlineData(9,  10,  0.003,  true,  false)]  // Zero(60)→Ten(10):       threshold 0.006 vs 0.001, value between
        [InlineData(19, 20,  0.0015, false, true)]   // Ten(10)→Twenty(20):     threshold 0.001 vs 0.002
        [InlineData(29, 30,  0.0025, false, true)]   // Twenty(20)→Thirty(30):  threshold 0.002 vs 0.003
        [InlineData(39, 40,  0.005,  false, true)]   // Thirty(30)→Fourty(80):  threshold 0.003 vs 0.008
        [InlineData(49, 50,  0.0085, false, true)]   // Fourty(80)→Fifty(90):   threshold 0.008 vs 0.009
        [InlineData(59, 60,  0.0095, false, true)]   // Fifty(90)→Sixty(100):   threshold 0.009 vs 0.01
        [InlineData(69, 70,  0.0125, false, true)]   // Sixty(100)→Seventy(150):threshold 0.01  vs 0.015
        [InlineData(79, 80,  0.025,  false, true)]   // Seventy(150)→Eighty(375):threshold 0.015 vs 0.0375
        [InlineData(89, 90,  0.045,  false, true)]   // Eighty(375)→Ninety(525):threshold 0.0375 vs 0.0525
        [InlineData(99, 100, 0.1,    false, true)]   // Ninety(525)→Hundred(1500):threshold 0.0525 vs 0.15
        public void ProcessDeaths_AgeBoundaryTransition_CorrectModifierApplied(int lowerAge,
            int upperAge,
            double nextDoubleValue,
            bool lowerDies,
            bool upperDies)
        {
            var engine = CreateEngine(nextDoubleValue);
            var generation = CreateGeneration();

            var lowerSurvivors = engine.ProcessDeaths(new List<Person> { CreatePerson(lowerAge) },
                generation,
                0).People;
            var upperSurvivors = engine.ProcessDeaths(new List<Person> { CreatePerson(upperAge) },
                generation,
                0).People;

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
        // The mock returns 0 for Next(0,101), so the old path 0 <= 3.75 wrongly triggers death
        // for the Eighty bracket (modifier 375, deathChance 3.75, threshold 0.0375).
        // The correct float path gives 0.04 * 100 = 4.0 > 3.75, correctly giving survival.
        [Fact]
        public void ProcessDeaths_BugFix_FloatComparisonPreventsSpuriousDeath()
        {
            var engine = CreateEngine(0.04);

            var result = engine.ProcessDeaths(new List<Person> { CreatePerson(85) },
                CreateGeneration(),
                0);

            Assert.Single(result.People);
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

        private static Generation CreateGeneration(int iteration = 1)
        {
            return new Generation { Iteration = iteration, People = new List<Person>() };
        }

        private DeathEngine CreateEngine(double nextDoubleValue)
        {
            var mockDiceGenerator = Substitute.For<IDiceGenerator>();
            mockDiceGenerator.NextDouble(Arg.Any<Random>()).Returns(nextDoubleValue);
            return new DeathEngine(_mockOutputService,
                mockDiceGenerator);
        }
    }
}